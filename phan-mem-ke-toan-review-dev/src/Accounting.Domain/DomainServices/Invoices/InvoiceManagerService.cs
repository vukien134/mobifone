using Accounting.Constants;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Licenses;
using Microsoft.Extensions.Configuration;
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
    public class InvoiceManagerService : DomainService
    {
        #region Fields        
        private readonly IServiceProvider _serviceProvider;
        #endregion
        #region Ctor
        public InvoiceManagerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion
        public IEInvoice GetEInvoice(InvoiceSupplier supplier)
        {
            IEInvoice eInvoice = supplier.Code switch
            {
                InvoiceSupplierNameConst.EFY => this.CreateEFYInvoice(supplier.TaxCode,supplier.Link,
                                supplier.UserName,supplier.Password,supplier.CheckCircular,supplier.CertificateSerial),
                InvoiceSupplierNameConst.BKAV => this.CreateBkavInvoice(supplier.Link,supplier.PartnerGuid,
                                supplier.PartnerToken),
                InvoiceSupplierNameConst.VIETTEL => this.CreateViettelInvoice(supplier.TaxCode,supplier.Link,
                                supplier.UserName,supplier.Password, supplier.CertificateSerial),
                InvoiceSupplierNameConst.FPT => this.CreateFptInvoice(supplier.TaxCode,supplier.Link,
                                supplier.UserName,supplier.Password,supplier.CheckCircular,supplier.CertificateSerial),
                InvoiceSupplierNameConst.MINVOICE => this.CreateMInvoiceInvoice(supplier.Link,supplier.UserName,
                                supplier.Password),
                InvoiceSupplierNameConst.MOBIFONE => this.CreateMobifoneInvoice(supplier.Link, supplier.UserName,
                                supplier.Password),
                _ => null
            };            
            if (eInvoice == null) throw new Exception("Có lỗi xảy ra, xin vui lòng thử lại!");
            return eInvoice;
        }
        #region Private
        private EFYInvoice CreateEFYInvoice(string taxCode,string link,string userName,
                                string password,string checkCircular,string certSerial)
        {
            var model = new JsonObject
            {
                { "codeTax", taxCode },
                { "apiLink", link },
                { "username", userName },
                { "password", password },
                { "checkCircular", checkCircular },
                { "certificateSerial", certSerial }
            };
            var efyInvoice = new EFYInvoice(model, _serviceProvider);
            return efyInvoice;
        }
        private BkavInvoice CreateBkavInvoice(string link,string partnerGuid,string partnerToken)                                
        {
            JsonObject model = new JsonObject
            {
                { "apiURL", link },
                { "partnerGUID", partnerGuid },
                { "partnerTOKEN", partnerToken }
            };
            var invoice = new BkavInvoice(model, _serviceProvider);
            return invoice;
        }
        private ViettelInvoice CreateViettelInvoice(string taxCode,string link,string userName,
                            string password,string certSerial)
        {
            var model = new JsonObject()
            {
                { "codeTax", taxCode },
                { "apiLink", link },
                { "username", userName },
                { "password", password },
                { "certificateSerial", certSerial }
            };
            var invoice = new ViettelInvoice(model, _serviceProvider);
            return invoice;
        }
        private FPTInvoice CreateFptInvoice(string taxCode,string link,string userName,string password,
                        string checkCircular,string certSerial)
        {
            var model = new JsonObject
            {
                { "codeTax", taxCode },
                { "apiLink", link },
                { "username", userName },
                { "password", password },
                { "checkCircular", checkCircular },
                { "certificateSerial", certSerial }
            };
            var invoice = new FPTInvoice(model, _serviceProvider);
            return invoice;
        }
        private MinvoiceInvoice CreateMInvoiceInvoice(string link,string userName,string password)
        {
            var model = new JsonObject
            {
                { "apiLink", link },
                { "username", userName },
                { "password", password }
            };
            var invoice = new MinvoiceInvoice(model,_serviceProvider);
            return invoice;
        }
        private MobifoneInvoice CreateMobifoneInvoice(string link, string userName, string password)
        {
            var model = new JsonObject
            {
                { "apiLink", link },
                { "username", userName },
                { "password", password }
            };
            var invoice = new MobifoneInvoice(model,_serviceProvider);
            return invoice;
        }
        #endregion
    }
}