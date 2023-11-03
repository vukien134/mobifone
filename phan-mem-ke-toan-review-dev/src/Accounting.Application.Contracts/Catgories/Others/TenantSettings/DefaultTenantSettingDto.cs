using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Others.TenantSettings
{
    public class DefaultTenantSettingDto : BaseDto
    {
        public int? Ord { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SettingType { get; set; }
    }
}
