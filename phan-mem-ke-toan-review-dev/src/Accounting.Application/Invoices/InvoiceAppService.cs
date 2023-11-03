using Accounting.Common;
using Accounting.DomainServices.Invoices;
using Microsoft.Extensions.Localization;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.MultiTenancy;

namespace Accounting.Invoices
{
    public class InvoiceAppService : AccountingAppService, IInvoiceAppService
    {
        #region Fields
        private readonly InvoiceService _invoiceService;
        #endregion
        #region Ctor
        public InvoiceAppService(InvoiceService invoiceService
                            )
        {
            _invoiceService = invoiceService;
        }
        #endregion
        public async Task<JsonObject> InvoiceTemplate(JsonObject model) //GetMauHD
        {
            return await _invoiceService.InvoiceTemplate(model);
        }

        public async Task<JsonObject> Create(JsonObject model) //TaoHoaDon
        {
            return await _invoiceService.Create(model);
        }

        public async Task<JsonObject> Update(JsonObject model) //UpdateHoaDon
        {
            return await _invoiceService.Update(model);
        }

        public async Task<JsonObject> Delete(JsonObject model) //Xóa HĐ
        {
            return await _invoiceService.Delete(model);
        }

        public async Task<byte[]> Preview(JsonObject model)
        {
            return await _invoiceService.Preview(model);
        }

        public async Task<JsonObject> CreateHkd(JsonObject model)
        {
            return await _invoiceService.CreateHKD(model);
        }
        public async Task<JsonObject> UpdateInvoiceStatus(JsonObject model)
        {
            return await _invoiceService.UpdateInoiceStatus(model);
        }
        #region Methods

        #endregion
    }
}
