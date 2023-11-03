using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Constants
{
    public static class MIMETYPE
    {
        public const string XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string DOCX = "application/msword";
        public const string PDF = "application/pdf";
        public const string XML = "application/xml";
        public const string HTML = "text/html";
        public const string OCTET_STREAM = "application/octet-stream";
        public const string ZIP = "application/zip";

        public static string GetContentType(string type)
        {
            string result = type switch
            {
                "pdf" => PDF,
                "xlsx" => XLSX,
                "xml" => XML,
                _ => null
            };
            return result;
        }
    }

    public static class AssetToolConst
    {
        public const string Asset = "TS";
        public const string Tool = "CC";
        public const decimal PriceDifference = 100;
    }

    public class ExcutionStatus
    {
        public const string Waiting = "Chờ thực hiện";
        public const string Excuting = "Đang thực hiện";
        public const string Ended = "Kết thúc";
        public const string Error = "Lỗi";
    }
    public class TenantSettingType
    {
        public const string Report = "Report";
    }

    public class CostProduction
    {
        public const int Round = 6;
    }
    public class JavaScriptCodeType
    {
        public const string Report = "report";
        public const string Window = "window";
    }
    public class PermissionGroup
    {
        public const string Report = "Report";
    }
    public class ActionType
    {
        public const string View = "View";
    }
    public class LedgerParameterConst
    {
        public const string LstOrgCode = "lstOrgCode";
        public const string FromDate = "fromDate";
        public const string ToDate = "toDate";
        public const string CurrencyCode = "currecyCode";
        public const string PartnerCode = "partnerCode";
        public const string ReciprocalPartnerCode = "reciprocalPartnerCode";
        public const string CaseCode = "caseCode";
        public const string DepartmentCode = "departmentCode";
        public const string BusinessCode = "businessCode";
        public const string LstVoucherCode = "lstVoucherCode";
        public const string Acc = "acc";
        public const string ReciprocalAcc = "reciprocalAcc";
        public const string WorkPlaceCode = "workPlaceCode";
        public const string ContractCode = "contractCode";
        public const string ReciprocalContractCode = "reciprocalContractCode";
        public const string FProductWorkCode = "fProductWorkCode";
        public const string ReciprocalFProductWorkCode = "reciprocalFProductWorkCode";
        public const string SectionCode = "sectionCode";
        public const string ReciprocalSectionCode = "reciprocalSectionCode";
        public const string CreatorName = "creatorName";
        public const string DebitOrCredit = "debitOrCredit";
        public const string Year = "year";
        public const string BeginVoucherNumber = "beginVoucherNumber";
        public const string EndVoucherNumber = "endVoucherNumber";
        public const string PartnerGroup = "partnerGroup";
        public const string VoucherCode = "voucherCode";
        public const string Sort = "sort";
        public const string CreationUser = "creationUser";
        public const string CreationTime = "creationTime";
        public const string CleaningAcc = "cleaningAcc";
        public const string CleaningFProductWork = "cleaningFProductWork";
        public const string AccRank = "accRank";
    }

    public class WarehouseBookParameterConst
    {
        public const string OrgCode = "orgCode";
        public const string LstOrgCode = "lstOrgCode";
        public const string FromDate = "fromDate";
        public const string ToDate = "toDate";
        public const string CurrencyCode = "currecyCode";
        public const string PartnerCode = "partnerCode";
        public const string ReciprocalPartnerCode = "reciprocalPartnerCode";
        public const string CaseCode = "caseCode";
        public const string DepartmentCode = "departmentCode";
        public const string BusinessCode = "businessCode";
        public const string LstVoucherCode = "lstVoucherCode";
        public const string Acc = "acc";
        public const string Acc1 = "acc1";
        public const string ReciprocalAcc = "reciprocalAcc";
        public const string CreditAcc2 = "creditAcc2";
        public const string DebitAcc = "debitAcc";
        public const string WorkPlaceCode = "workPlaceCode";
        public const string ContractCode = "contractCode";
        public const string ReciprocalContractCode = "reciprocalContractCode";
        public const string FProductWorkCode = "fProductWorkCode";
        public const string ReciprocalFProductWorkCode = "reciprocalFProductWorkCode";
        public const string SectionCode = "sectionCode";
        public const string ReciprocalSectionCode = "reciprocalSectionCode";
        public const string CreatorName = "creatorName";
        public const string DebitOrCredit = "debitOrCredit";
        public const string Year = "year";
        public const string Status = "status";
        public const string ChannelCode = "channelCode";
        public const string WarehouseCode = "warehouseCode";
        public const string ProductCode = "productCode";
        public const string ProductLotCode = "productLotCode";
        public const string ProductOriginCode = "productOriginCode";
        public const string ProductGroupCode = "productGroupCode";
        public const string ProductOrigin = "productOrigin";
        public const string BeginVoucherNumber = "beginVoucherNumber";
        public const string EndVoucherNumber = "endVoucherNumber";
        public const string PartnerGroup = "partnerGroup";
        public const string VoucherCode = "voucherCode";
        public const string Sort = "sort";
        public const string SaleChannel = "saleChannel";
        public const string VoucherGroup = "voucherGroup";
        public const string OpenYear = "openYear";
    }

    public class CkktType
    {
        public const string N = "N";
        public const string C = "C";
    }
    public class VoucherStatusType
    {
        public const string Save = "1";
        public const string UnSave = "2";
    }
    public class ColumnType
    {
        public const string Number = "number";
        public const string Decimal = "decimal";
        public const string Text = "string";
        public const string Date = "date";
    }
    public class OperatorFilter
    {
        public const string NumEqual = "=";
        public const string GreaterThan = ">";
        public const string GreaterThanOrEqual = ">=";
        public const string LessThan = "<";
        public const string LessThanOrEqual = "<=";
    }
    public class JobStatus
    {
        public const string WaitForRun = "WaitForRun";
        public const string Completed = "Completed";
        public const string Error = "Error";
    }

    public class SectionConst
    {
        public static Dictionary<string, string> Default = new Dictionary<string, string>()
        {
            { "154A", "Chi phí nhân công" },
            { "154B", "Chi phí điện" },
            { "154C", "Chi phí nước" },
            { "154D", "Chi phí viễn thông" },
            { "154E", "Chi phí thuê kho bãi, mặt bằng kinh doanh" },
            { "154F", "Chi phí quản lý (chi phí văn phòng phẩm, công cụ, dụng cụ...)" },
            { "154G", "Chi phí khác (hội nghị, công tác phí, thanh lý, nhượng bán tài sản cố định, thuê ngoài khác...)" }
        };
    }
    public class LinkCodeConst
    {
        public const string CaseCode = "CaseCode";
        public const string SectionCode = "SectionCode";
        public const string PartnerCode = "PartnerCode";
        public const string ProductCode = "ProductCode";
        public const string FProductWorkCode = "FProductWorkCode";
        public const string ContractCode = "ContractCode";
        public const string BusinessCode = "BusinessCode";
        public const string CurrencyCode = "CurrencyCode";
        public const string WorkPlaceCode = "WorkPlaceCode";
        public const string DepartmentCode = "DepartmentCode";
        public const string SaleChannelCode = "SaleChannelCode";
        public const string ProductGroupCode = "ProductGroupCode";
        public const string ProductLotCode = "ProductLotCode";
        public const string ProductOriginCode = "ProductOriginCode";
        public const string WarehouseCode = "WarehouseCode";
        public const string AccCode = "AccCode";
        public const string CapitalCode = "CapitalCode";
        public const string PurposeCode = "PurposeCode";
        public const string AssetToolCode = "AssetToolCode";
        public const string ReasonCode = "ReasonCode";
        public const string ProductPeriodCode = "ProductPeriodCode";
        public const string EmployeeCode = "EmployeeCode";
        public const string OrgUnitCode = "OrgUnitCode";
        public const string TaxCode = "TaxCode";
    }
    public static class AbpSettingKey
    {
        public const string ABP_MAILING_DEFAULT_ADRESS = "Abp.Mailing.DefaultFromAddress";
        public const string ABP_MAILING_DEFAULT_DISPLAYNAME = "Abp.Mailing.DefaultFromDisplayName";
        public const string ABP_MAILING_SMTP_HOST = "Abp.Mailing.Smtp.Host";
        public const string ABP_MAILING_SMTP_PORT = "Abp.Mailing.Smtp.Port";
        public const string ABP_MAILING_SMTP_USERNAME = "Abp.Mailing.Smtp.UserName";
        public const string ABP_MAILING_SMTP_PASSWORD = "Abp.Mailing.Smtp.Password";
        public const string ABP_MAILING_SMTP_ENABLESSL = "Abp.Mailing.Smtp.EnableSsl";
        public const string ABP_MAILING_SMTP_DOMAIN = "Abp.Mailing.Smtp.Domain";
        public const string ABP_MAILING_SMTP_USE_DEFAULT_CREDENTIALS = "Abp.Mailing.Smtp.UseDefaultCredentials";
    }
    public static class LangConst
    {
        public const string EN = "en";
        public const string VI = "vi";
    }
    public static class InvoiceSupplierNameConst
    {
        public const string EFY = "EFY";
        public const string BKAV = "BKAV";
        public const string VIETTEL = "VIETTEL";
        public const string FPT = "FPT";
        public const string MINVOICE = "M-INVOICE";
        public const string MOBIFONE = "MOBIFONE";
    }
    public static class ContentTypeConst
    {
        public const string ApplicationJson = "application/json";
    }
    public static class InvoiceConst
    {
        public const string WaitingNumber = "Chờ sinh số";
        public const string CodeNotCqt = "CQT không cấp mã";
        public const string InvoiceNotCqt = "CQT không tiếp nhận HĐ";
        public const string InvoiceCQT = "Đã cấp mã";
        public const string Signed = "Đã Ký";
    }

    public static class CompanyTypeConst
    {
        public const string Service = "Service";
        public const string Commerce = "Commerce";
        public const string Manufacture = "Manufacture";
        public const string Construct = "Construct";
        public const string Synthetic = "Synthetic";
        public const string Household = "Household";
    }
}
