using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Invoices
{
    public interface IInvoiceAppService
    {
        public Task<JsonObject> InvoiceTemplate(JsonObject model);
        public Task<JsonObject> Create(JsonObject model);
        public Task<JsonObject> CreateHkd(JsonObject model);
        public Task<JsonObject> Update(JsonObject model);
        public Task<JsonObject> Delete(JsonObject model);
        //public JObject Preview(JObject model);
    }
}
