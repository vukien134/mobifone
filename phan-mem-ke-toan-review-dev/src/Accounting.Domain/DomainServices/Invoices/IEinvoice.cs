using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.Licenses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.Invoices
{
    public interface IEInvoice
    {
        Task<JsonObject> Create(JsonObject model);
        Task<JsonObject> CreateHKD(JsonObject model);

        Task<JsonObject> UpdateInvoiceStatus(JsonObject model);
        Task<JsonObject> Update(JsonObject model);

        Task<JsonObject> Delete(JsonObject model);

        Task<string> Login();

        byte[] Preview(JsonObject model);

        Task<JsonObject> InvoiceTemplate(JsonObject model);

        Task<JsonObject> SaveXmlInvoice(JsonObject model);
    }
}
