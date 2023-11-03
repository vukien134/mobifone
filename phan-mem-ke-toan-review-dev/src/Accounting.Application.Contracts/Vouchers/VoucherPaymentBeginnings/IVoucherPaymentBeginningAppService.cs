using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.VoucherPaymentBeginnings
{
    public interface IVoucherPaymentBeginningAppService
    {
        Task<PageResultDto<VoucherPaymentBeginningDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<VoucherPaymentBeginningDto>> GetListAsync(PageRequestDto dto);
        Task<VoucherPaymentBeginningDto> GetByIdAsync(string partnerId);
        Task<VoucherPaymentBeginningDto> CreateAsync(CrudVoucherPaymentBeginningDto dto);
        //Task<List<BankPartnerDto>> GetListBankPartnerAsync(string partnerId);
        Task UpdateAsync(string id, CrudVoucherPaymentBeginningDto dto);
        Task DeleteAsync(string id);
        Task<List<VoucherPaymentBeginningDetailDto>> GetDetailAsync(string voucherPaymentBeginningId);
    }
}
