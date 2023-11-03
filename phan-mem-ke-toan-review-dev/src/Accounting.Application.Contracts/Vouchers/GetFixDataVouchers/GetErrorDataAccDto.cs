using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.GetFixDataVouchers
{
    public class GetErrorDataAccDto
    {
        public string errorId { get; set; }
        public string errorName { get; set; }
        public string keyError { get; set; }
        public string id { get; set; }
        public int year { get; set; }
        public string accCode { get; set; }
        public string currencyCode { get; set; }
        public string partnerCode { get; set; }
        public string contractCode { get; set; }
        public string fProductWorkCode { get; set; }
        //Mã khoản mục
        public string accSectionCode { get; set; }
        //Mã phân xưởng
        public string workPlaceCode { get; set; }
        //Dư nợ
        public decimal debit { get; set; }
        //Dư nợ ngoại tệ
        public decimal debitCur { get; set; }
        //Dư có
        public decimal credit { get; set; }
        //Dư có ngoại tệ
        public decimal creditCur { get; set; }
        //Dư nợ lũy kế
        public decimal debitCum { get; set; }
        //Dư nợ lũy kế ngoại tệ
        public decimal debitCumCur { get; set; }
        //Dư có lũy kế
        public decimal creditCum { get; set; }
        //Dư có lũy kế ngoại tệ
        public decimal creditCumCur { get; set; }
    }
}
