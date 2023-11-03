using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Caching
{
    public static class CacheKeyManager
    {
        public const int Hour = 1;

        public const string ListByCode = "Code_{0}";
        public const string ListCombo = "Combo";
        public const string ListDefault = "Default";
        public const string ListByOrgCode = "OrgCode_{0}";
        public const string ListByUserId = "UserId_{0}";
        public const string YearCurrentUnit = "Year_{0}";
        public const string YearCategoryByYear = "OrgCode_{0}_Year_{1}";
        public const string ListCurrency = "OrgCode_{0}";
        public const string CodeWithOrgCode = "OrgCode_{0}_Code_{1}";
        public const string ListDefaultCurrency = "Default";
        public const string ListDefaultAssetToolGroup = "Default_{0}";
        public const string ListAssetToolGroup = "OrgCode_{0}_Type_{1}";
        public const string MenuByUser = "t:{0},c:MenuByUser,k:{1}_{2}";
        public const string MenuByRole = "t:{0},c:MenuByRole,k:{1}";
        public const string AccountSystemByYear = "OrgCode_{0}_Year_{1}";
        public const string DefaultAccountSystemByUsingDecision = "UsingDecision_{0}";
        public const string ListBusinessCategoryByVoucherCode = "OrgCode_{0}_VoucherCode_{1}";
        public const string DashboardByUser = "t:{0},c:DashboardByUser,k:{1}";
        public const string ListTaxCategory = "OrgCode_{0}_OutOrIn_{1}";
    }
}
