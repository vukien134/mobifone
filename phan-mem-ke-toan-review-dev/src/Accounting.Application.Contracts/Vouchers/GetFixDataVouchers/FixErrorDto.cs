using Accounting.BaseDtos;
using Accounting.JsonConverters;
using System;

namespace Accounting.Vouchers.GetFixDataVouchers
{
    public class FixErrorDto
    {
        public string errorId { get; set; }
        public string errorName { get; set; }
        public int tag { get; set; }
        public string keyError { get; set; }
        public int classify { get; set; }
        public bool? selectRow { get; set; }
    }
}
