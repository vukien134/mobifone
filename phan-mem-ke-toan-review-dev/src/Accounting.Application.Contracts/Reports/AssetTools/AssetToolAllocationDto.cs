using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports.Cores
{
    public class AssetToolAllocationDto
    {
        public string Bold { get; set; }
        public int Sort { get; set; }
        public string Id { get; set; }
        public string AssetToolId { get; set; }
        public string OrgCode { get; set; }
        public string DepartmentCode { get; set; }
        public string AssetToolCode { get; set; }
        public string AssetToolName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DebitAcc { get; set; }
        public string CreditAcc { get; set; }
        public string DebitAccName { get; set; }
        public string CreditAccName { get; set; }
        public decimal? ImpoverishmentPrice { get; set; }
    }
}
