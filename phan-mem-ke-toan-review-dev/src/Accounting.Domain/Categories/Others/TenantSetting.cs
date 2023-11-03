using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class TenantSetting : TenantOrgEntity
    {
        public int? Ord { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SettingType { get; set; }
    }
}
