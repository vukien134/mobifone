using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Invoices.InvoiceAuths;
using Accounting.Licenses;
using IdentityServer4.Extensions;
using Newtonsoft.Json;
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
    public class InvoiceService : DomainService
    {
        #region Fields
        private readonly YearCategoryService _yearCategoryService;
        private readonly WebHelper _webHelper;
        private readonly InvoiceSupplierService _invoiceSupplierService;
        private readonly InvoiceManagerService _invoiceManagerService;
        private readonly InvoiceAuthService _invoiceAuthService;
        private readonly InvoiceAuthDetailService _invoiceAuthDetailService;
        #endregion
        #region Ctor
        public InvoiceService(YearCategoryService yearCategoryService,
                              WebHelper webHelper,
                              InvoiceSupplierService invoiceSupplierService,
                              InvoiceManagerService invoiceManagerService,
                              InvoiceAuthService invoiceAuthService,
                              InvoiceAuthDetailService invoiceAuthDetailService
                              )
        {
            _yearCategoryService = yearCategoryService;
            _webHelper = webHelper;
            _invoiceSupplierService = invoiceSupplierService;
            _invoiceManagerService = invoiceManagerService;
            _invoiceAuthService = invoiceAuthService;
            _invoiceAuthDetailService = invoiceAuthDetailService;
        }
        #endregion
        public async Task<JsonObject> InvoiceTemplate(JsonObject model)
        {
            var supplier = await _invoiceSupplierService.GetSupplierActive();
            IEInvoice eInvoice = _invoiceManagerService.GetEInvoice(supplier);
            JsonObject res = await eInvoice.InvoiceTemplate(model);
            return res;
        }

        public async Task<JsonObject> Create(JsonObject model)
        {
            var supplier = await _invoiceSupplierService.GetSupplierActive();
            IEInvoice eInvoice = _invoiceManagerService.GetEInvoice(supplier);
            var invoiceAuth = await _invoiceAuthService.GetQueryableAsync();
            var lstinvoiceAuth = invoiceAuth.Where(p => p.Id == model["id"].ToString()).ToList();
            var invoiceDetail = await _invoiceAuthDetailService.GetQueryableAsync();
            var lstInvoiceDetail = invoiceDetail.Where(p => p.InvoiceAuthId == model["id"].ToString()).ToList();
            List<InvoiceAuthDetail> invoiceAuthDetailDtos = lstInvoiceDetail;
            foreach (var item in lstinvoiceAuth)
            {
                if (item.InvoiceId == null)
                {
                    item.InvoiceAuthDetails = invoiceAuthDetailDtos;
                }
                else
                {
                    throw new Exception("Thông tin hóa đơn đã được khởi tạo trên Hệ thống Hóa đơn điện tử. Vui lòng kiểm tra!"); 
                }
               
            }

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var resulAll = JsonConvert.SerializeObject(lstinvoiceAuth[0]);

            JsonObject modelNew = (JsonObject)JsonObject.Parse(resulAll);
            if (!string.IsNullOrEmpty(model["ctttb_id"].ToString()))
            {
                modelNew.Add("idMobi", model["ctttb_id"].ToString());
            }
            modelNew.Add("id", model["id"].ToString());
            JsonObject res = await eInvoice.Create(modelNew);
            return res;
        }
        public async Task<JsonObject> CreateHKD(JsonObject model)
        {
            var supplier = await _invoiceSupplierService.GetSupplierActive();
            IEInvoice eInvoice = _invoiceManagerService.GetEInvoice(supplier);
            var invoiceAuth = await _invoiceAuthService.GetQueryableAsync();
            var tét = model["id"].ToString();
            var lstinvoiceAuth = invoiceAuth.Where(p => p.Id == model["id"].ToString()).ToList();
            var invoiceDetail = await _invoiceAuthDetailService.GetQueryableAsync();
            var lstInvoiceDetail = invoiceDetail.Where(p => p.InvoiceAuthId == model["id"].ToString()).ToList();
            List<InvoiceAuthDetail> invoiceAuthDetailDtos = lstInvoiceDetail;
            foreach (var item in lstinvoiceAuth)
            {
                if (item.InvoiceId == null)
                {
                    item.InvoiceAuthDetails = invoiceAuthDetailDtos;
                }
                else
                {
                    throw new Exception("Thông tin hóa đơn đã được khởi tạo trên Hệ thống Hóa đơn điện tử. Vui lòng kiểm tra!"); 
                }
            }

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var resulAll = JsonConvert.SerializeObject(lstinvoiceAuth[0]);

            JsonObject modelNew = (JsonObject)JsonObject.Parse(resulAll);
            if (!string.IsNullOrEmpty(model["ctttb_id"].ToString()))
            {
                modelNew.Add("idMobi", model["ctttb_id"].ToString());
            }

            JsonObject res = await eInvoice.CreateHKD(modelNew);
            return res;
        }

        public async Task<JsonObject> Update(JsonObject model)
        {
            var supplier = await _invoiceSupplierService.GetSupplierActive();
            IEInvoice eInvoice = _invoiceManagerService.GetEInvoice(supplier);
            JsonObject res = await eInvoice.Update(model);
            return res;
        }

        public async Task<JsonObject> Delete(JsonObject model)
        {
            var supplier = await _invoiceSupplierService.GetSupplierActive();
            IEInvoice eInvoice = _invoiceManagerService.GetEInvoice(supplier);
            JsonObject res = await eInvoice.Delete(model);
            return res;
        }
        public async Task<JsonObject> UpdateInoiceStatus(JsonObject model)
        {
            var supplier = await _invoiceSupplierService.GetSupplierActive();
            IEInvoice eInvoice = _invoiceManagerService.GetEInvoice(supplier);
            JsonObject res = await eInvoice.UpdateInvoiceStatus(model);
            return res;
        }
        public async Task<byte[]> Preview(JsonObject model)
        {
            var supplier = await _invoiceSupplierService.GetSupplierActive();
            IEInvoice eInvoice = _invoiceManagerService.GetEInvoice(supplier);
            return eInvoice.Preview(model);
        }
        public async Task<JsonObject> CheckProxy(JsonObject model)
        {
            JsonObject keys = new JsonObject();
            return keys;
        }

        #region Private
        #endregion
    }
}
