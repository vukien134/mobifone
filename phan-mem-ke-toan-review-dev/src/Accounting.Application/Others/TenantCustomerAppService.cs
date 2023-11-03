using Accounting.BaseDtos;
using Accounting.Configs;
using Accounting.Constants;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.EntityFrameworkCore;
using Accounting.Exceptions;
using Accounting.Jobs.Tenants;
using Accounting.Licenses;
using Accounting.Localization;
using Accounting.Migrations.HostDbMigration;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Settings;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using static Volo.Abp.TenantManagement.TenantManagementPermissions;

namespace Accounting.Others
{
    public class TenantCustomerAppService : AccountingAppService
    {
        private readonly ILogger<TenantCustomerAppService> _logger;
        private readonly AccountingDb _accountingDb;
        private readonly ICurrentTenant _currentTenant;
        private readonly ITenantManager _tenantManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly IDistributedEventBus _distributedEventBus;
        private readonly IDataSeeder _dataSeeder;
        private readonly IConfiguration _config;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly CustomerRegisterService _customerRegisterService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ISettingEncryptionService _settingEncryptionService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly PackgeMobiService _packgeMobiService;
        private readonly LicenseAppService _licenseAppService;
        private readonly RegLicenseService _regLicenseService;
        private readonly RegLicenseInfoService _regLicenseInfoService;
        public TenantCustomerAppService(ILogger<TenantCustomerAppService> logger,
                    AccountingDb accountingDb,
                    ICurrentTenant currentTenant,
                    ITenantManager tenantManager,
                    ITenantRepository tenantRepository,
                    IDistributedEventBus distributedEventBus,
                    IDataSeeder dataSeeder,
                    IConfiguration config,
                    IUnitOfWorkManager unitOfWorkManager,
                    CustomerRegisterService customerRegisterService,
                    IBackgroundJobManager backgroundJobManager,
                    ISettingEncryptionService settingEncryptionService,
                    IStringLocalizer<AccountingResource> localizer,
                    PackgeMobiService packgeMobiService,
                    LicenseAppService licenseAppService,
                    RegLicenseService regLicenseService,
                    RegLicenseInfoService regLicenseInfoService
            )
        {
            _logger = logger;
            _accountingDb = accountingDb;
            _currentTenant = currentTenant;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _distributedEventBus = distributedEventBus;
            _dataSeeder = dataSeeder;
            _config = config;
            _unitOfWorkManager = unitOfWorkManager;
            _customerRegisterService = customerRegisterService;
            _backgroundJobManager = backgroundJobManager;
            _settingEncryptionService = settingEncryptionService;
            _localizer = localizer;
            _packgeMobiService = packgeMobiService;
            _licenseAppService = licenseAppService;
            _regLicenseService = regLicenseService;
            _regLicenseInfoService = regLicenseInfoService;
        }
        [UnitOfWork(isTransactional: false)]
        [AllowAnonymous]
        public async Task CreateAsync(CrudTenantDto dto)
        {
            dto.AccessCode = dto.AccessCode.Trim();
            if (dto.AccessCode.Contains("."))
            {
                throw new Exception("Không thể sử dụng dấu . để đặt cho mã truy cập, xin vui lòng thử lại!");
            }
            if (dto.AccessCode.Contains(" "))
            {
                throw new Exception("Không thể sử dụng khoảng trắng để đặt cho mã truy cập, xin vui lòng thử lại!");
            }
            dto.AccessCode = dto.AccessCode.ToLower();
            var tenant = await _tenantRepository.FindByNameAsync(dto.AccessCode);
            if (tenant != null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Tenant, ErrorCode.Duplicate),
                        $"Tenant Name ['{dto.AccessCode}'] already exist ");
            }
            if (dto.AccessCode.Count() > 50)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Tenant, ErrorCode.Other),
                        _localizer["Err:AccessCodeOverCharacter"]);
            }

            if (dto.TaxCode != null)
            {
                if (dto.TaxCode.Count() > 20)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Tenant, ErrorCode.Other),
                        _localizer["Err:TaxCodeOverCharacter"]);
                }
                dto.TaxCode = dto.TaxCode.Trim();
            }

            var customerRegister = await this.InsertCustomerRegister(dto);
            await _backgroundJobManager.EnqueueAsync(
                new CreateTenantCustomerArg
                {
                    CustomerRegisterId = customerRegister.Id
                }
            );
        }

        public async Task<ResultDto> GetStatusExpired()
        {
            var customerRegister = await _customerRegisterService.GetByAccessCode(_currentTenant.Name);
            var res = new ResultDto();
            var dateNow = DateTime.Now;
            TimeSpan time = (customerRegister.EndDate ?? DateTime.Now) - dateNow;
            if (time.Days < 0 && customerRegister.IsDemo == true)
            {
                res.Ok = false;
                res.Message = "Tài khoản của Quý khách đã hết hạn dùng thử. Vui lòng liên hệ MobiFone để mua bản quyền sử dụng chính thức. Xin cảm ơn!";
                return res;
            }
            res.Ok = true;
            res.Message = "Thành công";
            return res;
        }

        private async Task<CustomerRegister> InsertCustomerRegister(CrudTenantDto dto)
        {
            var customerRegister = await _customerRegisterService.GetByAccessCode(dto.AccessCode);
            if (customerRegister != null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Duplicate),
                        $"Đăng ký không thành công.Bạn đã tạo tài khoản dùng thử.Vui lòng liên hệ quản trị hệ thống để được hỗ trợ");
            }
            var lstCustomerRegister = await _customerRegisterService.GetQueryableAsync();
            var customerRegisterEmail = lstCustomerRegister.Where(p => p.Email == dto.Email && p.AccessCode.Contains("-demo")== false ).ToList();
            if (customerRegisterEmail.Count > 0)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Duplicate),
                        "Đăng ký không thành công.Bạn đã tạo tài khoản dùng thử.Vui lòng liên hệ quản trị hệ thống để được hỗ trợ");
            }
            if (dto.Type != 6)
            {
                bool checkValue = CheckValueDate(dto);
            }
            dto.Id = this.GetNewObjectId();
            var entity = ObjectMapper.Map<CrudTenantDto, CustomerRegister>(dto);
            entity.Status = JobStatus.WaitForRun;
            DateTime startDate = DateTime.Now;
            if (entity.IsDemo==true)
            {
                if (entity.Type==1)
                {
                    entity.EndDate = startDate.AddDays(30);
                }else if (entity.Type==2)
                {
                    entity.EndDate = startDate.AddDays(14);
                }
            }
            await _customerRegisterService.CreateAsync(entity, true);
            await CurrentUnitOfWork.SaveChangesAsync();
            return entity;
        }

        [AllowAnonymous]
        public string EncryptEmailPassword(string pass)
        {
            if (string.IsNullOrWhiteSpace(pass)) return null;

            SettingDefinition settingDefinition = new(AbpSettingKey.ABP_MAILING_SMTP_PASSWORD, isEncrypted: true);
            return _settingEncryptionService.Encrypt(settingDefinition, pass);
        }
        [AllowAnonymous]
        public string DecryptEmailPassword(string encryptedValued)
        {
            if (string.IsNullOrWhiteSpace(encryptedValued)) return encryptedValued;
            SettingDefinition settingDefinition = new(AbpSettingKey.ABP_MAILING_SMTP_PASSWORD, isEncrypted: true);
            return _settingEncryptionService.Decrypt(settingDefinition, encryptedValued);
        }
        private static bool IsValid(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";


            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
        private static bool CheckValueDate(CrudTenantDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                        $"Email is empty ");

            }
            else
            {
                bool chekcValudate = IsValid(dto.Email);
                if (chekcValudate == false)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"Invalid email form ");
                }
            }
            if (string.IsNullOrEmpty(dto.Type.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"Type is empty ");
            }
            else
            {
                if (!"1,2".Contains(dto.Type.ToString()))
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                     $"Invalid Type form ");
                }
                else
                {
                    if (dto.Type == 1)
                    {
                        if (!("Service, Commerce,Manufacture,Construct, Synthetic").Contains(dto.CompanyType))
                        {
                            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                                    $"Type =1. CompanyType Type form. UsingDecision = " + dto.UsingDecision.ToString());
                        }
                        if (!("200,133").Contains(dto.UsingDecision.ToString()))
                        {
                            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                                    $"Type =1." + "CompanyType = " + dto.CompanyType + ". UsingDecision Type form ");
                        }
                    }
                    else
                    {
                        if (!("Household").Contains(dto.CompanyType))
                        {
                            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                                    $"Type=2. CompanyType Type form. UsingDecision= " + dto.UsingDecision);
                        }
                        if (!("88").Contains(dto.UsingDecision.ToString()))
                        {
                            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                                    $"Type=2.CompanyType = " + dto.CompanyType + ". UsingDecision Type form");
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(dto.TaxCode.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"Taxcode is empty ");
            }
            if (string.IsNullOrEmpty(dto.CompanyType.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"CompanyType code is empty ");
            }

            if (string.IsNullOrEmpty(dto.CompanyName.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"CompanyName code is empty ");
            }
            if (string.IsNullOrEmpty(dto.UsingDecision.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"UsingDecision code is empty ");
            }
            if (string.IsNullOrEmpty(dto.RegNumUser.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"RegNumUser code is empty ");
            }
            if (string.IsNullOrEmpty(dto.RegNumMonth.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"RegNumMonth code is empty ");
            }
            if (string.IsNullOrEmpty(dto.RegNumCompany.ToString()))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                       $"RegNumCompany code is empty ");
            }
            return true;
        }
        [UnitOfWork(isTransactional: false)]
        [AllowAnonymous]
        public async Task<JsonObject> CreateCrmAsync(JsonObject dto)
        {
            JsonObject keyValuePairs = new JsonObject();
            if (dto["type"].ToString() == "1")
            {
                var customerRegister = await _customerRegisterService.GetByAccessCode(dto["AccessCode"].ToString());

                if (customerRegister != null)
                {
                    keyValuePairs.Add("status", "2");
                    keyValuePairs.Add("message", "Tạo mới thông tin dịch vụ không thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;

                }
                var lstCustomerRegisters = await _customerRegisterService.GetQueryableAsync();
                var customerRegisterEmail = lstCustomerRegisters.Where(p => p.Email == dto["adminEmail"].ToString()).ToList();
                if (customerRegisterEmail.Count > 0)
                {

                    keyValuePairs.Add("status", "2");
                    keyValuePairs.Add("message", "Tạo mới thông tin dịch vụ không thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;
                }
                if (customerRegister== null || customerRegisterEmail.Count==0)
               
                {
                    keyValuePairs.Add("status", "1");
                    keyValuePairs.Add("message", "Tạo mới thông tin dịch vụ thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;

                }

            }
            if (dto["type"].ToString() == "2")
            {
                var accessCode = dto["AccessCode"].ToString().Trim();
                accessCode = accessCode.ToLower();
                var lstCustomerRegister = await _customerRegisterService.GetByAccessCode(accessCode);
                if (lstCustomerRegister != null)
                {
                    keyValuePairs.Add("status", "1");
                    keyValuePairs.Add("message", "Update thông tin tài khoản thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;
                }
                else
                {
                    keyValuePairs.Add("status", "2");
                    keyValuePairs.Add("message", "Update thông tin tài khoản không thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;
                }


            }
            if (dto["type"].ToString() == "3")
            {
                var accessCode = dto["AccessCode"].ToString().Trim();
                accessCode = accessCode.ToLower();
                var lstCustomerRegister = await _customerRegisterService.GetByAccessCode(accessCode);
                lstCustomerRegister.EndDate = DateTime.Now;
                await _customerRegisterService.UpdateAsync(lstCustomerRegister, true);
                var tenant = await _tenantRepository.FindByNameAsync(accessCode);

                var regLicense = await _regLicenseService.GetByTenantId(tenant.Id);
                CrudReglicenseInfoDto Cr = new CrudReglicenseInfoDto();
                Cr.CustomerTenantId = tenant.Id;
                Cr.Month = regLicense.Month;
                Cr.Name = regLicense.Name;
                Cr.ApprovalDate = regLicense.ApprovalDate;
                Cr.EndDate = DateTime.Now;
                Cr.TaxCode = regLicense.TaxCode;
                Cr.IsApproval = regLicense.IsApproval;  
                Cr.CompanyQuantity = regLicense.CompanyQuantity;
                Cr.Id= this.GetNewObjectId();
                Cr.TypeLic = regLicense.TypeLic;
                Cr.RegDate = regLicense.RegDate;
                Cr.CompanyType= regLicense.CompanyType;
                Cr.UserQuantity = regLicense.UserQuantity;
                Cr.Code = "Pause";
                using (_currentTenant.Change(tenant.Id))
                {
                    var regLicenseInfo = ObjectMapper.Map<CrudReglicenseInfoDto, RegLicenseInfo>(Cr);
                    await _regLicenseInfoService.CreateAsync(regLicenseInfo, true);

                    keyValuePairs.Add("status", "1");
                    keyValuePairs.Add("message", "Tạm dừng dịch vụ thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;


                }


            }
            if (dto["type"].ToString() == "4")
            {
                var accessCode = dto["AccessCode"].ToString().Trim();
                accessCode = accessCode.ToLower();
                var lstCustomerRegister = await _customerRegisterService.GetByAccessCode(accessCode);
               // keyValuePairs = await _licenseAppService.CreateLicenseAsync(dto);
                JsonArray listPackage = (JsonArray)JsonArray.Parse(dto["listPackage"].ToString());
                lstCustomerRegister.EndDate = DateTime.Parse(listPackage[0]["endDate"].ToString());
                await _customerRegisterService.UpdateAsync(lstCustomerRegister,true);
                var tenant = await _tenantRepository.FindByNameAsync(accessCode);

                var regLicense = await _regLicenseService.GetByTenantId(tenant.Id);
                CrudReglicenseInfoDto Cr = new CrudReglicenseInfoDto();
                Cr.CustomerTenantId = tenant.Id;
                Cr.Month = regLicense.Month;
                Cr.Name = regLicense.Name;
                Cr.ApprovalDate = regLicense.ApprovalDate;
                Cr.EndDate = DateTime.Now;
                Cr.TaxCode = regLicense.TaxCode;
                Cr.IsApproval = regLicense.IsApproval;
                Cr.CompanyQuantity = regLicense.CompanyQuantity;
                Cr.Id = this.GetNewObjectId();
                Cr.TypeLic = regLicense.TypeLic;
                Cr.RegDate = regLicense.RegDate;
                Cr.CompanyType = regLicense.CompanyType;
                Cr.UserQuantity = regLicense.UserQuantity;
                Cr.Code = "Start";
                using (_currentTenant.Change(tenant.Id))
                {
                    var regLicenseInfo = ObjectMapper.Map<CrudReglicenseInfoDto, RegLicenseInfo>(Cr);
                    await _regLicenseInfoService.CreateAsync(regLicenseInfo, true);

                    keyValuePairs.Add("status", "1");
                    keyValuePairs.Add("message", "Kích hoạt dịch vụ thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;


                }


            }
            if (dto["type"].ToString() == "5")
            {
                var accessCode = dto["AccessCode"].ToString().Trim();
                accessCode = accessCode.ToLower();
                var lstCustomerRegister = await _customerRegisterService.GetByAccessCode(accessCode);
                lstCustomerRegister.EndDate = DateTime.Now;
                await _customerRegisterService.UpdateAsync(lstCustomerRegister);
                var tenant = await _tenantRepository.FindByNameAsync(accessCode);

                var regLicense = await _regLicenseService.GetByTenantId(tenant.Id);
                CrudReglicenseInfoDto Cr = new CrudReglicenseInfoDto();
                Cr.CustomerTenantId = tenant.Id;
                Cr.Month = regLicense.Month;
                Cr.Name = regLicense.Name;
                Cr.ApprovalDate = regLicense.ApprovalDate;
                Cr.EndDate = DateTime.Now;
                Cr.TaxCode = regLicense.TaxCode;
                Cr.IsApproval = regLicense.IsApproval;
                Cr.CompanyQuantity = regLicense.CompanyQuantity;
                Cr.Id = this.GetNewObjectId();
                Cr.TypeLic = regLicense.TypeLic;
                Cr.RegDate = regLicense.RegDate;
                Cr.CompanyType = regLicense.CompanyType;
                Cr.UserQuantity = regLicense.UserQuantity;
                Cr.Code = "Stop";
                using (_currentTenant.Change(tenant.Id))
                {
                    var regLicenseInfo = ObjectMapper.Map<CrudReglicenseInfoDto, RegLicenseInfo>(Cr);
                    await _regLicenseInfoService.CreateAsync(regLicenseInfo, true);

                    keyValuePairs.Add("status", "1");
                    keyValuePairs.Add("message", "Xóa tài khoản thành công");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;


                }


            }
            if (dto["type"].ToString() == "6")
            {
                var accessCode = dto["AccessCode"].ToString().Trim();
                accessCode = accessCode.ToLower();
                var lstCustomerRegister = await _customerRegisterService.GetByAccessCode(accessCode);

                if (lstCustomerRegister == null)
                {
                    JsonArray listPackage = (JsonArray)JsonArray.Parse(dto["listPackage"].ToString());

                    var lstpackeMobi = await _packgeMobiService.GetQueryableAsync();
                    var code = listPackage[0]["packageCode"].ToString();
                    var pageMobi = lstpackeMobi.Where(p => p.Code == code).FirstOrDefault();
                    var usingDecision = dto["usingDecision"].ToString();
                    if (usingDecision == "88")
                    {
                        pageMobi.Type = 2;
                    }
                    else
                    {
                        pageMobi.Type =1;
                    }
                    DateTime endDate = DateTime.Parse(listPackage[0]["endDate"].ToString());
                    DateTime startDate = DateTime.Parse(listPackage[0]["startDate"].ToString());
                    TimeSpan Time =  endDate- startDate;
                    int numberOfMonths;

                    if (Time.TotalDays < 30)
                    {
                        numberOfMonths = 1;
                    }
                    else
                    {
                        int daysInMonth = DateTime.DaysInMonth(endDate.Year, endDate.Month);
                        int remainingDaysInFirstMonth = daysInMonth - endDate.Day + 1;
                        int daysInLastMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                        int elapsedDaysInLastMonth = startDate.Day - 1;
                        numberOfMonths = (startDate.Year - endDate.Year) * 12 + startDate.Month - endDate.Month;
                        if (remainingDaysInFirstMonth + elapsedDaysInLastMonth < daysInMonth)
                        {
                            numberOfMonths--;
                        }
                    }

                    CrudTenantDto crudTenantDto = new CrudTenantDto();
                    crudTenantDto.AccessCode = dto["accessCode"].ToString();
                    crudTenantDto.Email = dto["adminEmail"].ToString();
                    crudTenantDto.Type = pageMobi.Type;/// chờ 
                    crudTenantDto.TaxCode = dto["taxCode"].ToString();
                    crudTenantDto.CompanyType = pageMobi.CompanyType;
                    crudTenantDto.CompanyName = dto["customerName"].ToString();
                    crudTenantDto.UsingDecision = int.Parse(dto["usingDecision"].ToString());
                    crudTenantDto.RegNumUser = pageMobi.UserQuantity;
                    crudTenantDto.RegNumMonth =Math.Abs( numberOfMonths);
                    crudTenantDto.RegNumCompany = pageMobi.CompanyQuantity;
                    crudTenantDto.FullName = null;
                    crudTenantDto.PhoneNumber = dto["customerTel"].ToString();
                    crudTenantDto.StartDate = startDate;
                    crudTenantDto.EndDate = endDate;

                    await CreateAsync(crudTenantDto);
                    keyValuePairs.Add("message", "Đang thực hiện yêu cầu vui lòng chờ thực hiện!");
                    keyValuePairs.Add("status", 1);

                }
                else
                {
                    keyValuePairs= await _licenseAppService.CreateLicenseAsync(dto);
                    JsonArray listPackage = (JsonArray)JsonArray.Parse(dto["listPackage"].ToString());
                    lstCustomerRegister.EndDate = DateTime.Parse(listPackage[0]["endDate"].ToString());
                    await _customerRegisterService.UpdateAsync(lstCustomerRegister,true);
                }

               

            }

            return keyValuePairs;
        }
    }
}
