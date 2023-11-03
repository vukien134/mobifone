using Accounting.TenantEntities;
using System;
using System.Collections.Generic;

namespace Accounting.Categories.AssetTools
{
    public class AssetTool : TenantOrgEntity
    {
        public int Year { get; set; }
        public string AssetOrTool { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string AssetToolGroupId { get; set; }
        //Thẻ tài sản, công cụ
        public string AssetToolCard { get; set; }
        public string UnitCode { get; set; }
        public string Country { get; set; }
        //Năm sản xuất
        public int? ProductionYear { get; set; }
        //Công suất
        public string Wattage { get; set; }
        public decimal? Quantity { get; set; }
        public string AssetToolAcc { get; set; }
        //Mã mục đích sử dụng
        public string PurposeCode { get; set; }
        public string DepreciationType { get; set; }
        public string Note { get; set; }
        //Mã giảm
        public string UpDownCode { get; set; }
        public string ReduceDetail { get; set; }
        public DateTime? ReduceDate { get; set; }
        public string Content { get; set; }
        public string CalculatingMethod { get; set; }
        //Khấu hao theo
        public string FollowDepreciation { get; set; }
        //Nguyên giá
        public decimal? OriginalPrice { get; set; }
        public decimal? Impoverishment { get; set; }
        public decimal? Remaining { get; set; }
        //Giá trị khấu hao
        public decimal? DepreciationAmount { get; set; }
        //Giá trị trích khấu hao
        public decimal? DepreciationAmount0 { get; set; }

        public ICollection<AssetToolDetail> AssetToolDetails { get; set; }
        public ICollection<AssetToolAccessory> AssetToolAccessories { get; set; }
        public ICollection<AssetToolStoppingDepreciation> AssetToolStoppingDepreciations { get; set; }
        public ICollection<AssetToolAccount> AssetToolAccounts { get; set; }
        public ICollection<AssetToolDepreciation> AssetToolDepreciations { get; set; }
    }
}
