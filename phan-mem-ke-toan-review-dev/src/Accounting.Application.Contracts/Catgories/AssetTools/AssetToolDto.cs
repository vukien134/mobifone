using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.AssetTools
{
    public class AssetToolDto : TenantOrgDto
    {
        public int Year { get; set; }
        public string AssetOrTool { get; set; }

        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
        public string AssetToolGroupId { get; set; }
        //Thẻ tài sản, công cụ
        public string AssetToolCard { get; set; }
        public string UnitCode { get; set; }
        public string Country { get; set; }
        //Năm sản xuất
        public int ProductionYear { get; set; }
        //Công suất
        public string Wattage { get; set; }
        public decimal Quantity { get; set; }
        public string AssetToolAcc { get; set; }
        //Mã mục đích sử dụng
        public string PurposeAcc { get; set; }
        public string DepreciationType { get; set; }
        public string Note { get; set; }
        //Mã giảm
        public string ReduceAcc { get; set; }
        public string ReduceDetail { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ReduceDate { get; set; }
        public string Content { get; set; }
        public string CalculatingMethod { get; set; }
        //Khấu hao theo
        public string FollowDepreciation { get; set; }
        //Nguyên giá
        public decimal OriginalPrice { get; set; }
        public decimal Impoverishment { get; set; }
        public decimal Remaining { get; set; }
        //Giá trị khấu hao
        public decimal DepreciationAmount { get; set; }
        //Giá trị trích khấu hao
        public decimal? DepreciationAmount0 { get; set; }
        // add feld
        public string AssetGroupCode { get; set; }
        public string PurposeCode { get; set; }
        public string DepreciationDebitAcc { get; set; }
        public string DepreciationCreditAcc { get; set; }
        public string PartnerCode { get; set; }
        public string WorkPlaceCode { get; set; }
        public string FProductWorkCode { get; set; }
        public string SectionCode { get; set; }
        public string CaseCode { get; set; }
        public string VoucherNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime VoucherDate { get; set; }
        public string UpDownCode { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime UpDownDate { get; set; }
        public string DepartmentCode { get; set; }
        public string CapitalCode { get; set; }

        public decimal MonthNumber0 { get; set; }
        public string IsCalculating { get; set; }
        public decimal CalculatingAmount { get; set; }
        public decimal MonthNumber { get; set; }
        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime BeginDate { get; set; }

        public string NoteDetail { get; set; }
    }
}
