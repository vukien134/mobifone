using Accounting.Catgories.OrgUnits;
using Accounting.Common;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Users;
using Accounting.EntityFrameworkCore;
using Accounting.Exceptions;
using Accounting.Generals;
using Accounting.Localization;
using Accounting.Migrations.HostDbMigration;
using Accounting.Windows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Uow;
using static Volo.Abp.TenantManagement.TenantManagementPermissions;

namespace Accounting.Licenses
{
    public class LicenseAppService : AccountingAppService, ILicenseAppService
    {
        #region Fields
        private readonly RegLicenseService _regLicenseService;
        private readonly TenantLicenseService _tenantLicenseService;
        private readonly ICurrentTenant _currentTenant;
        private readonly IConfiguration _config;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ITenantRepository _tenantRepository;
        private readonly PackgeMobiService _packgeMobiService;
        private readonly RegLicenseInfoService _regLicenseInfoService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        #endregion
        #region Ctor
        public LicenseAppService(RegLicenseService regLicenseService,
                                TenantLicenseService tenantLicenseService,
                                ICurrentTenant currentTenant,
                                IConfiguration config,
                                IStringLocalizer<AccountingResource> localizer,
                                OrgUnitService orgUnitService,
                                TenantExtendInfoService tenantExtendInfoService,
                                ITenantRepository tenantRepository,
                                PackgeMobiService packgeMobiService,
                                RegLicenseInfoService regLicenseInfoService,
                                IUnitOfWorkManager unitOfWorkManager
                            )
        {
            _regLicenseService = regLicenseService;
            _tenantLicenseService = tenantLicenseService;
            _currentTenant = currentTenant;
            _config = config;
            _localizer = localizer;
            _orgUnitService = orgUnitService;
            _tenantExtendInfoService = tenantExtendInfoService;
            _tenantRepository = tenantRepository;
            _packgeMobiService = packgeMobiService;
            _regLicenseInfoService = regLicenseInfoService;
            _unitOfWorkManager = unitOfWorkManager;
        }
        #endregion
        #region Methods
        public async Task Register(CrudRegLicenseDto dto)
        {
            var regLicense = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            if (regLicense == null)
            {
                await CreateRegLicenseAsync(dto);
                return;
            }

            if (regLicense.IsApproval == true)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.IsApproval),
                        $"TenantId = ['{_currentTenant.Id}'] already register ");
            }
            dto.Id = regLicense.Id;
            ObjectMapper.Map(dto, regLicense);
            await _regLicenseService.UpdateAsync(regLicense);
        }
        public async Task<ResLicenseDto> GetRegisterByTenantIdAsync()
        {
            var regDto = await this.GetInfoRegLicense();
            regDto.LicXml = null;
            //convert DateTime regDate  to dd/MM/yyyy
            ResLicenseDto entity = new ResLicenseDto();
            entity.TaxCode = regDto.TaxCode;
            entity.Name = regDto.Name;
            entity.TypeLic = regDto.TypeLic;
            entity.Month = regDto.Month;
            entity.CompanyQuantity = regDto.CompanyQuantity;
            entity.LicXml = regDto.LicXml;
            entity.CustomerTenantId = regDto.CustomerTenantId;
            entity.IsApproval = regDto.IsApproval;
            entity.Key = regDto.Key;
            entity.UserQuantity = regDto.UserQuantity;
            entity.CompanyType = regDto.CompanyType;
            entity.Id = regDto.Id;
            entity.ApprovalDate = regDto.ApprovalDate;
            entity.CreatorId = regDto.CreatorId;
            entity.CreationTime = regDto.CreationTime;
            entity.LastModificationTime = regDto.LastModificationTime;
            entity.LastModifierId = regDto.LastModifierId;
            
            if (regDto.RegDate.HasValue)
            {
                // Chuyển đổi đối tượng DateTime? thành chuỗi
                string formattedDate = regDto.RegDate.Value.ToString("dd/MM/yyyy");
                entity.RegDate = formattedDate;
            }
            if (regDto.EndDate.HasValue)
            {
                // Chuyển đổi đối tượng DateTime? thành chuỗi
                string formattedDate = regDto.EndDate.Value.ToString("dd/MM/yyyy");
                entity.EndDate = formattedDate;
            }
            if (entity != null) return entity;

            var orgUnit = await _orgUnitService.GetOrgUnitForRegistering();
            var tenantExtendInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);

            regDto = new RegLicenseDto()
            {
                Name = orgUnit.Name,
                TaxCode = orgUnit.TaxCode,
                TypeLic = tenantExtendInfo.TenantType
            };

            return entity;
        }
        public async Task<RegLicenseDto> ApprovalAsync(Guid tenantId)
        {
            var regLicense = await _regLicenseService.GetByTenantId(tenantId);
            if (regLicense == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.NotFoundEntity),
                        $"RegLicense With TenantId=['{tenantId}'] not found ");
            };
            if (regLicense.IsApproval == true)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.IsApproval),
                        $"RegLicense With TenantId=['{tenantId}'] already approval ");
            }

            var doc = BuildXmlRegLicense(regLicense);
            string xml = SignXml(doc);
            string key = doc.GetElementsByTagName("Key")[0].InnerText;
            var approvalDate = Convert.ToDateTime(doc.GetElementsByTagName("ApprovalDate")[0].InnerText);
            regLicense.ApprovalDate = approvalDate;
            regLicense.LicXml = xml;
            regLicense.IsApproval = true;
            regLicense.Key = key;
            await _regLicenseService.UpdateAsync(regLicense);
            var dto = ObjectMapper.Map<RegLicense, RegLicenseDto>(regLicense);
            return dto;
        }
        public async Task<Dictionary<string, string>> GetInfoLicenseAsync()
        {
            var dic = new Dictionary<string, string>();
            var regDto = await this.GetInfoRegLicense();
            if (regDto == null)
            {
                dic.Add("data", _localizer["UsingDemo"] + $" hạn dùng: {regDto.EndDate:dd/MM/yyyy}");
                return dic;
            }
            if (string.IsNullOrEmpty(regDto.LicXml))
            {
                dic.Add("data", _localizer["UsingDemo"] + $" hạn dùng: {regDto.EndDate:dd/MM/yyyy}");
                return dic;
            }

            var date = regDto.EndDate;
            dic.Add("data", _localizer["DueDate"] + ": " + $"{date:dd/MM/yyyy HH:mm:ss}");
            return dic;
        }
        #endregion
        #region Private
        private async Task CreateRegLicenseAsync(CrudRegLicenseDto dto)
        {
            var orgUnits = await _orgUnitService.GetAllAsync();
            var groupUnits = orgUnits.GroupBy(g => new { g.TaxCode, g.Name })
                                .Select(p => new OrgUnitDto()
                                {
                                    TaxCode = p.Key.TaxCode,
                                    Name = p.Key.Name
                                }).ToList();
            if (dto.CompanyQuantity < groupUnits.Count)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.Other),
                        _localizer["Err:CompanyRegLessThanCreating"]);
            }
            if (dto.CompanyQuantity == 1 && groupUnits.Count == 1
                && (dto.TaxCode != groupUnits[0].TaxCode || dto.Name != groupUnits[0].Name))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.Other),
                        _localizer["Err:TaxCodeOrNameNotSameInOrgUnit"]);
            }
            dto.Id = this.GetNewObjectId();
            dto.CustomerTenantId = _currentTenant.Id;
            dto.RegDate = DateTime.Now;
            var regLic = ObjectMapper.Map<CrudRegLicenseDto, RegLicense>(dto);
            await _regLicenseService.CreateAsync(regLic);
        }


        private XmlDocument BuildXmlRegLicense(RegLicense regLicense)
        {
            var doc = new XmlDocument();

            var root = doc.CreateElement("License");
            doc.AppendChild(root);

            var elInfoCompany = doc.CreateElement("InfoCompany");
            elInfoCompany.SetAttribute("id", "data");
            root.AppendChild(elInfoCompany);

            var element = doc.CreateElement("LicenseInfoId");
            element.InnerText = regLicense.Id;
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("TaxCode");
            element.InnerText = regLicense.TaxCode;
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("Name");
            element.InnerText = regLicense.Name;
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("TypeLicense");
            element.InnerText = regLicense.TypeLic.ToString();
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("Month");
            element.InnerText = regLicense.Month.ToString();
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("CompanyQuantity");
            element.InnerText = regLicense.CompanyQuantity.ToString();
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("RegDate");
            element.InnerText = $"{regLicense.RegDate:yyyy-MM-ddTHH:mm:ss}";
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("ApprovalDate");
            element.InnerText = $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss}";
            elInfoCompany.AppendChild(element);
            element = doc.CreateElement("Key");
            element.InnerText = this.GetNewObjectId();
            element = doc.CreateElement("CompanyType");
            element.InnerText = regLicense.CompanyType;
            element = doc.CreateElement("UserQuantity");
            element.InnerText = regLicense.UserQuantity.ToString();
            elInfoCompany.AppendChild(element);

            var elSignatures = doc.CreateElement("Signatures");
            root.AppendChild(elSignatures);

            return doc;
        }
        private string SignXml(XmlDocument doc)
        {
            string certPath = _config.GetValue<string>("LicenseCert:Path");
            string password = _config.GetValue<string>("LicenseCert:Password");
            string xml = doc.OuterXml;
            xml = Util.SignXml(certPath, password, xml, "Seller", "#data", "Signatures");
            return xml;
        }
        private async Task CreateTenantLicenseAsync(RegLicense reg)
        {
            var dto = new CrudTenantLicenseDto();
            dto.Id = reg.Id;
            dto.LicXml = reg.LicXml;
            dto.Key = reg.Key;
            var entity = ObjectMapper.Map<CrudTenantLicenseDto, TenantLicense>(dto);
            await _tenantLicenseService.CreateAsync(entity);
        }
        private async Task<RegLicenseDto> GetInfoRegLicense()
        {
            var tenantLic = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            if (tenantLic == null) return null;

            return ObjectMapper.Map<RegLicense, RegLicenseDto>(tenantLic);
        }
        [UnitOfWork(isTransactional: false)]
        [AllowAnonymous]
        public async Task<RegLicenseDto> UpdateAsync(string AccessCode, CrudRegLicenseDto dto)
        {
            var tenant = await _tenantRepository.FindByNameAsync(AccessCode);
            if (tenant == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.CustomerRegister, ErrorCode.Other),
                     $"AccessCode " + AccessCode + " Type form ");
            }
            var regLicense = await _regLicenseService.GetByTenantId(tenant.Id);
            regLicense.Month = dto.Month;
            regLicense.UserQuantity = dto.UserQuantity;
            regLicense.CompanyQuantity = dto.CompanyQuantity;
            regLicense.Name = dto.Name;
            await _regLicenseService.UpdateAsync(regLicense);
            var dtos = ObjectMapper.Map<RegLicense, RegLicenseDto>(regLicense);
            return dtos;
        }
        [AllowAnonymous]
        public async Task<JsonObject> CreateLicenseAsync(JsonObject dto)
        {
            var AccessCode = dto["AccessCode"].ToString();
           var accessCode = AccessCode.ToLower();
            var tenant = await _tenantRepository.FindByNameAsync(accessCode);
            JsonArray listPackage = (JsonArray)JsonArray.Parse(dto["listPackage"].ToString());
            var regLicense = await _regLicenseService.GetByTenantId(tenant.Id);
            var lstpackeMobi = await _packgeMobiService.GetQueryableAsync();
            var code = listPackage[0]["packageCode"].ToString();
            var pageMobi = lstpackeMobi.Where(p => p.Code == code).FirstOrDefault();
            DateTime endDate = DateTime.Parse(listPackage[0]["endDate"].ToString());
            DateTime startDate = DateTime.Parse(listPackage[0]["startDate"].ToString());
            JsonObject keyValuePairs= new JsonObject(); 
            if (regLicense.CompanyType == "Synthetic")
            {
                if (listPackage[0]["packageCode"].ToString().Contains("UPTO"))
                {
                    keyValuePairs.Add("status", "2");
                    keyValuePairs.Add("message", "Phê duyệt thất bại. Vui lòng kiểm tra lại thông tin gói cước!");
                    keyValuePairs.Add("data", null);
                   return keyValuePairs;
                }
            }
          
            if (code.Substring(0,3).Contains("DN_") || code.Contains("HKD_1MST_KGH_USER") || code.Contains("KT_DV_5MST_5USER"))
            {
                keyValuePairs.Add("status", "2");
                keyValuePairs.Add("message", "Phê duyệt thất bại. Vui lòng kiểm tra lại thông tin gói cước!");
                keyValuePairs.Add("data", null);
                return keyValuePairs;
            }

            if (listPackage[0]["packageCode"].ToString().Contains("UPTO"))
            {

                regLicense.CompanyType = pageMobi.CompanyType;
                regLicense.EndDate = DateTime.Parse(endDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)); CrudReglicenseInfoDto crudRegLicenseDto = new CrudReglicenseInfoDto();


                var uow = _unitOfWorkManager.Begin(isTransactional: false);
                using (_currentTenant.Change(tenant.Id))
                {
                    await CreateRegLicenseInfo(pageMobi, dto, regLicense, tenant);

                    regLicense.UserQuantity += pageMobi.UserQuantity;
                    regLicense.CompanyQuantity += pageMobi.CompanyQuantity;

                }

                await _regLicenseService.UpdateAsync(regLicense, true);
                keyValuePairs.Add("status", "1");
                keyValuePairs.Add("message", "Thành công!");
                keyValuePairs.Add("data", null);
                return keyValuePairs;
            }
            else
            {
                var check = CheckLicense(regLicense.CompanyType, dto);
                if (check.Result == false)
                {
                    keyValuePairs.Add("status", "2");
                    keyValuePairs.Add("message", "Phê duyệt thất bại. Vui lòng kiểm tra lại thông tin gói cước!");
                    keyValuePairs.Add("data", null);
                    return keyValuePairs;
                }
                var uow = _unitOfWorkManager.Begin(isTransactional: false);
                using (_currentTenant.Change(tenant.Id))
                {
                    await CreateRegLicenseInfo(pageMobi, dto, regLicense, tenant);

                    regLicense.UserQuantity += pageMobi.UserQuantity;
                    regLicense.CompanyQuantity += pageMobi.CompanyQuantity;

                }
                regLicense.EndDate =DateTime.Parse( listPackage[0]["endDate"].ToString());
                await _regLicenseService.UpdateAsync(regLicense, true);

                //await CreateRegLicenseInfo(pageMobi, dto, regLicense, tenant);
                await _regLicenseService.UpdateAsync(regLicense, true);
                keyValuePairs.Add("status", "1");
                keyValuePairs.Add("message", "Thành công!");
                keyValuePairs.Add("data", null);
                return keyValuePairs;
            }


         
            return keyValuePairs;
        }
        private async Task<bool> CheckLicense(string companyType, JsonObject dto)
        {
            JsonArray listPackage = (JsonArray)JsonArray.Parse(dto["listPackage"].ToString());
            var lstpackeMobi = await _packgeMobiService.GetQueryableAsync();
            var code = listPackage[0]["packageCode"].ToString();
            var pageMobi = lstpackeMobi.Where(p => p.Code == code).FirstOrDefault();
            if (companyType != pageMobi.CompanyType)
            {
                return false;
            }

            return true;
        }
        private async Task CreateRegLicenseInfo(PackageMobi pageMobi, JsonObject dto, RegLicense regLicense, Tenant tenant)
        {
            JsonArray listPackage = (JsonArray)JsonArray.Parse(dto["listPackage"].ToString());
            DateTime endDate = DateTime.Parse(listPackage[0]["endDate"].ToString());
            DateTime startDate = DateTime.Parse(listPackage[0]["startDate"].ToString());
            CrudReglicenseInfoDto crudRegLicenseDto = new CrudReglicenseInfoDto();
            crudRegLicenseDto.Code = listPackage[0]["packageCode"].ToString();
            crudRegLicenseDto.UserQuantity = pageMobi.UserQuantity;
            crudRegLicenseDto.CompanyQuantity = pageMobi.CompanyQuantity;
            crudRegLicenseDto.CustomerTenantId = tenant.Id;
            crudRegLicenseDto.ApprovalDate = DateTime.Parse(startDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            crudRegLicenseDto.EndDate = DateTime.Parse(endDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            crudRegLicenseDto.TaxCode = dto["taxCode"].ToString();
            crudRegLicenseDto.CompanyType = pageMobi.CompanyType;
            crudRegLicenseDto.RegDate = DateTime.Parse(startDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            crudRegLicenseDto.Id = this.GetNewObjectId();
            crudRegLicenseDto.Name = dto["customerName"].ToString();
            crudRegLicenseDto.Key = regLicense.Key;
            crudRegLicenseDto.TypeLic = regLicense.TypeLic;
            DateTime ngaymuon = Convert.ToDateTime(listPackage[0]["startDate"].ToString());
            DateTime ngaytra = Convert.ToDateTime(listPackage[0]["endDate"].ToString());
            TimeSpan Time = ngaytra - ngaymuon;
            int numberOfMonths;

            if (Time.TotalDays < 30)
            {
                numberOfMonths = 1;
            }
            else
            {
                int daysInMonth = DateTime.DaysInMonth(ngaymuon.Year, ngaymuon.Month);
                int remainingDaysInFirstMonth = daysInMonth - ngaymuon.Day + 1;
                int daysInLastMonth = DateTime.DaysInMonth(ngaytra.Year, ngaytra.Month);
                int elapsedDaysInLastMonth = ngaytra.Day - 1;
                numberOfMonths = (ngaytra.Year - ngaymuon.Year) * 12 + ngaytra.Month - ngaymuon.Month;
                if (remainingDaysInFirstMonth + elapsedDaysInLastMonth < daysInMonth)
                {
                    numberOfMonths--;
                }
            }

            crudRegLicenseDto.Month = numberOfMonths;
            var uow = _unitOfWorkManager.Begin(isTransactional: false);
            using (_currentTenant.Change(tenant.Id))
            {
                var regLicenseInfo = ObjectMapper.Map<CrudReglicenseInfoDto, RegLicenseInfo>(crudRegLicenseDto);

                await _regLicenseInfoService.CreateAsync(regLicenseInfo, true);


                //regLicense.UserQuantity += pageMobi.UserQuantity ?? 0;
                //regLicense.CompanyQuantity += pageMobi.CompanyQuantity ?? 0;
                //regLicense.RegDate = DateTime.Parse(startDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
                //regLicense.EndDate = DateTime.Parse(endDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
                //regLicense.ApprovalDate = DateTime.Parse(endDate.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            }



            await _unitOfWorkManager.Current.RollbackAsync();
        }
        #endregion
    }
}
