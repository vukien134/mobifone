using Accounting.BaseDtos;
using Accounting.Excels;
using Accounting.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Accounting.Invoices.InvoiceAuths
{
    public interface IInvoiceAuthAppService
    {
        Task<PageResultDto<InvoiceAuthDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<InvoiceAuthDto>> GetListAsync(PageRequestDto dto);
        Task<InvoiceAuthDto> GetByIdAsync(string partnerId);
        Task<InvoiceAuthDto> CreateAsync(CrudInvoiceAuthDto dto);
        Task<List<InvoiceAuthDetailDto>> GetListInvoiceAuthDetailAsync(string invoiceAuthId);
        Task UpdateAsync(string id, CrudInvoiceAuthDto dto);
        Task<JsonObject> DeleteAsync(string id);
        Task<ResultDto> InvoiceAuthUpdate(JsonObject dto);

    }
}
