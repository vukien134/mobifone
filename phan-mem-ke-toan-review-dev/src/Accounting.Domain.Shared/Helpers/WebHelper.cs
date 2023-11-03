using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace Accounting.Helpers
{
    public class WebHelper : ITransientDependency
    {
        #region Fields

        private const string OrgUnitCookieName = "__orgcode";
        private const string OrgUnitHeaderKeyName = "x-orgcode";
        private const string YearCookieName = "__year";
        private const string YearHeaderKeyName = "x-year";
        private const string MenuIdName = "x-menu";
        private const string TenantName = "__tenant";

        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion
        #region Ctor
        public WebHelper(IHttpContextAccessor httpContextAccessor
                    )
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion
        #region Methods
        public string GetCurrentOrgUnit()
        {
            var value = _httpContextAccessor.HttpContext.Request.Cookies
                                .Where(c => c.Key == OrgUnitCookieName)
                                .Select(c => c.Value)
                                .FirstOrDefault();
            if (value != null) return value;

            value = _httpContextAccessor.HttpContext.Request.Headers
                        .Where(p => p.Key == OrgUnitHeaderKeyName)
                        .Select(p => p.Value)
                        .FirstOrDefault();


            return value;
        }
        public int GetCurrentYear()
        {
            var value = _httpContextAccessor.HttpContext.Request.Cookies
                                .Where(c => c.Key == YearCookieName)
                                .Select(c => c.Value)
                                .FirstOrDefault();
            if (value != null) return Convert.ToInt32(value);

            value = _httpContextAccessor.HttpContext.Request.Headers
                        .Where(p => p.Key == YearHeaderKeyName)
                        .Select(p => p.Value)
                        .FirstOrDefault();

            return Convert.ToInt32(value);
        }
        public string GetMenuId()
        {
            string value = _httpContextAccessor.HttpContext.Request.Headers
                        .Where(p => p.Key == MenuIdName)
                        .Select(p => p.Value)
                        .FirstOrDefault();


            return value;
        }
        public string GetTenantId()
        {
            string value = _httpContextAccessor.HttpContext.Request.Headers
                        .Where(p => p.Key == TenantName)
                        .Select(p => p.Value)
                        .FirstOrDefault();


            return value;
        }
        public string GetUrlApi()
        {
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;            
            string scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            return $"{scheme}://{host}/api";
        }
        public string GetBearerToken()
        {
            string authorization = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            return authorization;
        }
        public string GetCurrentUrl()
        {
            string url = _httpContextAccessor.HttpContext.Request.Headers["Host"];
            return url;
        }
        public string GetCurrentLanguage()
        {
            string lang = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];
            return lang;
        }
        #endregion
    }
}
