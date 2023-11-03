using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.PaymentTerms
{
    public interface IPaymentTermAppService
    {
        Task<PageResultDto<PaymentTermDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<PaymentTermDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetDataReference();
        Task<PaymentTermDto> GetByIdAsync(string paymentTermId);        
        Task<PaymentTermDto> CreateAsync(CrudPaymentTermDto dto);
        Task<List<PaymentTermDetailDto>> GetListPaymentTermDetailAsync(string paymentTermId);
        Task UpdateAsync(string id, CrudPaymentTermDto dto);
        Task DeleteAsync(string id);
    }
}
