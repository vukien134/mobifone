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
    public class EFYInvoice : IEInvoice
    {
        private InvoiceSupplierService _invoiceSupplierService;
        private InvoiceAuthService _invoiceAuthService;
        private string TOKEN_HEADER = "";
        private string _host = "";
        private string _taxcode = "";
        private string _username = "";
        private string _password = "";
        private string _checkCircular = "";
        private string Token = "";
        private string URL_CREATE_INVOICE = "/efyvn_exportInvoiceWaitSign";
        private string URL_CREATE_INVOICE_AND_SIGN = "/efyvn_exportInvoiceDraft";
        private string URL_CONVERT_INVOICE = "/efyvn_convertInvoice";
        private string URL_CANCEL_INVOICE = "/efyvn_cancelInvoice";
        private string URL_TEMPLATE_INVOICE = "/efyvn_viewInvoiceDraft";

        private WebClient _webClient = null;

        public EFYInvoice(JsonObject model, IServiceProvider serviceProvider)
        {
            _invoiceSupplierService = serviceProvider.GetRequiredService<InvoiceSupplierService>();
            _invoiceAuthService = serviceProvider.GetRequiredService<InvoiceAuthService>();
            _webClient = new WebClient();
            _webClient.Encoding = System.Text.Encoding.UTF8;
            _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            this._host = model["apiLink"].ToString();
            this._username = model["username"].ToString(); ;
            this._password = model["password"].ToString(); ;
            this._taxcode = model["codeTax"].ToString(); ;
            this._checkCircular = model["checkCircular"].ToString(); ;

            this.Login();

        }

        public async Task<JsonObject> Create(JsonObject model)
        {
            JsonObject obj = new JsonObject();
            if (_checkCircular == "K")
            {

                JsonObject postData = new JsonObject();
                postData.Add("other_id", model["id"].ToString());
                postData.Add("template_code", model["InvoiceTemplate"].ToString());
                postData.Add("invoice_date", Convert.ToDateTime(model["IssuedDate"]).ToString("yyyy-MM-dd HH:mm:ss"));
                postData.Add("invoice_series", model["InvoiceSeries"].ToString());
                postData.Add("status", "GIU_SO");
                postData.Add("customer_id", null);
                postData.Add("origin_id", null);
                postData.Add("adjustment_form", null);
                postData.Add("doc_relate_symbol", null);
                postData.Add("doc_relate_date", null);
                postData.Add("adjustment_type", Convert.ToInt32(model["AdjustmentType"].ToString()));
                postData.Add("customer_code", model["PartnerCode"].ToString());
                postData.Add("customer_name", model["BuyerLegalName"].ToString());
                postData.Add("customer_type", null);
                postData.Add("customer_object_code", null);
                postData.Add("buyer_name", model["BuyerDisplayName"].ToString());
                postData.Add("buyer_tax_code", model["BuyerTaxCode"].ToString());
                postData.Add("buyer_mobile", model["BuyerMobile"].ToString());
                postData.Add("buyer_address", model["BuyerAddressLine"].ToString());
                postData.Add("buyer_email", model["BuyerEmail"].ToString());
                postData.Add("bank_name", model["BuyerBankName"].ToString());
                postData.Add("bank_account_name", null);
                postData.Add("bank_account_number", model["BuyerBankAccount"].ToString());
                postData.Add("note", model["InvoiceNote"].ToString());
                postData.Add("currency_code", model["CurrencyCode"].ToString());
                postData.Add("currency_rate", Convert.ToInt32(model["inv_exchangeRate"]));
                postData.Add("payment_method_name", GetPaymentMethod(model["PaymentMethodName"].ToString()));
                postData.Add("amount", Convert.ToDecimal(model["TotalAmountWithoutVat"]));
                postData.Add("amount_discount", Convert.ToDecimal(model["DiscountAmount"]));
                postData.Add("total_amount_vat", Convert.ToDecimal(model["VatAmount"]));
                postData.Add("amount_after_vat", Convert.ToDecimal(model["TotalAmount"]));
                postData.Add("total_payment", Convert.ToDecimal(model["TotalAmount"]));
                postData.Add("total_payment_in_word", model["amount_to_word"].ToString());
                decimal t_thue5 = 0;
                decimal t_thue10 = 0;
                JsonArray postDataDetails = new JsonArray();
                JsonArray details = (JsonArray)model["details"];

                foreach (JsonObject item in details)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("order", Convert.ToInt32(item["Ord0"].ToString().Remove(0, 1)));
                    postItem.Add("view_order", Convert.ToInt32(item["Ord0"].ToString().Remove(0, 1)));
                    postItem.Add("product_code", item["ItemCode"].ToString());
                    postItem.Add("product_name", item["ItemName"].ToString());
                    postItem.Add("commercial_discount", false);
                    postItem.Add("is_promotion", Convert.ToBoolean(item["Promotion"]));
                    postItem.Add("unit_code", item["UnitCode"].ToString());
                    postItem.Add("unit_name", item["UnitName"].ToString());
                    postItem.Add("quantity", Convert.ToDecimal(item["Quantity"]));
                    postItem.Add("price", Convert.ToDecimal(item["Price"]));
                    int? vat = string.IsNullOrEmpty(item["VatPercentage"].ToString()) ? (int?)null : Convert.ToInt32(item["VatPercentage"]);
                    if (vat != null)
                    {
                        postItem.Add("vat", vat);
                        if (vat == 5)
                        {
                            t_thue5 = t_thue5 + Convert.ToDecimal(item["VatAmount"]);
                        }
                        else if (vat == 10)
                        {
                            t_thue10 = t_thue10 + Convert.ToDecimal(item["VatAmount"]);
                        }
                    }
                    postItem.Add("discount", null);
                    postItem.Add("amount_discount", Convert.ToDecimal(item["DiscountAmount"]));
                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"]));
                    postItem.Add("amount_vat", Convert.ToDecimal(item["VatAmount"]));
                    postItem.Add("amount_after_vat", Convert.ToDecimal(item["TotalAmount"]));
                    postDataDetails.Add(postItem);
                }
                postData.Add("amount_vatx", null);
                postData.Add("amount_vat0", null);
                postData.Add("amount_vat5", t_thue5);
                postData.Add("amount_vat10", t_thue10);
                postData.Add("total_amount_product_vat5", t_thue5);
                postData.Add("total_amount_product_vat10", t_thue10);
                postData.Add("total_amount_product_vatx", null);
                postData.Add("total_amount_product_vat0", null);
                postData.Add("amount_after_vatx", null);
                postData.Add("amount_after_vat0", null);
                postData.Add("amount_after_vat5", t_thue5);
                postData.Add("amount_after_vat10", t_thue10);

                postData.Add("invoice_products", postDataDetails);

                JsonObject postInvoice = new JsonObject();

                postInvoice.Add("invoice", postData);

                string url = this._host + this.URL_CREATE_INVOICE;

                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string result = _webClient.UploadString(url, postInvoice.ToString());

                JsonObject res = (JsonObject)result;


                if (res["status"].ToString() != "success")
                {
                    obj.Add("ok", false);
                    obj.Add("code", res["code"].ToString());
                    obj.Add("error", res["message"].ToString());
                }
                else
                {
                    string id = model["id"].ToString();
                    JsonObject inv = (JsonObject)res["data"].ToString();
                    string invoiceSeries = inv["invoice_series"].ToString();
                    string invoiceNumber = inv["invoice_number"].ToString().PadLeft(7, '0');
                    string invoiceTemplate = inv["template_code"].ToString();
                    string reservationCode = inv["verify_code"].ToString();

                    //Guid id_suppiler = Guid.Parse(inv["id"].ToString());
                    obj.Add("ok", true);
                    obj.Add("Id", inv["other_id"].ToString());
                    obj.Add("ReservationCode", inv["verify_code"].ToString());
                    obj.Add("InvoiceNumber", inv["invoice_number"].ToString());
                    obj.Add("IssuedDate", inv["invoice_date"].ToString());
                    obj.Add("InvoiceTemplate", inv["template_code"].ToString());
                    obj.Add("InvoiceSeries", model["InvoiceSeries"]);
                    obj.Add("AdjustmentType", model["AdjustmentType"]);
                    obj.Add("SupplierId", inv["id"].ToString());

                    var updateInvoiceNumberDto = new UpdateInvoiceNumberDto
                    {
                        InvoiceSeries = invoiceSeries,
                        InvoiceNumber = invoiceNumber,
                        ReservationCode = reservationCode,
                        InvoiceTemplate = invoiceTemplate,
                        Id = id
                    };
                    await _invoiceAuthService.UpdateInvoiceNumberAsync(updateInvoiceNumberDto);
                }

            }
            else
            {

                JsonObject postData = new JsonObject();
                postData.Add("other_id", model["id"].ToString());
                postData.Add("template_code", model["InvoiceTemplate"].ToString());
                postData.Add("invoice_series", model["InvoiceSeries"].ToString());
                postData.Add("origin_id", null);
                postData.Add("adjustment_form", null);
                postData.Add("doc_relate_symbol", null);
                postData.Add("doc_relate_date", null);
                postData.Add("adjustment_type", Convert.ToInt32(model["AdjustmentType"].ToString()));
                postData.Add("customer_name", model["BuyerLegalName"].ToString());
                postData.Add("customer_object_code", null);
                postData.Add("buyer_name", model["BuyerDisplayName"].ToString());
                postData.Add("buyer_tax_code", model["BuyerTaxCode"].ToString());
                postData.Add("buyer_mobile", model["BuyerMobile"].ToString());
                postData.Add("buyer_address", model["BuyerAddressLine"].ToString());
                postData.Add("buyer_email", model["BuyerEmail"].ToString());
                postData.Add("bank_name", model["BuyerBankName"].ToString());
                postData.Add("currency_code", model["CurrencyCode"].ToString());
                postData.Add("currency_rate", Convert.ToInt32(model["inv_exchangeRate"]));
                postData.Add("payment_method_name", GetPaymentMethod(model["PaymentMethodName"].ToString()));
                postData.Add("payment_method_view_name", GetPaymentMethod(model["PaymentMethodName"].ToString()));
                postData.Add("amount", Convert.ToDecimal(model["TotalAmountWithoutVat"]));
                postData.Add("amount_discount", Convert.ToDecimal(model["DiscountAmount"]));
                decimal t_thue5 = 0;
                decimal t_thue10 = 0;
                JsonArray postDataDetails = new JsonArray();
                JsonArray details = (JsonArray)model["details"];

                foreach (JsonObject item in details)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("view_order", Convert.ToInt32(item["Ord0"].ToString().Remove(0, 1)));
                    postItem.Add("product_code", item["ItemCode"].ToString());
                    postItem.Add("product_name", item["ItemName"].ToString());
                    postItem.Add("unit_code", item["UnitCode"].ToString());
                    postItem.Add("unit_name", item["UnitName"].ToString());
                    postItem.Add("quantity", Convert.ToDecimal(item["Quantity"]));
                    postItem.Add("price", Convert.ToDecimal(item["Price"]));
                    int? vat = string.IsNullOrEmpty(item["VatPercentage"].ToString()) ? (int?)null : Convert.ToInt32(item["VatPercentage"]);
                    if (vat != null)
                    {
                        postItem.Add("vat", vat);
                        if (vat == 5)
                        {
                            t_thue5 = t_thue5 + Convert.ToDecimal(item["VatAmount"]);
                        }
                        else if (vat == 10)
                        {
                            t_thue10 = t_thue10 + Convert.ToDecimal(item["VatAmount"]);
                        }
                    }
                    postItem.Add("discount", null);
                    postItem.Add("amount_discount", Convert.ToDecimal(item["DiscountAmount"]));
                    postItem.Add("amount_vat", Convert.ToDecimal(item["VatAmount"]));
                    postItem.Add("amount_after_vat", Convert.ToDecimal(item["TotalAmount"]));
                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"]));
                    postItem.Add("commercial_discount", false);
                    postItem.Add("is_promotion", Convert.ToBoolean(item["Promotion"]));
                    postDataDetails.Add(postItem);
                }
                postData.Add("amount_vat5", t_thue5);
                postData.Add("amount_vat10", t_thue10);
                postData.Add("total_amount_product_vat5", t_thue5);
                postData.Add("total_amount_product_vat10", t_thue10);
                postData.Add("total_amount_product_vatx", null);
                postData.Add("total_amount_product_vat0", null);
                postData.Add("total_amount_vat", Convert.ToDecimal(model["VatAmount"]));
                postData.Add("amount_after_vatx", null);
                postData.Add("amount_after_vat0", null);
                postData.Add("amount_after_vat5", t_thue5);
                postData.Add("amount_after_vat10", t_thue10);
                postData.Add("amount_after_vat", Convert.ToDecimal(model["TotalAmount"]));
                postData.Add("total_payment", Convert.ToDecimal(model["TotalAmount"]));
                if (Convert.ToDecimal(model["DiscountAmount"]) > 0)
                {
                    postData.Add("is_resolution_43", true);
                    postData.Add("sales_percentage", 20);
                }
                else
                {
                    postData.Add("is_resolution_43", false);
                    postData.Add("sales_percentage", 0);
                }

                postData.Add("money_resolution_43", Convert.ToDecimal(model["DiscountAmount"]));
                postData.Add("invoice_products", postDataDetails);
                JsonObject postInvoice = new JsonObject();
                postInvoice.Add("invoice", postData);
                string url = this._host + this.URL_CREATE_INVOICE_AND_SIGN;
                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string result = _webClient.UploadString(url, postInvoice.ToString());
                JsonObject res = (JsonObject)result;
                if (res["status"].ToString() != "success")
                {
                    obj.Add("ok", false);
                    obj.Add("code", res["code"].ToString());
                    obj.Add("error", res["message"].ToString());
                }
                else
                {
                    string id = model["id"].ToString();
                    JsonObject inv = (JsonObject)res["data"].ToString();
                    string invoiceSeries = inv["invoice_series"].ToString();
                    string invoiceNumber = inv["invoice_number"].ToString().PadLeft(7, '0');
                    string invoiceTemplate = inv["template_code"].ToString();
                    string reservationCode = inv["verify_code"].ToString();

                    //Guid id_suppiler = Guid.Parse(inv["id"].ToString());
                    obj.Add("ok", true);
                    obj.Add("Id", inv["other_id"].ToString());
                    obj.Add("ReservationCode", inv["verify_code"].ToString());
                    obj.Add("InvoiceNumber", inv["invoice_number"].ToString());
                    obj.Add("IssuedDate", inv["invoice_date"].ToString());
                    obj.Add("InvoiceTemplate", inv["template_code"].ToString());
                    obj.Add("InvoiceSeries", model["InvoiceSeries"]);
                    obj.Add("AdjustmentType", model["AdjustmentType"]);
                    obj.Add("SupplierId", inv["id"].ToString());

                    var updateInvoiceNumberDto = new UpdateInvoiceNumberDto
                    {
                        InvoiceSeries = invoiceSeries,
                        InvoiceNumber = invoiceNumber,
                        ReservationCode = reservationCode,
                        InvoiceTemplate = invoiceTemplate,
                        Id = id
                    };
                    await _invoiceAuthService.UpdateInvoiceNumberAsync(updateInvoiceNumberDto);
                }
            }
            return obj;
        }

        private string GetPaymentMethod(string paymentMethodName)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("TM", "TM");
            dic.Add("CK", "CK");
            dic.Add("TM/CK", "TMCK");

            return dic[paymentMethodName];
        }

        public async Task<JsonObject> Delete(JsonObject model)
        {
            JsonObject obj = new JsonObject();
            if (_checkCircular == "K")
            {

                JsonObject postInv = new JsonObject();
                postInv.Add("id", model["id_inv"].ToString());//id hóa đơn
                postInv.Add("doc_relate_symbol", model["doc_relate_symbol"].ToString());//Số, ký hiệu văn bản thỏa thuận giữa 2 bên"
                postInv.Add("doc_relate_date", model["IssuedDate"]);//Ngày văn bản xóa bỏ
                postInv.Add("destroy_reason", model["destroy_reason"].ToString());//Lý do hủy hóa đơn

                JsonObject postInvoice = new JsonObject();
                postInvoice.Add("invoice", postInv);

                string url = this._host + this.URL_CANCEL_INVOICE;

                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string result = _webClient.UploadString(url, postInvoice.ToString());
                JsonObject res = (JsonObject)result;


                if (res["status"].ToString() != "success")
                {
                    obj.Add("ok", false);
                    obj.Add("code", res["code"].ToString());
                    obj.Add("error", res["message"].ToString());
                }
                else
                {
                    obj.Add("ok", true);
                    obj.Add("code", res["code"].ToString());
                }
            }
            else
            {
                JsonObject postInv = new JsonObject();
                postInv.Add("other_id", model["id_inv"].ToString());//id hóa đơn
                postInv.Add("doc_relate_symbol", model["doc_relate_symbol"].ToString());//Số, ký hiệu văn bản thỏa thuận giữa 2 bên"
                postInv.Add("doc_relate_date", model["IssuedDate"]);//Ngày văn bản xóa bỏ
                postInv.Add("destroy_reason", model["destroy_reason"].ToString());//Lý do hủy hóa đơn

                JsonObject postInvoice = new JsonObject();
                postInvoice.Add("invoice", postInv);

                string url = this._host + this.URL_CANCEL_INVOICE;

                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                // _webClient.Headers.Add(HttpRequestHeader.Authorization, this.Token);
                string result = _webClient.UploadString(url, postInvoice.ToString());
                JsonObject res = (JsonObject)result;


                if (res["status"].ToString() != "success")
                {
                    obj.Add("ok", false);
                    obj.Add("code", res["code"].ToString());
                    obj.Add("error", res["message"].ToString());
                }
                else
                {
                    obj.Add("ok", true);
                    obj.Add("code", res["code"].ToString());
                }
            }
            return obj;
        }

        public Task<string> Login()
        {
            TOKEN_HEADER = Base64Encode(_username + ":" + _password);
            _webClient.Headers.Add(HttpRequestHeader.Authorization, "Basic " + TOKEN_HEADER);
            return Task.FromResult(TOKEN_HEADER);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public byte[] Preview(JsonObject model)
        {
            JsonObject obj = new JsonObject();
            JsonObject postData = new JsonObject();
            JsonObject res = new JsonObject();
            string result = "";
            string url = "";
            if (_checkCircular == "K")
            {


                postData.Add("other_id", model["id"].ToString());
                postData.Add("template_code", model["InvoiceTemplate"].ToString());
                postData.Add("invoice_date", Convert.ToDateTime(model["IssuedDate"]).ToString("yyyy-MM-dd HH:mm:ss"));
                postData.Add("invoice_series", model["InvoiceSeries"].ToString());
                postData.Add("status", "GIU_SO");
                postData.Add("customer_id", null);
                postData.Add("origin_id", null);
                postData.Add("adjustment_form", null);
                postData.Add("doc_relate_symbol", null);
                postData.Add("doc_relate_date", null);
                postData.Add("adjustment_type", Convert.ToInt32(model["AdjustmentType"].ToString()));
                postData.Add("customer_code", model["PartnerCode"].ToString());
                postData.Add("customer_name", model["BuyerLegalName"].ToString());
                postData.Add("customer_type", null);
                postData.Add("customer_object_code", null);
                postData.Add("buyer_name", model["BuyerDisplayName"].ToString());
                postData.Add("buyer_tax_code", model["BuyerTaxCode"].ToString());
                postData.Add("buyer_mobile", model["BuyerMobile"].ToString());
                postData.Add("buyer_address", model["BuyerAddressLine"].ToString());
                postData.Add("buyer_email", model["BuyerEmail"].ToString());
                postData.Add("bank_name", model["BuyerBankName"].ToString());
                postData.Add("bank_account_name", null);
                postData.Add("bank_account_number", model["BuyerBankAccount"].ToString());
                postData.Add("note", model["InvoiceNote"].ToString());
                postData.Add("currency_code", model["CurrencyCode"].ToString());
                postData.Add("currency_rate", Convert.ToInt32(model["inv_exchangeRate"]));
                postData.Add("payment_method_name", GetPaymentMethod(model["PaymentMethodName"].ToString()));
                postData.Add("amount", Convert.ToDecimal(model["TotalAmountWithoutVat"]));
                postData.Add("amount_discount", Convert.ToDecimal(model["DiscountAmount"]));
                postData.Add("total_amount_vat", Convert.ToDecimal(model["VatAmount"]));
                postData.Add("amount_after_vat", Convert.ToDecimal(model["TotalAmount"]));
                postData.Add("total_payment", Convert.ToDecimal(model["TotalAmount"]));
                postData.Add("total_payment_in_word", model["amount_to_word"].ToString());
                decimal t_thue5 = 0;
                decimal t_thue10 = 0;
                JsonArray postDataDetails = new JsonArray();
                JsonArray details = (JsonArray)model["details"];

                foreach (JsonObject item in details)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("order", Convert.ToInt32(item["Ord0"].ToString().Remove(0, 1)));
                    postItem.Add("view_order", Convert.ToInt32(item["Ord0"].ToString().Remove(0, 1)));
                    postItem.Add("product_code", item["ItemCode"].ToString());
                    postItem.Add("product_name", item["ItemName"].ToString());
                    postItem.Add("commercial_discount", false);
                    postItem.Add("is_promotion", Convert.ToBoolean(item["Promotion"]));
                    postItem.Add("unit_code", item["UnitCode"].ToString());
                    postItem.Add("unit_name", item["UnitName"].ToString());
                    postItem.Add("quantity", Convert.ToDecimal(item["Quantity"]));
                    postItem.Add("price", Convert.ToDecimal(item["Price"]));
                    int? vat = string.IsNullOrEmpty(item["VatPercentage"].ToString()) ? (int?)null : Convert.ToInt32(item["VatPercentage"]);
                    if (vat != null)
                    {
                        postItem.Add("vat", vat);
                        if (vat == 5)
                        {
                            t_thue5 = t_thue5 + Convert.ToDecimal(item["VatAmount"]);
                        }
                        else if (vat == 10)
                        {
                            t_thue10 = t_thue10 + Convert.ToDecimal(item["VatAmount"]);
                        }
                    }
                    postItem.Add("discount", null);
                    postItem.Add("amount_discount", Convert.ToDecimal(item["DiscountAmount"]));
                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"]));
                    postItem.Add("amount_vat", Convert.ToDecimal(item["VatAmount"]));
                    postItem.Add("amount_after_vat", Convert.ToDecimal(item["TotalAmount"]));
                    postDataDetails.Add(postItem);
                }
                postData.Add("amount_vatx", null);
                postData.Add("amount_vat0", null);
                postData.Add("amount_vat5", t_thue5);
                postData.Add("amount_vat10", t_thue10);
                postData.Add("total_amount_product_vat5", t_thue5);
                postData.Add("total_amount_product_vat10", t_thue10);
                postData.Add("total_amount_product_vatx", null);
                postData.Add("total_amount_product_vat0", null);
                postData.Add("amount_after_vatx", null);
                postData.Add("amount_after_vat0", null);
                postData.Add("amount_after_vat5", t_thue5);
                postData.Add("amount_after_vat10", t_thue10);

                postData.Add("invoice_products", postDataDetails);

                JsonObject postInvoice = new JsonObject();

                postInvoice.Add("invoice", postData);

                url = this._host + this.URL_TEMPLATE_INVOICE;

                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                result = _webClient.UploadString(url, postInvoice.ToString());

                res = (JsonObject)result;



            }
            else
            {


                postData.Add("other_id", model["id"].ToString());
                postData.Add("template_code", model["InvoiceTemplate"].ToString());
                postData.Add("invoice_series", model["InvoiceSeries"].ToString());
                postData.Add("origin_id", null);
                postData.Add("adjustment_form", null);
                postData.Add("doc_relate_symbol", null);
                postData.Add("doc_relate_date", null);
                postData.Add("adjustment_type", Convert.ToInt32(model["AdjustmentType"].ToString()));
                postData.Add("customer_name", model["BuyerLegalName"].ToString());
                // postData.Add("customer_type", null);
                postData.Add("customer_object_code", null);
                postData.Add("buyer_name", model["BuyerDisplayName"].ToString());
                postData.Add("buyer_tax_code", model["BuyerTaxCode"].ToString());
                postData.Add("buyer_mobile", model["BuyerMobile"].ToString());
                postData.Add("buyer_address", model["BuyerAddressLine"].ToString());
                postData.Add("buyer_email", model["BuyerEmail"].ToString());
                postData.Add("bank_name", model["BuyerBankName"].ToString());
                postData.Add("currency_code", model["CurrencyCode"].ToString());
                postData.Add("currency_rate", Convert.ToInt32(model["inv_exchangeRate"]));
                postData.Add("payment_method_name", GetPaymentMethod(model["PaymentMethodName"].ToString()));
                postData.Add("payment_method_view_name", GetPaymentMethod(model["PaymentMethodName"].ToString()));
                postData.Add("amount", Convert.ToDecimal(model["TotalAmountWithoutVat"]));
                postData.Add("amount_discount", Convert.ToDecimal(model["DiscountAmount"]));
                decimal t_thue5 = 0;
                decimal t_thue10 = 0;
                JsonArray postDataDetails = new JsonArray();
                JsonArray details = (JsonArray)model["details"];

                foreach (JsonObject item in details)
                {
                    JsonObject postItem = new JsonObject();
                    postItem.Add("view_order", Convert.ToInt32(item["Ord0"].ToString().Remove(0, 1)));
                    postItem.Add("product_code", item["ItemCode"].ToString());
                    postItem.Add("product_name", item["ItemName"].ToString());
                    postItem.Add("unit_code", item["UnitCode"].ToString());
                    postItem.Add("unit_name", item["UnitName"].ToString());
                    postItem.Add("quantity", Convert.ToDecimal(item["Quantity"]));
                    postItem.Add("price", Convert.ToDecimal(item["Price"]));
                    int? vat = string.IsNullOrEmpty(item["VatPercentage"].ToString()) ? (int?)null : Convert.ToInt32(item["VatPercentage"]);
                    if (vat != null)
                    {
                        postItem.Add("vat", vat);
                        if (vat == 5)
                        {
                            t_thue5 = t_thue5 + Convert.ToDecimal(item["VatAmount"]);
                        }
                        else if (vat == 10)
                        {
                            t_thue10 = t_thue10 + Convert.ToDecimal(item["VatAmount"]);
                        }
                    }
                    postItem.Add("discount", null);
                    postItem.Add("amount_discount", Convert.ToDecimal(item["DiscountAmount"]));
                    postItem.Add("amount_vat", Convert.ToDecimal(item["VatAmount"]));
                    postItem.Add("amount_after_vat", Convert.ToDecimal(item["TotalAmount"]));
                    postItem.Add("amount", Convert.ToDecimal(item["TotalAmountWithoutVat"]));
                    postItem.Add("commercial_discount", false);
                    postItem.Add("is_promotion", Convert.ToBoolean(item["Promotion"]));
                    postDataDetails.Add(postItem);
                }
                postData.Add("amount_vat5", t_thue5);
                postData.Add("amount_vat10", t_thue10);
                postData.Add("total_amount_product_vat5", t_thue5);
                postData.Add("total_amount_product_vat10", t_thue10);
                postData.Add("total_amount_product_vatx", null);
                postData.Add("total_amount_product_vat0", null);
                postData.Add("total_amount_vat", Convert.ToDecimal(model["VatAmount"]));
                postData.Add("amount_after_vatx", null);
                postData.Add("amount_after_vat0", null);
                postData.Add("amount_after_vat5", t_thue5);
                postData.Add("amount_after_vat10", t_thue10);
                postData.Add("amount_after_vat", Convert.ToDecimal(model["TotalAmount"]));
                postData.Add("total_payment", Convert.ToDecimal(model["TotalAmount"]));
                postData.Add("invoice_products", postDataDetails);
                JsonObject postInvoice = new JsonObject();
                postInvoice.Add("invoice", postData);
                url = this._host + this.URL_TEMPLATE_INVOICE;
                _webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                result = _webClient.UploadString(url, postInvoice.ToString());
                res = (JsonObject)result;
            }
            url = this._host + this.URL_TEMPLATE_INVOICE;
            string url_invoiceView = "";
            byte[] bytes = null;
            res = (JsonObject)result;
            obj = new JsonObject();

            if (res["status"].ToString() != "success")
            {
                obj.Add("ok", false);
                obj.Add("code", res["code"].ToString());
                obj.Add("error", res["message"].ToString());
            }
            else
            {
                Guid id = Guid.Parse(model["id"].ToString());
                JsonObject inv = (JsonObject)res["data"].ToString();
                url_invoiceView = inv["link_pdf_file"].ToString();
            }
            if (!string.IsNullOrEmpty(url_invoiceView))
            {
                bytes = _webClient.DownloadData(url_invoiceView);
                return bytes;
            }
            return bytes;
        }

        public Task<JsonObject> GetTemplateInvoice(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> SaveXmlInvoice(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> SearchInvoice(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> Update(JsonObject model)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> InvoiceTemplate(JsonObject model)
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