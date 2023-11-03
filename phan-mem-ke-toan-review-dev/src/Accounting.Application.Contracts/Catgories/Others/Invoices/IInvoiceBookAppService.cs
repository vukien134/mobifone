using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.Invoices
{
    public interface IInvoiceBookAppService
    {
        Task<PageResultDto<InvoiceBookDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<InvoiceBookDto>> GetListAsync(PageRequestDto dto);
        Task<List<InvoiceBookComboItemDto>> GetDataReference();
        Task<InvoiceBookDto> GetByIdAsync(string invoiceBookId);
        Task<InvoiceBookDto> CreateAsync(CrudInvoiceBookDto dto);
        Task UpdateAsync(string id, CrudInvoiceBookDto dto);
        Task DeleteAsync(string id);
    }
}
