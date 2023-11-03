using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Excels;
using Accounting.Windows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.VoucherNumbers
{
    public interface IVoucherPaymentBookAppService
    {
        Task<PageResultDto<VoucherPaymentBookDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<VoucherPaymentBookDto>> GetListAsync(PageRequestDto dto);
        Task<VoucherPaymentBookDto> GetByIdAsync(string caseId);
        Task<VoucherPaymentBookDto> CreateAsync(CrudVoucherPaymentBookDto dto);
        Task UpdateAsync(string id, CrudVoucherPaymentBookDto dto);
        Task DeleteAsync(string id);
        Task<List<DataVoucherPaymentDto>> DataVoucherPaymentAsync(VoucherPaymentFilterDto dto);
        Task<List<DetailDataVoucherPaymentDto>> DetailVoucherPaymentAsync(DetailVoucherPaymentFilterDto dto);
        Task<ResultDto> PaymentAsync(ListPaymentFilterDto dto);
        Task<WindowDto> GetWindowByVoucherCodeAsync(string voucherCode);
    }
}
