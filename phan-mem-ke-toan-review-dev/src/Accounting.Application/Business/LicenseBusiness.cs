using Accounting.Catgories.OrgUnits;
using Accounting.Common;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Licenses;
using Accounting.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Accounting.Business
{
    public class LicenseBusiness : ITransientDependency
    {
        #region Fields
        private readonly TenantLicenseService _tenantLicenseService;
        private readonly ICurrentTenant _currentTenant;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly CustomerRegisterService _customerRegisterService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly OrgUnitService _orgUnitService;
        private readonly RegLicenseService _regLicenseService;
        private readonly IObjectMapper _objectMapper;
        private readonly IConfiguration _config;
        private readonly WebHelper _webHelper;
        private readonly UserService _userService;
        private readonly IServiceProvider _serviceProvider;
        #endregion
        #region Ctor
        public LicenseBusiness(TenantLicenseService tenantLicenseService,
                        ICurrentTenant currentTenant,
                        AccVoucherService accVoucherService,
                        ProductVoucherService productVoucherService,
                        CustomerRegisterService customerRegisterService,
                        IStringLocalizer<AccountingResource> localizer,
                        OrgUnitService orgUnitService,
                        RegLicenseService regLicenseService,
                        IObjectMapper objectMapper,
                        IConfiguration config,
                        WebHelper webHelper,
                        UserService userService,
                        IServiceProvider serviceProvider
                    )
        {
            _tenantLicenseService = tenantLicenseService;
            _currentTenant = currentTenant;
            _accVoucherService = accVoucherService;
            _productVoucherService = productVoucherService;
            _customerRegisterService = customerRegisterService;
            _localizer = localizer;
            _orgUnitService = orgUnitService;
            _regLicenseService = regLicenseService;
            _objectMapper = objectMapper;
            _config = config;
            _webHelper = webHelper;
            _userService = userService;
            _serviceProvider = serviceProvider;
        }
        #endregion
        #region Methods
        public async Task ValidRegUserQuantity()
        {
            var regLicense = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            if (regLicense == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.Other),
                        _localizer["Err:LicNotRegisted"]);
            }
            int currentUserQuantity = await _userService.Count();
            if (regLicense.UserQuantity < currentUserQuantity)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.Other),
                        _localizer["Err:LicUserLimited",regLicense.UserQuantity]);
            }
        }
        public async Task ValidLicAsync()
        {
            bool isRegistered = await _regLicenseService.IsRegisted(_currentTenant.Id);
            if (!isRegistered)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.Other),
                        _localizer["Err:LicNotRegisted"]);
            }
            var dto = await this.GetInfoRegLicense();
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
        }
        public async Task CheckExpired()
        {
            string tenantName = null;
            if (_currentTenant.Name == null)
            {
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    var tenantStore = serviceScope
                        .ServiceProvider
                        .GetRequiredService<ITenantStore>();

                    var tenant = await tenantStore.FindAsync(_currentTenant.Id.Value);
                    tenantName = tenant.Name;
                }
            }
            else {
                tenantName = _currentTenant.Name;
            }
            var customerRegister = await _customerRegisterService.GetByAccessCode(tenantName);
            var dateNow = DateTime.Now;
            TimeSpan time = (customerRegister.EndDate ?? DateTime.Now) - dateNow;
            if (time.Days < 0)
            {
                throw new Exception("Bạn không có quyền thực hiện. Vui lòng liên hệ quản trị hệ thống để được hỗ trợ!");
            }
        }
        public async Task<bool> IsDemo()
        {
            var tenantLic = await _tenantLicenseService.GetByTenantIdAsync(_currentTenant.Id);
            if (tenantLic == null) return true;
            return false;
        }
        public XmlDocument BuildXmlRegLicense(RegLicense regLicense)
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
            element.InnerText = ObjectHelper.NewId();
            element = doc.CreateElement("CompanyType");
            element.InnerText = regLicense.CompanyType;
            element = doc.CreateElement("UserQuantity");
            element.InnerText = regLicense.UserQuantity.ToString();
            elInfoCompany.AppendChild(element);

            var elSignatures = doc.CreateElement("Signatures");
            root.AppendChild(elSignatures);

            return doc;
        }
        public string SignXml(XmlDocument doc)
        {
            string certPath = _config.GetValue<string>("LicenseCert:Path");
            string password = _config.GetValue<string>("LicenseCert:Password");
            string xml = doc.OuterXml;
            xml = Util.SignXml(certPath, password, xml, "Seller", "#data", "Signatures");
            return xml;
        }
        #endregion
        #region privates        
        public async Task<RegLicenseDto> GetInfoRegLicense()
        {
            var tenantLic = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            if (tenantLic == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.RegLicense, ErrorCode.NotRegisterLic),
                        _localizer["Err:NotRegisterLic"]);
            };
            if (string.IsNullOrEmpty(tenantLic.LicXml))
            {
                return _objectMapper.Map<RegLicense, RegLicenseDto>(tenantLic);
            }
            var doc = new XmlDocument();
            doc.LoadXml(tenantLic.LicXml);

            var regLicDto = new RegLicenseDto();
            regLicDto.Id = doc.GetElementsByTagName("LicenseInfoId")[0].InnerText;
            regLicDto.ApprovalDate = DateTime.Parse(doc.GetElementsByTagName("ApprovalDate")[0].InnerText);
            regLicDto.CompanyQuantity = Convert.ToInt32(doc.GetElementsByTagName("CompanyQuantity")[0].InnerText);
            regLicDto.Key = doc.GetElementsByTagName("Key")[0].InnerText;
            regLicDto.Month = Convert.ToInt32(doc.GetElementsByTagName("Month")[0].InnerText);
            regLicDto.Name = doc.GetElementsByTagName("Name")[0].InnerText;
            regLicDto.RegDate = DateTime.Parse(doc.GetElementsByTagName("RegDate")[0].InnerText);
            regLicDto.TaxCode = doc.GetElementsByTagName("TaxCode")[0].InnerText;
            regLicDto.TypeLic = Convert.ToInt32(doc.GetElementsByTagName("TypeLicense")[0].InnerText);
            regLicDto.LicXml = tenantLic.LicXml;
            return regLicDto;
        }
        #endregion
    }
}
