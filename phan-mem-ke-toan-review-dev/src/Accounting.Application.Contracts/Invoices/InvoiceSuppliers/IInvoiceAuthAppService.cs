using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Invoices.InvoiceSuppliers
{
    public interface IInvoiceSupplierAppService
    {
        Task<PageResultDto<InvoiceSupplierDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<InvoiceSupplierDto>> GetListAsync(PageRequestDto dto);
        Task<InvoiceSupplierDto> GetByIdAsync(string partnerId);
        Task<InvoiceSupplierDto> CreateAsync(CrudInvoiceSupplierDto dto);
        Task UpdateAsync(string id, CrudInvoiceSupplierDto dto);
        Task DeleteAsync(string id);
    }
}
