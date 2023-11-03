using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Invoices.InvoiceAuths;
using Accounting.DomainServices.Invoices.InvoiceSuppliers;
using Accounting.Helpers;
using Accounting.Invoices;
using Accounting.Licenses;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.Invoices
{
    public class BkavInvoice : IEInvoice
    {
        private InvoiceSupplierService _invoiceSupplierService;
        private InvoiceAuthService _invoiceAuthService;
        private string _parterGUID = "";
        private string _partnerTOKEN = "";
        private string _apiURL = "";

        private WebClient _webClient;

        public BkavInvoice(JsonObject model, IServiceProvider serviceProvider)
        {
            _invoiceSupplierService = serviceProvider.GetRequiredService<InvoiceSupplierService>();
            _invoiceAuthService = serviceProvider.GetRequiredService<InvoiceAuthService>();
            this._apiURL = model["apiURL"].ToString();
            this._parterGUID = model["partnerGUID"].ToString();
            this._partnerTOKEN = model["partnerTOKEN"].ToString();
            this._webClient = new WebClient();
            _webClient.Encoding = Encoding.UTF8;
            _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/soap+xml; charset=utf-8");
        }

        private string EncryptBase64(string json)
        {
            string strbase64 = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(json));
            return strbase64;
        }

        private string DecryptBase64(string res)
        {
            var blob = Convert.FromBase64String(res);
            var json = Encoding.UTF8.GetString(blob);
            return json;
        }

        private string PrepareXMLSOAP(string base64Str)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
               + "<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\">"
               + "<soap12:Body>"
               + "<ExecCommand xmlns=\"http://tempuri.org/\">"
               + "<partnerGUID>" + this._parterGUID + "</partnerGUID>"
               + "<CommandData>" + base64Str + "</CommandData>"
               + "</ExecCommand>"
               + "</soap12:Body>"
               + "</soap12:Envelope>";

            return xml;
        }

        private JsonObject PrepareJsonData(JsonObject model)
        {
            JsonObject postData = new JsonObject();
            postData.Add("CmdType", 112);

            JsonObject dp = new JsonObject();
            dp.Add("InvoiceForm", model["InvoiceTemplate"].ToString());
            dp.Add("InvoiceSerial", model["InvoiceSeries"].ToString());
            dp.Add("InvoiceTypeID", 1);
            dp.Add("InvoiceDate", Convert.ToDateTime(model["IssuedDate"].ToString()));
            dp.Add("BuyerName", model["BuyerDisplayName"].ToString());
            dp.Add("BuyerTaxCode", model["BuyerTaxCode"].ToString());
            dp.Add("BuyerUnitName", model["BuyerLegalName"].ToString());
            dp.Add("BuyerAddress", model["BuyerAddressLine"].ToString());
            dp.Add("BuyerBankAccount", model["BuyerBankAccount"].ToString());
            dp.Add("PayMethodID", GetPaymentMethod(model["PaymentMethodName"].ToString()));
            dp.Add("ReceiveTypeID", 1);
            dp.Add("ReceiverEmail", "");
            dp.Add("ReceiverMobile", "");
            dp.Add("ReceiverAddress", "");
            dp.Add("ReceiverName", "");
            dp.Add("Note", model["InvoiceNote"].ToString());
            dp.Add("BillCode", model["BillCode"].ToString());
            dp.Add("CurrencyID", model["CurrencyCode"].ToString());
            dp.Add("ExchangeRate", Convert.ToDecimal(model["ExchangeRate"].ToString()));

            JsonArray postDetails = new JsonArray();

            JsonArray details = (JsonArray)model["details"];

            for (int i = 0; i < details.Count; i++)
            {
                JsonObject obj = (JsonObject)details[i];

                bool is_discount = obj["is_discount"] == null ? false : Convert.ToBoolean(obj["is_discount"].ToString());

                JsonObject item = new JsonObject();
                item.Add("ItemName", obj["ItemName"].ToString());
                item.Add("UnitName", obj["UnitName"].ToString());
                item.Add("Qty", Convert.ToDecimal(obj["Quantity"].ToString()));
                item.Add("Price", Convert.ToDecimal(obj["Price"].ToString()));
                item.Add("Amount", Convert.ToDecimal(obj["TotalAmountWithoutVat"].ToString()));
                item.Add("TaxRateID", GetTaxRateID(obj["VatPercentage"].ToString()));
                item.Add("TaxAmount", Convert.ToDecimal(obj["VatAmount"].ToString()));
                item.Add("DiscountRate", Convert.ToDecimal(obj["DiscountPercentage"].ToString()));
                item.Add("DiscountAmount", Convert.ToDecimal(obj["DiscountAmount"].ToString()));
                item.Add("IsDiscount", is_discount);
                item.Add("IsIncrease", null);
                item.Add("ItemTypeID", 0);
                postDetails.Add(item);
            }

            JsonObject invs = new JsonObject();

            invs.Add("Invoice", dp);
            invs.Add("ListInvoiceDetailsWS", postDetails);
            invs.Add("PartnerInvoiceID", 0);
            invs.Add("PartnerInvoiceStringID", model["id"].ToString());

            JsonArray commandObj = new JsonArray();
            commandObj.Add(invs);

            postData.Add("CommandObject", commandObj);

            return postData;
        }

        private int GetPaymentMethod(string paymentMethodName)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            dic.Add("TM", 1);
            dic.Add("CK", 2);
            dic.Add("TM/CK", 3);

            return dic[paymentMethodName];
        }

        private int GetTaxRateID(string taxRate)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            dic.Add("0", 1);
            dic.Add("5", 2);
            dic.Add("10", 3);
            dic.Add("-1", 4);
            dic.Add("R05G", 7);
            dic.Add("R10G", 8);
            dic.Add("8", 9);

            return dic[taxRate];
        }

        public async Task<JsonObject> Create(JsonObject model)
        {
            JsonObject postData = this.PrepareJsonData(model);
            string json = postData.ToString();

            string encryptStr = EncryptBase64(json);
            string xml = PrepareXMLSOAP(encryptStr);

            string respone = _webClient.UploadString(this._apiURL, xml);
            string ExecCommandResult = GetExecCommandResult(respone);

            string decryptStr = DecryptBase64(ExecCommandResult);

            JsonObject objDecrypt = (JsonObject)decryptStr;
            int status = Convert.ToInt32(objDecrypt["Status"]);

            JsonObject res = new JsonObject();

            if (status != 0)
            {
                res.Add("ok", false);
                res.Add("error", objDecrypt["Object"].ToString());
            }
            else
            {
                JsonArray array = (JsonArray)(objDecrypt["Object"].ToString());
                JsonObject invReturn = (JsonObject)array[0];

                string MessLog = invReturn["MessLog"].ToString();

                if (MessLog.Length > 0)
                {
                    res.Add("ok", false);
                    res.Add("error", MessLog);
                }
                else
                {
                    string id = model["id"].ToString();

                    string invoiceSeries = invReturn["InvoiceSerial"].ToString();
                    string invoiceTemplate = invReturn["InvoiceForm"].ToString();
                    string invoiceNumber = invReturn["InvoiceNo"].ToString().PadLeft(7, '0');
                    string reservationCode = invReturn["MTC"].ToString();

                    res.Add("ok", true);
                    res.Add("InvoiceSeries", invoiceSeries);
                    res.Add("InvoiceTemplate", invoiceTemplate);
                    res.Add("InvoiceNumber", invoiceNumber);
                    res.Add("ReservationCode", reservationCode);
                    res.Add("SupplierId", invReturn["InvoiceGUID"].ToString());
                    var updateInvoiceNumberDto = new UpdateInvoiceNumberDto
                    {
                        InvoiceSeries = invoiceSeries,
                        InvoiceNumber = invoiceNumber,
                        ReservationCode = reservationCode,
                        Id = id
                    };
                    await _invoiceAuthService.UpdateInvoiceNumberAsync(updateInvoiceNumberDto);
                }
            }

            return res;
        }

        public async Task<JsonObject> Update(JsonObject model)
        {
            JsonObject postData = this.PrepareJsonData(model);
            postData["CmdType"] = 200;

            string json = postData.ToString();

            string encryptStr = EncryptBase64(json);
            string xml = PrepareXMLSOAP(encryptStr);

            string respone = _webClient.UploadString(this._apiURL, xml);
            string ExecCommandResult = GetExecCommandResult(respone);

            string decryptStr = DecryptBase64(ExecCommandResult);

            JsonObject objDecrypt = (JsonObject)decryptStr;
            int status = Convert.ToInt32(objDecrypt["Status"]);

            JsonObject res = new JsonObject();

            if (status != 0)
            {
                res.Add("ok", false);
                res.Add("error", objDecrypt["Object"].ToString());
            }
            else
            {
                JsonArray array = (JsonArray)(objDecrypt["Object"].ToString());
                JsonObject invReturn = (JsonObject)array[0];

                string MessLog = invReturn["MessLog"].ToString();

                if (MessLog.Length > 0)
                {
                    res.Add("ok", false);
                    res.Add("error", MessLog);
                }
                else
                {
                    Guid id = Guid.Parse(model["id"].ToString());

                    string invoiceSeries = invReturn["InvoiceSerial"].ToString();
                    string invoiceTemplate = invReturn["InvoiceForm"].ToString();
                    string invoiceNumber = invReturn["InvoiceNo"].ToString().PadLeft(7, '0');

                    res.Add("ok", true);
                    res.Add("InvoiceSeries", invoiceSeries);
                    res.Add("InvoiceTemplate", invoiceTemplate);
                    res.Add("InvoiceNumber", invoiceNumber);
                    res.Add("SupplierId", invReturn["InvoiceGUID"].ToString());
                }
            }

            return res;
        }

        public async Task<JsonObject> Delete(JsonObject model)
        {
            JsonObject postData = new JsonObject();
            postData.Add("CmdType", 202);
            JsonObject item = new JsonObject();
            JsonObject reson = new JsonObject();
            reson.Add("Reson", model["reson"].ToString());
            item.Add("PartnerInvoiceID", 0);
            item.Add("PartnerInvoiceStringID", model["id"].ToString());
            item.Add("Invoice", reson);
            JsonArray array = new JsonArray();
            array.Add(item);
            postData.Add("CommandObject", array);

            string json = postData.ToString();

            string encryptStr = EncryptBase64(json);
            string xml = PrepareXMLSOAP(encryptStr);

            string respone = _webClient.UploadString(this._apiURL, xml);
            string ExecCommandResult = GetExecCommandResult(respone);

            string decryptStr = DecryptBase64(ExecCommandResult);

            JsonObject objDecrypt = (JsonObject)decryptStr;
            int status = Convert.ToInt32(objDecrypt["Status"]);

            JsonObject res = new JsonObject();

            if (status != 0)
            {
                res.Add("ok", false);
                res.Add("error", objDecrypt["Object"].ToString());
            }
            else
            {
                array = (JsonArray)(objDecrypt["Object"].ToString());
                JsonObject invReturn = (JsonObject)array[0];

                string MessLog = invReturn["MessLog"].ToString();

                if (MessLog.Length > 0)
                {
                    res.Add("ok", false);
                    res.Add("error", MessLog);
                }
                else
                {
                    res.Add("ok", true);
                }
            }

            return res;
        }

        public byte[] Preview(JsonObject model)
        {
            byte[] bytes = null;

            JsonObject postData = new JsonObject();
            postData.Add("CmdType", 808);
            postData.Add("CommandObject", model["id"].ToString());

            string json = postData.ToString();

            string encryptStr = EncryptBase64(json);
            string xml = PrepareXMLSOAP(encryptStr);

            string respone = _webClient.UploadString(this._apiURL, xml);
            string ExecCommandResult = GetExecCommandResult(respone);

            string decryptStr = DecryptBase64(ExecCommandResult);

            JsonObject objDecrypt = (JsonObject)decryptStr;
            int status = Convert.ToInt32(objDecrypt["Status"]);

            if (status != 0) return null;

            JsonObject invReturn = (JsonObject)(objDecrypt["Object"].ToString());

            string base64Pdf = invReturn["PDF"].ToString();
            bytes = Convert.FromBase64String(base64Pdf);

            return bytes;
        }

        private string GetExecCommandResult(string res)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(res);

            return doc.GetElementsByTagName("ExecCommandResult")[0].InnerText;
        }

        public Task<string> Login()
        {
            throw new NotImplementedException();
        }


        public Task<JsonObject> InvoiceTemplate(JsonObject model)
        {
            throw new NotImplementedException();
        }


        public Task<JsonObject> SaveXmlInvoice(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> CreateHKD(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> UpdateInvoiceStatus(JsonObject model)
        {
            throw new NotImplementedException();
        }
    }
}