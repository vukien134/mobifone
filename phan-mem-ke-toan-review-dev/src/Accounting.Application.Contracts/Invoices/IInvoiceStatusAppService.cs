using Accounting.BaseDtos;
using Accounting.Invoices.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Invoices
{
    public interface IInvoiceStatusAppService
    {
        Task<PageResultDto<InvoiceStatusDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<InvoiceStatusDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetDataReference();
        Task<InvoiceStatusDto> GetByIdAsync(string caseId);
        Task<InvoiceStatusDto> CreateAsync(CrudInvoiceStatusDto dto);
        Task UpdateAsync(string id, CrudInvoiceStatusDto dto);
        Task DeleteAsync(string id);        
    }
}
