using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class ExcelProductDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        //Quy cách sản phẩm
        public string Specification { get; set; }
        public string Barcode { get; set; }
        public string UnitCode { get; set; }
        //Theo lô
        public string AttachProductLot { get; set; }
        //Theo nguồn
        public string AttachProductOrigin { get; set; }
        //Theo phân xưởng
        public string AttachWorkPlace { get; set; }
        public string ProductType { get; set; }
        //Mã thuế tiêu thụ đặc biệt
        public string ExciseTaxCode { get; set; }
        //Tài khoản hàng hóa, vật tư
        public string ProductAcc { get; set; }
        //Tài khoản giá vốn
        public string ProductCostAcc { get; set; }
        public string RevenueAcc { get; set; }
        public string DiscountAcc { get; set; }
        //Tài khoản hàng bán bị trả lại
        public string SaleReturnsAcc { get; set; }
        //Mã công trình sản phẩm
        public string FProductWorkCode { get; set; }
        //Mã giai đoạn sản xuất
        public string ProductionPeriodCode { get; set; }
        public string ProductGroupCode { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal MaxQuantity { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal ExciseTaxPercentage { get; set; }
        public decimal ImportTaxPercentage { get; set; }
        public string Note { get; set; }
        //Mã danh mục thuế
        public string TaxCategoryCode { get; set; }
        //Mã ngành nghề
        public string CareerCode { get; set; }
        //% thuế thu nhập cá nhân
        public decimal PITPercentage { get; set; }
        public string ProductGroupId { get; set; }

        //Chi tiết đơn vị tính
        public decimal ExchangeRate { get; set; }
        //Giá mua , giá nhập
        public decimal PurchasePrice { get; set; }
        //Giá bán
        public decimal SalePrice { get; set; }
        public decimal PitPercentage { get; set; }

    }
}
