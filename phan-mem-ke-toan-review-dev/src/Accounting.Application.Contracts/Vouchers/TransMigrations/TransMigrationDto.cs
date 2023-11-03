using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.TransMigrations
{
    public class TransMigrationDto : CruOrgBaseDto
    {
        public string FieldCode { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int IsCheck { get; set; }
    }
}
