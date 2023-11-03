using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    public class InventoryRecord : TenantOrgEntity
    {
        public string VoucherCode { get; set; }
        public int? Year { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime? VoucherDate { get; set; }
        //NGAY_DC
        public DateTime? TransDate { get; set; }
        //DAI_DIEN1
        public string Representative1 { get; set; }
        public string Position1 { get; set; }
        //ONG_BA1
        public string OtherContact1 { get; set; }
        //DAI_DIEN2
        public string Representative2 { get; set; }
        public string Position2 { get; set; }
        //ONG_BA2
        public string OtherContact2 { get; set; }
        //DAI_DIEN2
        public string Representative3 { get; set; }
        public string Position3 { get; set; }
        //ONG_BA2
        public string OtherContact3 { get; set; }
        public string Description { get; set; }
        //T_TIEN_KT
        public decimal? TotalAuditAmount { get; set; }
        //T_TIEN_KK
        public decimal? TotalInventoryAmount { get; set; }
        //T_TIEN_THUA
        public decimal? TotalOverAmount { get; set; }
        //T_TIEN_THIEU
        public decimal? TotalShortAmount { get; set; }
        public ICollection<InventoryRecordDetail> InventoryRecordDetails { get; set; }
    }
}
