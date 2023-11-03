using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.Printings
{
    public interface IPrintingVoucherAppService
    {
        //Task<FileContentResult> CreateAsync(PrintingVoucherRequestDto dto);
        Task<object> GetDataAsync(string voucherCode, string voucherId);
    }
}
