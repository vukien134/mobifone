using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.YearCategories;
using Accounting.Common;
using Accounting.Configs;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Users;
using Accounting.EntityFrameworkCore;
using Accounting.Licenses;
using Accounting.Others;
using Accounting.Permissions;
using Accounting.Tenants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectMapping;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Volo.Abp.PermissionManagement;

namespace Accounting.Jobs.Tenants
{
    public class CreateTenantCustomerJob
        : AsyncBackgroundJob<CreateTenantCustomerArg>, ITransientDependency
    {
        #region Fields
        private readonly ILogger<CreateTenantCustomerJob> _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AccountingDb _accountingDb;
        private readonly TenantDb _tenantDb;
        private readonly DbServerService _dbServerService;
        private readonly CustomerRegisterService _customerRegisterService;
        private readonly IConfiguration _config;
        private readonly ICurrentTenant _currentTenant;
        private readonly ITenantManager _tenantManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly IDataSeeder _dataSeeder;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly IObjectMapper _objectMapper;
        private readonly IEmailSender _emailSender;
        private readonly IAbpDistributedLock _distributedLock;
        private readonly RegLicenseService _regLicenseService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly UserService _userService;
        private readonly RegLicenseInfoService _regLicenseInfoService;
        private readonly PermissionManager _permissionManager;

        #endregion
        #region Ctor
        public CreateTenantCustomerJob(ILogger<CreateTenantCustomerJob> logger,
                    IUnitOfWorkManager unitOfWorkManager,
                    AccountingDb accountingDb,
                    TenantDb tenantDb,
                    CustomerRegisterService customerRegisterService,
                    IConfiguration config,
                    ICurrentTenant currentTenant,
                    ITenantManager tenantManager,
                    ITenantRepository tenantRepository,
                    IDistributedEventBus distributedEventBus,
                    IDataSeeder dataSeeder,
                    TenantExtendInfoService tenantExtendInfoService,
                    IObjectMapper objectMapper,
                    DbServerService dbServerService,
                    IEmailSender emailSender,
                    IAbpDistributedLock distributedLock,
                    RegLicenseService regLicenseService,
                    LicenseBusiness licenseBusiness,
                    OrgUnitService orgUnitService,
                    YearCategoryService yearCategoryService,
                    UserService userService,
                    RegLicenseInfoService regLicenseInfoService,
                    PermissionManager permissionManager
                )
        {
            _logger = logger;
            _unitOfWorkManager = unitOfWorkManager;
            _accountingDb = accountingDb;
            _tenantDb = tenantDb;
            _customerRegisterService = customerRegisterService;
            _config = config;
            _currentTenant = currentTenant;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _distributedEventBus = distributedEventBus;
            _dataSeeder = dataSeeder;
            _tenantExtendInfoService = tenantExtendInfoService;
            _objectMapper = objectMapper;
            _dbServerService = dbServerService;
            _emailSender = emailSender;
            _distributedLock = distributedLock;
            _regLicenseService = regLicenseService;
            _licenseBusiness = licenseBusiness;
            _orgUnitService = orgUnitService;
            _yearCategoryService = yearCategoryService;
            _userService = userService;
            _regLicenseInfoService = regLicenseInfoService;
            _permissionManager = permissionManager;
        }
        #endregion
        #region Methods
        public override async Task ExecuteAsync(CreateTenantCustomerArg args)
        {
            if (string.IsNullOrEmpty(args.CustomerRegisterId)) return;

            using (var uow = _unitOfWorkManager.Begin(isTransactional: false))
            {
                var customerRegister = await this.GetCustomerRegister(args.CustomerRegisterId);
                if (customerRegister == null)
                {
                    await uow.CompleteAsync();
                    return;
                }

                if (customerRegister.Status == JobStatus.Completed)
                {
                    await uow.CompleteAsync();
                    return;
                };

                string error = "";

                try
                {
                    string workingDirectory = _config.GetValue<string>("DirectoryMigrate:Path");
                    if (!Directory.Exists(workingDirectory))
                    {
                        throw new Exception($"{workingDirectory} does not exists");
                    }
                    await this.SendMailForProcessing(customerRegister.Email);
                    var dbServer = await this.GetDbServer(customerRegister.IsDemo);
                    if (dbServer == null)
                    {
                        throw new Exception("Không tìm thấy dữ liệu DbServer");
                    }
                    string pass = RandomPassword.Generate(8, 10);
                    string key = ObjectId.GenerateNewId().ToString();
                    int ordSchema = dbServer.SchemaOrd.Value;
                    string schema = ordSchema.ToString().PadLeft(6, '0');
                    string connectionString = this.BuildConnectionString(schema, dbServer);
                    var tenant = await this.CreateTenant(customerRegister, pass, schema, connectionString);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await this.CreateSchema(tenant.Id, schema, dbServer);
                    this.CreateTable(connectionString);
                    await this.DataSeed(tenant.Id, customerRegister.Email, pass);
                    await this.UpdateTenantExtendInfo(tenant.Id, customerRegister);
                    await this.CreateLicense(tenant.Id, customerRegister, key);
                    await this.CreateLicenseInfo(tenant.Id, customerRegister, key);
                    var orgUnit = await this.CreateOrgUnit(customerRegister.TaxCode, customerRegister.CompanyName, tenant.Id, customerRegister.Email);
                    await this.CreateYearCategory(customerRegister.TaxCode, customerRegister.UsingDecision, tenant.Id);
                    await this.SendMailForCompleted(customerRegister.Email, pass, customerRegister);
                    customerRegister.Status = JobStatus.Completed;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    error = ex.Message;
                }

                if (!string.IsNullOrEmpty(error))
                {
                    customerRegister.Status = JobStatus.Error;
                    customerRegister.Note = error;
                }
                await this.UpdateCustomerRegister(customerRegister);
                await uow.CompleteAsync();
            }
        }
        #endregion
        #region Privates        
        private async Task CreateSchema(Guid? tenantId, string schema, DbServer dbServer)
        {
            using (_currentTenant.Change(tenantId))
            {
                string username = dbServer.UserName;
                //await _accountingDb.ExecuteSQLAsync($"CREATE USER u{username} WITH PASSWORD '{password}';");
                await _tenantDb.ExecuteSQLAsync($"CREATE SCHEMA IF NOT EXISTS s{schema} AUTHORIZATION {username};");

                string sql = $"CREATE TABLE IF NOT EXISTS s{schema}.\"__EFMigrationsHistory\" ( "
                                + "\"MigrationId\" varchar(150) NOT NULL,"
                                + "\"ProductVersion\" varchar(32) NOT NULL,"
                                + "CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY(\"MigrationId\")"
                                + ") TABLESPACE pg_default";
                await _tenantDb.ExecuteSQLAsync(sql);

                sql = $"ALTER TABLE s{schema}.\"__EFMigrationsHistory\" OWNER to {username};";
                await _tenantDb.ExecuteSQLAsync(sql);
            }
        }
        private string BuildConnectionString(string schema, DbServer dbServer)
        {
            if (string.IsNullOrEmpty(schema))
            {
                return $"Host = {dbServer.Name}; Port = {dbServer.Port}; Database = {dbServer.DatabaseName}; User ID = {dbServer.UserName}; Password = {dbServer.Password}";
            }

            return $"Host = {dbServer.Name}; Port = {dbServer.Port}; Database = {dbServer.DatabaseName}; User ID = {dbServer.UserName}; Password = {dbServer.Password}; SearchPath = s{schema}";
        }
        private void CreateTable(string connectionString)
        {
            string outputText = string.Empty;
            var standardError = string.Empty;
            string workingDirectory = _config.GetValue<string>("DirectoryMigrate:Path");

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"ef database update -c TenancyDbContext -- ConnectionString=\"{connectionString}\"",
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                };

                process.Start();
                outputText = process.StandardOutput.ReadToEnd();
                outputText = outputText.Replace(Environment.NewLine, string.Empty);
                standardError = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (!string.IsNullOrEmpty(standardError))
                {
                    throw new Exception(standardError);
                }
            }

        }
        private async Task<Tenant> CreateTenant(CustomerRegister customerRegister,
                            string pass, string schema, string connectionString)
        {
            var teantCreateDto = new TenantCreateDto()
            {
                Name = customerRegister.AccessCode,
                AdminEmailAddress = customerRegister.Email,
                AdminPassword = pass
            };
            using (_currentTenant.Change(null))
            {
                var tenant = await _tenantManager.CreateAsync(teantCreateDto.Name);
                teantCreateDto.MapExtraPropertiesTo(tenant);
                tenant.SetConnectionString(customerRegister.AccessCode, connectionString);
                await _tenantRepository.InsertAsync(tenant);
                //await _cu.SaveChangesAsync();
                await _distributedEventBus.PublishAsync(
                    new TenantCreatedEto
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        Properties =
                        {
                        { "AdminEmail", customerRegister.Email },
                        { "AdminPassword", pass }
                        }
                    }
                );
                return tenant;
            }
        }
        private async Task DataSeed(Guid? tenantId, string email, string pass)
        {
            using (_currentTenant.Change(tenantId))
            {
                await _dataSeeder.SeedAsync(
                        new DataSeedContext(tenantId)
                            .WithProperty("AdminEmail", email)
                            .WithProperty("AdminPassword", pass)
                        );
                // await _permissionManager.SetForRoleAsync("admin", "VatInvoice_Management_View", false);
                await _permissionManager.SetForRoleAsync("admin", "VatInvoice_Management_Create", false);
                await _permissionManager.SetForRoleAsync("admin", "InvoiceStatus_Management", false);
                await _permissionManager.SetForRoleAsync("admin", "InvoiceStatus_Management_View", false);
                await _permissionManager.SetForRoleAsync("admin", "InvoiceStatus_Management_Create", false);
                await _permissionManager.SetForRoleAsync("admin", "InvoiceStatus_Management_Update", false);
                await _permissionManager.SetForRoleAsync("admin", "InvoiceStatus_Management_Delete", false);
                
            }
        }
        private async Task UpdateTenantExtendInfo(Guid? tenantId, CustomerRegister customerRegister)
        {
            using (_currentTenant.Change(null))
            {
                var tenantExtendInfo = await _tenantExtendInfoService.GetByTenantId(tenantId);
                if (tenantExtendInfo == null)
                {
                    var dto = new CrudTenantExtendInfoDto()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        TenantId = tenantId,
                        TenantType = customerRegister.Type,
                        RegNumCompany = customerRegister.RegNumCompany,
                        CompanyType = customerRegister.CompanyType,
                        RegNumMonth = customerRegister.RegNumMonth,
                        RegNumUser = customerRegister.RegNumUser
                    };
                    var entity = _objectMapper.Map<CrudTenantExtendInfoDto, TenantExtendInfo>(dto);
                    await _tenantExtendInfoService.CreateAsync(entity);
                }
            }
        }
        private async Task CreateLicense(Guid? tenantId, CustomerRegister customerRegister, string key)
        {
            CrudRegLicenseDto dto = new()
            {
                ApprovalDate = DateTime.Now,
                CompanyQuantity = customerRegister.RegNumCompany,
                CompanyType = customerRegister.CompanyType,
                CustomerTenantId = tenantId,
                Id = ObjectId.GenerateNewId().ToString(),
                IsApproval = true,
                Key = key,
                LicXml = null,
                Month = customerRegister.RegNumMonth,
                Name = customerRegister.CompanyName,
                RegDate = DateTime.Now,
                TaxCode = customerRegister.TaxCode,
                TypeLic = customerRegister.Type,
                UserQuantity = customerRegister.RegNumUser,
                EndDate = customerRegister.EndDate
            };

            using (_currentTenant.Change(null))
            {
                var entity = _objectMapper.Map<CrudRegLicenseDto, RegLicense>(dto);
                await _regLicenseService.CreateAsync(entity);
            }
        }
        private async Task CreateLicenseInfo(Guid? tenantId, CustomerRegister customerRegister, string key)
        {
            CrudReglicenseInfoDto dto = new()
            {
                ApprovalDate = DateTime.Now,
                CompanyQuantity = customerRegister.RegNumCompany,
                CompanyType = customerRegister.CompanyType,
                CustomerTenantId = tenantId,
                Id = ObjectId.GenerateNewId().ToString(),
                IsApproval = true,
                Key = null,
                LicXml = null,
                Month = customerRegister.RegNumMonth,
                Name = customerRegister.CompanyName,
                RegDate =customerRegister.StartDate,
                TaxCode = customerRegister.TaxCode,
                TypeLic = customerRegister.Type,
                UserQuantity = customerRegister.RegNumUser,
                EndDate = customerRegister.EndDate,
              

            };

            using (_currentTenant.Change(tenantId))
            {
                var entity = _objectMapper.Map<CrudReglicenseInfoDto, RegLicenseInfo>(dto);
                await _regLicenseInfoService.CreateAsync(entity);
            }
        }
        private async Task<CustomerRegister> GetCustomerRegister(string customerRegisterId)
        {
            var customerRegister = await _customerRegisterService.FindAsync(customerRegisterId);
            return customerRegister;
        }
        private async Task<DbServer> GetDbServer(bool? isDemo)
        {
            await using (var handle = await _distributedLock.TryAcquireAsync("SchemaOrdUp"))
            {
                if (handle != null)
                {
                    var dbServer = await _dbServerService.GetActiveServer(isDemo);
                    if (dbServer == null) return null;

                    dbServer.SchemaOrd = dbServer.SchemaOrd + 1;
                    await this.UpdateDbServer(dbServer);
                    return dbServer;
                }
            }
            return null;
        }
        private async Task UpdateDbServer(DbServer dbServer)
        {
            using (_currentTenant.Change(null))
            {
                await _dbServerService.UpdateAsync(dbServer);
            }
        }
        private async Task UpdateCustomerRegister(CustomerRegister register)
        {
            using (_currentTenant.Change(null))
            {
                await _customerRegisterService.UpdateAsync(register);
            }
        }
        private async Task SendMailForProcessing(string to)
        {
            await _emailSender.SendAsync(
                to,     // target email address
                "Cảm ơn đã đăng ký sử dụng dịch vụ",         // subject
                "Chúng tôi đang tiến hành xử lý yêu cầu của bạn"  // email body
            );
        }
        private async Task SendMailForCompleted(string to, string pass, CustomerRegister customerRegister)
        {
            string suffixe = "ketoan-mas.vn";
            if (customerRegister.Type == 1)
            {
                string body =
                $"Kính gửi Quý khách hàng,<br>" +
                $"Chúng tôi vừa tạo tài khoản cho Quý khách để truy cập dùng thử Phần mềm kế toán MobiFone Accounting Solution của Tổng Công ty Viễn thông MobiFone với thông tin như sau:<br>" +
                $"Tên đăng nhập: admin <br>" +
                $"Mật khẩu:  {pass}<br>" +
                $"Quý khách vui lòng truy cập Hệ thống theo đường dẫn: https://{customerRegister.AccessCode}.{suffixe}/ <br>" +
                $"Quý khách lưu ý, đây là email tự động từ hệ thống, vui lòng không trả lời lại email này!<br>" +
                $"Thời gian dùng thử là 30 ngày <br>" +
                $"MobiFone xin chân thành cảm ơn sự hợp tác và tin dùng của Quý khách hàng.<br>" +
                $"Trân trọng. <br>";

                string subject = "Thông báo kết quả đăng ký";
                await _emailSender.SendAsync(
                    to,     // target email address
                    subject,         // subject
                    body  // email body
                );
            }
            if (customerRegister.Type == 2)
            {
                string body =
                $"Kính gửi Quý khách hàng,<br>" +
                $"Chúng tôi vừa tạo tài khoản cho Quý khách để truy cập dùng thử Phần mềm kế toán MobiFone Accounting Solution của Tổng Công ty Viễn thông MobiFone với thông tin như sau:<br>" +
                $"Tên đăng nhập: admin <br>" +
                $"Mật khẩu:  {pass}<br>" +
                $"Quý khách vui lòng truy cập Hệ thống theo đường dẫn: https://{customerRegister.AccessCode}.{suffixe}/ <br>" +
                $"Quý khách lưu ý, đây là email tự động từ hệ thống, vui lòng không trả lời lại email này!<br>" +
                $"Thời gian dùng thử là 14 ngày <br>" +
                $"MobiFone xin chân thành cảm ơn sự hợp tác và tin dùng của Quý khách hàng.<br>" +
                $"Trân trọng. <br>";

                string subject = "Thông báo kết quả đăng ký";
                await _emailSender.SendAsync(
                    to,     // target email address
                    subject,         // subject
                    body  // email body
                );
            }
            if (customerRegister.IsDemo == true)
            {
                await PostMobi(customerRegister);
            }
        }
        private async Task PostMobi(CustomerRegister dto)
        {
            string apiUrl = _config.GetValue<string>("ApiMobi:RegisterAccessInformation");
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            try
            {
                HttpClient client = new HttpClient(clientHandler);
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.CompanyName ?? ""), "mauticform[ten_cong_ty]");
                formData.Add(new StringContent(dto.TaxCode ?? ""), "mauticform[ma_so_thue]");
                formData.Add(new StringContent(dto.AccessCode ?? ""), "mauticform[ma_truy_cap]");
                formData.Add(new StringContent(dto.CompanyType ?? ""), "mauticform[loai_hinh_su_dung]");
                formData.Add(new StringContent(dto.UsingDecision.ToString()), "mauticform[thong_tu_ap_dung]");
                formData.Add(new StringContent(dto.FullName ?? ""), "mauticform[ho_va_ten]");
                formData.Add(new StringContent(dto.PhoneNumber ?? ""), "mauticform[so_dien_thoai]");
                formData.Add(new StringContent(dto.Email ?? ""), "mauticform[email]");
                formData.Add(new StringContent("59"), "mauticform[formId]");
                formData.Add(new StringContent("optinformdangkydungthuphanmemketoan"), "mauticform[formName]");

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");

                var response = await client.PostAsync(apiUrl, formData);
                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Data postMobi: " + formData.ToString());
                _logger.LogInformation(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private async Task<OrgUnit> CreateOrgUnit(string taxCode, string name, Guid? tenantId,
                                                    string email)
        {
            var dto = new CrudOrgUnitDto()
            {
                Code = taxCode,
                Name = name,
                Id = ObjectId.GenerateNewId().ToString(),
                TaxCode = taxCode
            };
            var entity = _objectMapper.Map<CrudOrgUnitDto, OrgUnit>(dto);
            entity.TenantId = tenantId;
            using (_currentTenant.Change(tenantId))
            {
                entity = await _orgUnitService.CreateAsync(entity);
                var user = await _userService.GetByEmailAsync(email);
                var dtoOrgPermission = new CrudOrgUnitPermissionDto()
                {
                    OrgUnitId = entity.Id,
                    UserId = user.Id,
                    Id = ObjectId.GenerateNewId().ToString()
                };
                var entityOrgPermission = _objectMapper.Map<CrudOrgUnitPermissionDto, OrgUnitPermission>(dtoOrgPermission);
                entityOrgPermission.TenantId = tenantId;
                await _orgUnitService.InsertOrgUnitPermissionAsync(entityOrgPermission);
            }

            return entity;
        }
        private async Task CreateYearCategory(string taxCode, int? usingDecision, Guid? tenantId)
        {
            DateTime now = DateTime.Now;

            var dto = new CruYearCategoryDto()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                BeginDate = new DateTime(now.Year, 1, 1),
                EndDate = new DateTime(now.Year, 12, 31),
                OrgCode = taxCode,
                Year = now.Year,
                UsingDecision = usingDecision,
            };
            var entity = _objectMapper.Map<CruYearCategoryDto, YearCategory>(dto);
            entity.TenantId = tenantId;
            using (_currentTenant.Change(tenantId))
            {
                await _yearCategoryService.CreateAsync(entity);
            }
        }
        #endregion
    }
}
