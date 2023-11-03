using Accounting.BaseDtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Accounting.Catgories.Accounts
{
    public class CruAccountSystemDto : CruOrgBaseDto
    {
        public int Year { get; set; }

        [Required]
        [MinLength(3)]
        [JsonPropertyName("code")]
        [DisplayName("code")]
        public string AccCode { get; set; }
        //Giá trị : 1 : Dư nợ, 2: Dư có, 3: Lưỡng Tính, 4: Không dư
        public int AccPattern { get; set; }
        public string AssetOrEquity { get; set; }

        [Required]
        [JsonPropertyName("name")]
        [DisplayName("name")]
        public string AccName { get; set; }
        [JsonPropertyName("nameE")]
        public string AccNameEn { get; set; }
        //Bac TK
        [JsonIgnore]
        public int AccRank { get; set; }
        //LOAI_TK : Giá trị C/K
        public string AccType { get; set; }

        [JsonIgnore]
        public string ParentCode { get; set; }

        [JsonIgnore]
        public string AccNameTemp { get; set; }
        [JsonIgnore]
        public string AccNameTempE { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string Province { get; set; }
        //Theo đối tượng, đối tác
        public string AttachPartner { get; set; }
        //Theo hợp đồng
        public string AttachContract { get; set; }
        //Theo Khoản mục
        public string AttachAccSection { get; set; }
        //Mã khoản mục
        public string AccSectionCode { get; set; }
        //Theo ngoại tệ
        public string AttachCurrency { get; set; }
        //Theo phân xưởng
        public string AttachWorkPlace { get; set; }
        //Theo giá thành, chi phí sản phẩm
        public string AttachProductCost { get; set; }
        //Là tài khoản ngoài bảng
        public string IsBalanceSheetAcc { get; set; }
        //Theo chứng từ
        public string AttachVoucher { get; set; }

        [JsonPropertyName("parentId")]
        public string ParentAccId { get; set; }
    }
}
