using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Permissions
{
    public static class ReportPermissions 
    {
        public const string GroupName = "Report";        

        //Action Report
        public const string ActionView = "View";        
        public const string ActionPrint = "Print";
        public const string ActionExportExcel = "ExportExcel";

        //Group Report
        public const string GeneralDiaryGroup = "GeneralDiaryReportGroup";
        public const string FinacialGroup = "FinacialReportGroup";
        public const string ImportExportGroup = "ImportExportReportGroup";
        public const string BookRecordingGroup = "BookRecordingReportGroup";
        public const string DebtGroup = "DebtReportGroup";
        public const string AssetGroup = "AssetReportGroup";
        public const string ToolGroup = "ToolReportGroup";
        public const string TaxGroup = "TaxReportGroup";
        public const string CostGroup = "CostReportGroup";
        public const string HkdGroup = "HkdReportGroup";
        public const string SummaryGroup = "SummaryReportGroup";
        public const string VoucherGroup = "VoucherReportGroup";
        public const string CategoryGroup = "CategoryReportGroup";

        //Sổ nhật ký chung
        public const string GeneralDiaryBookReport = "GeneralDiaryBook_Report";
        public const string GeneralDiaryBookReportView = GeneralDiaryBookReport + $"_{ActionView}";
        public const string GeneralDiaryBookReportPrint = GeneralDiaryBookReport + $"_{ActionPrint}";
        public const string GeneralDiaryBookReportExportExcel = GeneralDiaryBookReport + $"_{ActionExportExcel}";

        public const string AccountDetailBookReport = "AccountDetailBook_Report";
        public const string AccountDetailBookReportView = AccountDetailBookReport + $"_{ActionView}";
        public const string AccountDetailBookReportPrint = AccountDetailBookReport + $"_{ActionPrint}";
        public const string AccountDetailBookReportExportExcel = AccountDetailBookReport + $"_{ActionExportExcel}";

        public const string CashBookReport = "CashBook_Report";
        public const string CashBookReportView = CashBookReport + $"_{ActionView}";
        public const string CashBookReportPrint = CashBookReport + $"_{ActionPrint}";
        public const string CashBookReportExportExcel = CashBookReport + $"_{ActionExportExcel}";

        public const string CashInBankBookReport = "CashInBankBook_Report";
        public const string CashInBankBookReportView = CashInBankBookReport + $"_{ActionView}";
        public const string CashInBankBookReportPrint = CashInBankBookReport + $"_{ActionPrint}";
        public const string CashInBankBookReportExportExcel = CashInBankBookReport + $"_{ActionExportExcel}";

        public const string LedgerBookReport = "LedgerBook_Report";
        public const string LedgerBookReportView = LedgerBookReport + $"_{ActionView}";
        public const string LedgerBookReportPrint = LedgerBookReport + $"_{ActionPrint}";
        public const string LedgerBookReportExportExcel = LedgerBookReport + $"_{ActionExportExcel}";

        public const string BalanceSheetAccReport = "BalanceSheetAcc_Report";
        public const string BalanceSheetAccReportView = BalanceSheetAccReport + $"_{ActionView}";
        public const string BalanceSheetAccReportPrint = BalanceSheetAccReport + $"_{ActionPrint}";
        public const string BalanceSheetAccReportExportExcel = BalanceSheetAccReport + $"_{ActionExportExcel}";

        public const string MultiAccountDetailBookReport = "MultiAccountDetailBook_Report";
        public const string MultiAccountDetailBookReportView = MultiAccountDetailBookReport + $"_{ActionView}";
        public const string MultiAccountDetailBookReportPrint = MultiAccountDetailBookReport + $"_{ActionPrint}";
        public const string MultiAccountDetailBookReportExportExcel = MultiAccountDetailBookReport + $"_{ActionExportExcel}";

        public const string MultiLedgerBookReport = "MultiLedgerBook_Report";
        public const string MultiLedgerBookReportView = MultiLedgerBookReport + $"_{ActionView}";
        public const string MultiLedgerBookReportPrint = MultiLedgerBookReport + $"_{ActionPrint}";
        public const string MultiLedgerBookReportExportExcel = MultiLedgerBookReport + $"_{ActionExportExcel}";

        public const string CashCollectionDiaryReport = "CashCollectionDiary_Report";
        public const string CashCollectionDiaryReportView = CashCollectionDiaryReport + $"_{ActionView}";
        public const string CashCollectionDiaryReportPrint = CashCollectionDiaryReport + $"_{ActionPrint}";
        public const string CashCollectionDiaryReportExportExcel = CashCollectionDiaryReport + $"_{ActionExportExcel}";

        public const string CashSpendDiaryReport = "CashSpendDiary_Report";
        public const string CashSpendDiaryReportView = CashSpendDiaryReport + $"_{ActionView}";
        public const string CashSpendDiaryReportPrint = CashSpendDiaryReport + $"_{ActionPrint}";
        public const string CashSpendDiaryReportExportExcel = CashSpendDiaryReport + $"_{ActionExportExcel}";

        public const string DepositCollectionDiaryReport = "DepositCollectionDiary_Report";
        public const string DepositCollectionDiaryReportView = DepositCollectionDiaryReport + $"_{ActionView}";
        public const string DepositCollectionDiaryReportPrint = DepositCollectionDiaryReport + $"_{ActionPrint}";
        public const string DepositCollectionDiaryReportExportExcel = DepositCollectionDiaryReport + $"_{ActionExportExcel}";

        public const string DepositSpendDiaryReport = "DepositSpendDiary_Report";
        public const string DepositSpendDiaryReportView = DepositSpendDiaryReport + $"_{ActionView}";
        public const string DepositSpendDiaryReportPrint = DepositSpendDiaryReport + $"_{ActionPrint}";
        public const string DepositSpendDiaryReportExportExcel = DepositSpendDiaryReport + $"_{ActionExportExcel}";

        public const string PurchaseDiaryReport = "PurchaseDiary_Report";
        public const string PurchaseDiaryReportView = PurchaseDiaryReport + $"_{ActionView}";
        public const string PurchaseDiaryReportPrint = PurchaseDiaryReport + $"_{ActionPrint}";
        public const string PurchaseDiaryReportExportExcel = PurchaseDiaryReport + $"_{ActionExportExcel}";

        public const string SellingDiaryReport = "SellingDiary_Report";
        public const string SellingDiaryReportView = SellingDiaryReport + $"_{ActionView}";
        public const string SellingDiaryReportPrint = SellingDiaryReport + $"_{ActionPrint}";
        public const string SellingDiaryReportExportExcel = SellingDiaryReport + $"_{ActionExportExcel}";

        public const string AccountingTReport = "AccountT_Report";
        public const string AccountingTReportView = AccountingTReport + $"_{ActionView}";
        public const string AccountingTReportPrint = AccountingTReport + $"_{ActionPrint}";
        public const string AccountingTReportExportExcel = AccountingTReport + $"_{ActionExportExcel}";

        public const string ListOfVouchersBookReport = "ListOfVouchersBook_Report";
        public const string ListOfVouchersBookReportView = ListOfVouchersBookReport + $"_{ActionView}";
        public const string ListOfVouchersBookReportPrint = ListOfVouchersBookReport + $"_{ActionPrint}";
        public const string ListOfVouchersBookReportExportExcel = ListOfVouchersBookReport + $"_{ActionExportExcel}";

        public const string ListOfVoucherMultiBookReport = "ListOfVoucherMultiBook_Report";
        public const string ListOfVoucherMultiBookReportView = ListOfVoucherMultiBookReport + $"_{ActionView}";
        public const string ListOfVoucherMultiBookReportPrint = ListOfVoucherMultiBookReport + $"_{ActionPrint}";
        public const string ListOfVoucherMultiBookReportExportExcel = ListOfVoucherMultiBookReport + $"_{ActionExportExcel}";

        public const string ListAccVoucherReport = "ListAccVoucher_Report";
        public const string ListAccVoucherReportView = ListAccVoucherReport + $"_{ActionView}";
        public const string ListAccVoucherReportPrint = ListAccVoucherReport + $"_{ActionPrint}";
        public const string ListAccVoucherReportExportExcel = ListAccVoucherReport + $"_{ActionExportExcel}";

        public const string AccountAnalysisReport = "AccountAnalysis_Report";
        public const string AccountAnalysisReportView = AccountAnalysisReport + $"_{ActionView}";
        public const string AccountAnalysisReportPrint = AccountAnalysisReport + $"_{ActionPrint}";
        public const string AccountAnalysisReportExportExcel = AccountAnalysisReport + $"_{ActionExportExcel}";

        public const string ImportTempVoucherReport = "ImportTempVoucher_Report";
        public const string ImportTempVoucherReportView = ImportTempVoucherReport + $"_{ActionView}";
        public const string ImportTempVoucherReportPrint = ImportTempVoucherReport + $"_{ActionPrint}";
        public const string ImportTempVoucherReportExportExcel = ImportTempVoucherReport + $"_{ActionExportExcel}";

        //Báo cáo nhập - xuất - tồn
        public const string DetailedInventoryBookReport = "DetailedInventoryBook_Report";
        public const string DetailedInventoryBookReportView = DetailedInventoryBookReport + $"_{ActionView}";
        public const string DetailedInventoryBookReportPrint = DetailedInventoryBookReport + $"_{ActionPrint}";
        public const string DetailedInventoryBookReportExportExcel = DetailedInventoryBookReport + $"_{ActionExportExcel}";

        public const string InventorySummaryReport = "InventorySummary_Report";
        public const string InventorySummaryReportView = InventorySummaryReport + $"_{ActionView}";
        public const string InventorySummaryReportPrint = InventorySummaryReport + $"_{ActionPrint}";
        public const string InventorySummaryReportExportExcel = InventorySummaryReport + $"_{ActionExportExcel}";

        public const string ClosingInventoryReport = "ClosingInventory_Report";
        public const string ClosingInventoryReportView = ClosingInventoryReport + $"_{ActionView}";
        public const string ClosingInventoryReportPrint = ClosingInventoryReport + $"_{ActionPrint}";
        public const string ClosingInventoryReportExportExcel = ClosingInventoryReport + $"_{ActionExportExcel}";

        public const string OpeningInventoryReport = "OpeningInventory_Report";
        public const string OpeningInventoryReportView = OpeningInventoryReport + $"_{ActionView}";
        public const string OpeningInventoryReportPrint = OpeningInventoryReport + $"_{ActionPrint}";
        public const string OpeningInventoryReportExportExcel = OpeningInventoryReport + $"_{ActionExportExcel}";

        public const string IssueTransactionListReport = "IssueTransactionList_Report";
        public const string IssueTransactionListReportView = IssueTransactionListReport + $"_{ActionView}";
        public const string IssueTransactionListReportPrint = IssueTransactionListReport + $"_{ActionPrint}";
        public const string IssueTransactionListReportExportExcel = IssueTransactionListReport + $"_{ActionExportExcel}";

        public const string SaleDetailBookReport = "SaleDetailBook_Report";
        public const string SaleDetailBookReportView = SaleDetailBookReport + $"_{ActionView}";
        public const string SaleDetailBookReportPrint = SaleDetailBookReport + $"_{ActionPrint}";
        public const string SaleDetailBookReportExportExcel = SaleDetailBookReport + $"_{ActionExportExcel}";

        public const string SummaryPurchaseReport = "SummaryPurchase_Report";
        public const string SummaryPurchaseReportView = SummaryPurchaseReport + $"_{ActionView}";
        public const string SummaryPurchaseReportPrint = SummaryPurchaseReport + $"_{ActionPrint}";
        public const string SummaryPurchaseReportExportExcel = SummaryPurchaseReport + $"_{ActionExportExcel}";

        public const string ReceiptTransactionListReport = "ReceiptTransactionList_Report";
        public const string ReceiptTransactionListReportView = ReceiptTransactionListReport + $"_{ActionView}";
        public const string ReceiptTransactionListReportPrint = ReceiptTransactionListReport + $"_{ActionPrint}";
        public const string ReceiptTransactionListReportExportExcel = ReceiptTransactionListReport + $"_{ActionExportExcel}";

        public const string SalesTransactionListReport = "SalesTransactionList_Report";
        public const string SalesTransactionListReportView = SalesTransactionListReport + $"_{ActionView}";
        public const string SalesTransactionListReportPrint = SalesTransactionListReport + $"_{ActionPrint}";
        public const string SalesTransactionListReportExportExcel = SalesTransactionListReport + $"_{ActionExportExcel}";

        public const string ReturnToSupplierListReport = "ReturnToSupplierList_Report";
        public const string ReturnToSupplierListReportView = ReturnToSupplierListReport + $"_{ActionView}";
        public const string ReturnToSupplierListReportPrint = ReturnToSupplierListReport + $"_{ActionPrint}";
        public const string ReturnToSupplierListReportExportExcel = ReturnToSupplierListReport + $"_{ActionExportExcel}";

        public const string PurchaseImportTaxReport = "PurchaseImportTax_Report";
        public const string PurchaseImportTaxReportView = PurchaseImportTaxReport + $"_{ActionView}";
        public const string PurchaseImportTaxReportPrint = PurchaseImportTaxReport + $"_{ActionPrint}";
        public const string PurchaseImportTaxReportExportExcel = PurchaseImportTaxReport + $"_{ActionExportExcel}";

        public const string ExportSummaryReport = "ExportSummary_Report";
        public const string ExportSummaryReportView = ExportSummaryReport + $"_{ActionView}";
        public const string ExportSummaryReportPrint = ExportSummaryReport + $"_{ActionPrint}";
        public const string ExportSummaryReportExportExcel = ExportSummaryReport + $"_{ActionExportExcel}";

        public const string IssueTransactionListMultiReport = "IssueTransListMulti_Report";
        public const string IssueTransactionListMultiReportView = IssueTransactionListMultiReport + $"_{ActionView}";
        public const string IssueTransactionListMultiReportPrint = IssueTransactionListMultiReport + $"_{ActionPrint}";
        public const string IssueTransactionListMultiReportExportExcel = IssueTransactionListMultiReport + $"_{ActionExportExcel}";

        public const string SalesTransactionListMultiReport = "SalesTransListMulti_Report";
        public const string SalesTransactionListMultiReportView = SalesTransactionListMultiReport + $"_{ActionView}";
        public const string SalesTransactionListMultiReportPrint = SalesTransactionListMultiReport + $"_{ActionPrint}";
        public const string SalesTransactionListMultiReportExportExcel = SalesTransactionListMultiReport + $"_{ActionExportExcel}";

        public const string SummaryPurchaseMultiReport = "SummaryPurchaseMulti_Report";
        public const string SummaryPurchaseMultiReportView = SummaryPurchaseMultiReport + $"_{ActionView}";
        public const string SummaryPurchaseMultiReportPrint = SummaryPurchaseMultiReport + $"_{ActionPrint}";
        public const string SummaryPurchaseMultiReportExportExcel = SummaryPurchaseMultiReport + $"_{ActionExportExcel}";

        public const string SummaryExportMultiReport = "SummaryExportMulti_Report";
        public const string SummaryExportMultiReportView = SummaryExportMultiReport + $"_{ActionView}";
        public const string SummaryExportMultiReportPrint = SummaryExportMultiReport + $"_{ActionPrint}";
        public const string SummaryExportMultiReportExportExcel = SummaryExportMultiReport + $"_{ActionExportExcel}";

        public const string SalesReturnReport = "SalesReturn_Report";
        public const string SalesReturnReportView = SalesReturnReport + $"_{ActionView}";
        public const string SalesReturnReportPrint = SalesReturnReport + $"_{ActionPrint}";
        public const string SalesReturnReportExportExcel = SalesReturnReport + $"_{ActionExportExcel}";

        public const string ListProductVoucherReport = "ListProductVoucher_Report";
        public const string ListProductVoucherReportView = ListProductVoucherReport + $"_{ActionView}";
        public const string ListProductVoucherReportPrint = ListProductVoucherReport + $"_{ActionPrint}";
        public const string ListProductVoucherReportExportExcel = ListProductVoucherReport + $"_{ActionExportExcel}";

        public const string PurchaseWithTaxListReport = "PurchaseWithTaxList_Report";
        public const string PurchaseWithTaxListReportView = PurchaseWithTaxListReport + $"_{ActionView}";
        public const string PurchaseWithTaxListReportPrint = PurchaseWithTaxListReport + $"_{ActionPrint}";
        public const string PurchaseWithTaxListReportExportExcel = PurchaseWithTaxListReport + $"_{ActionExportExcel}";

        public const string DirectImportExportListReport = "DirectImportExportList_Report";
        public const string DirectImportExportListReportView = DirectImportExportListReport + $"_{ActionView}";
        public const string DirectImportExportListReportPrint = DirectImportExportListReport + $"_{ActionPrint}";
        public const string DirectImportExportListReportExportExcel = DirectImportExportListReport + $"_{ActionExportExcel}";

        public const string SummarySalesReport = "SummarySales_Report";
        public const string SummarySalesReportView = SummarySalesReport + $"_{ActionView}";
        public const string SummarySalesReportPrint = SummarySalesReport + $"_{ActionPrint}";
        public const string SummarySalesReportExportExcel = SummarySalesReport + $"_{ActionExportExcel}";

        public const string SalesAnalysisReport = "SalesAnalysis_Report";
        public const string SalesAnalysisReportView = SalesAnalysisReport + $"_{ActionView}";
        public const string SalesAnalysisReportPrint = SalesAnalysisReport + $"_{ActionPrint}";
        public const string SalesAnalysisReportExportExcel = SalesAnalysisReport + $"_{ActionExportExcel}";

        public const string SummarySalesMultiReport = "SummarySalesMulti_Report";
        public const string SummarySalesMultiReportView = SummarySalesMultiReport + $"_{ActionView}";
        public const string SummarySalesMultiReportPrint = SummarySalesMultiReport + $"_{ActionPrint}";
        public const string SummarySalesMultiReportExportExcel = SummarySalesMultiReport + $"_{ActionExportExcel}";

        public const string ListPurchaseMultiReport = "ListPurchaseMulti_Report";
        public const string ListPurchaseMultiReportView = ListPurchaseMultiReport + $"_{ActionView}";
        public const string ListPurchaseMultiReportPrint = ListPurchaseMultiReport + $"_{ActionPrint}";
        public const string ListPurchaseMultiReportExportExcel = ListPurchaseMultiReport + $"_{ActionExportExcel}";

        public const string StockCardReport = "StockCard_Report";
        public const string StockCardReportView = StockCardReport + $"_{ActionView}";
        public const string StockCardReportPrint = StockCardReport + $"_{ActionPrint}";
        public const string StockCardReportExportExcel = StockCardReport + $"_{ActionExportExcel}";

        public const string SalesOrderTrackingReport = "SalesOrderTracking_Report";
        public const string SalesOrderTrackingReportView = SalesOrderTrackingReport + $"_{ActionView}";
        public const string SalesOrderTrackingReportPrint = SalesOrderTrackingReport + $"_{ActionPrint}";
        public const string SalesOrderTrackingReportExportExcel = SalesOrderTrackingReport + $"_{ActionExportExcel}";

        public const string PurchaseOrderTrackingReport = "PurchaseOrderTracking_Report";
        public const string PurchaseOrderTrackingReportView = PurchaseOrderTrackingReport + $"_{ActionView}";
        public const string PurchaseOrderTrackingReportPrint = PurchaseOrderTrackingReport + $"_{ActionPrint}";
        public const string PurchaseOrderTrackingReportExportExcel = PurchaseOrderTrackingReport + $"_{ActionExportExcel}";

        //Báo cáo chứng từ ghi sổ
        public const string LedgerRecordingVoucherReport = "LedgerRecordingVoucher_Report";
        public const string LedgerRecordingVoucherReportView = LedgerRecordingVoucherReport + $"_{ActionView}";
        public const string LedgerRecordingVoucherReportPrint = LedgerRecordingVoucherReport + $"_{ActionPrint}";
        public const string LedgerRecordingVoucherReportExportExcel = LedgerRecordingVoucherReport + $"_{ActionExportExcel}";

        public const string AccountDetailBookRVReport = "AccountDetailBookRV_Report";
        public const string AccountDetailBookRVReportView = AccountDetailBookRVReport + $"_{ActionView}";
        public const string AccountDetailBookRVReportPrint = AccountDetailBookRVReport + $"_{ActionPrint}";
        public const string AccountDetailBookRVReportExportExcel = AccountDetailBookRVReport + $"_{ActionExportExcel}";

        public const string RecordingVoucherRegisterBookReport = "RecordingVoucherRegBook_Report";
        public const string RecordingVoucherRegisterBookReportView = RecordingVoucherRegisterBookReport + $"_{ActionView}";
        public const string RecordingVoucherRegisterBookReportPrint = RecordingVoucherRegisterBookReport + $"_{ActionPrint}";
        public const string RecordingVoucherRegisterBookReportExportExcel = RecordingVoucherRegisterBookReport + $"_{ActionExportExcel}";

        public const string RecordingVoucherReport = "RecordingVoucher_Report";
        public const string RecordingVoucherReportView = RecordingVoucherReport + $"_{ActionView}";
        public const string RecordingVoucherReportPrint = RecordingVoucherReport + $"_{ActionPrint}";
        public const string RecordingVoucherReportExportExcel = RecordingVoucherReport + $"_{ActionExportExcel}";

        //Báo cáo công nợ
        public const string DebtBalanceSheetReport = "DebtBalanceSheet_Report";
        public const string DebtBalanceSheetReportView = DebtBalanceSheetReport + $"_{ActionView}";
        public const string DebtBalanceSheetReportPrint = DebtBalanceSheetReport + $"_{ActionPrint}";
        public const string DebtBalanceSheetReportExportExcel = DebtBalanceSheetReport + $"_{ActionExportExcel}";

        public const string DebtDetailBookReport = "DebtDetailBook_Report";
        public const string DebtDetailBookReportView = DebtDetailBookReport + $"_{ActionView}";
        public const string DebtDetailBookReportPrint = DebtDetailBookReport + $"_{ActionPrint}";
        public const string DebtDetailBookReportExportExcel = DebtDetailBookReport + $"_{ActionExportExcel}";

        public const string DebtClosingBalanceReport = "DebtClosingBalance_Report";
        public const string DebtClosingBalanceReportView = DebtClosingBalanceReport + $"_{ActionView}";
        public const string DebtClosingBalanceReportPrint = DebtClosingBalanceReport + $"_{ActionPrint}";
        public const string DebtClosingBalanceReportExportExcel = DebtClosingBalanceReport + $"_{ActionExportExcel}";

        public const string DebtOpeningBalanceReport = "DebtOpeningBalance_Report";
        public const string DebtOpeningBalanceReportView = DebtOpeningBalanceReport + $"_{ActionView}";
        public const string DebtOpeningBalanceReportPrint = DebtOpeningBalanceReport + $"_{ActionPrint}";
        public const string DebtOpeningBalanceReportExportExcel = DebtOpeningBalanceReport + $"_{ActionExportExcel}";

        public const string DebtProductDetailReport = "DebtProductDetail_Report";
        public const string DebtProductDetailReportView = DebtProductDetailReport + $"_{ActionView}";
        public const string DebtProductDetailReportPrint = DebtProductDetailReport + $"_{ActionPrint}";
        public const string DebtProductDetailReportExportExcel = DebtProductDetailReport + $"_{ActionExportExcel}";

        public const string DebtCompareConfirmReport = "DebtCompareConfirm_Report";
        public const string DebtCompareConfirmReportView = DebtCompareConfirmReport + $"_{ActionView}";
        public const string DebtCompareConfirmReportPrint = DebtCompareConfirmReport + $"_{ActionPrint}";
        public const string DebtCompareConfirmReportExportExcel = DebtCompareConfirmReport + $"_{ActionExportExcel}";

        public const string DetailDebtInvoiceReport = "DetailDebtInvoice_Report";
        public const string DetailDebtInvoiceReportView = DetailDebtInvoiceReport + $"_{ActionView}";
        public const string DetailDebtInvoiceReportPrint = DetailDebtInvoiceReport + $"_{ActionPrint}";
        public const string DetailDebtInvoiceReportExportExcel = DetailDebtInvoiceReport + $"_{ActionExportExcel}";

        public const string SummaryDebtInvoiceReport = "SummaryDebtInvoice_Report";
        public const string SummaryDebtInvoiceReportView = SummaryDebtInvoiceReport + $"_{ActionView}";
        public const string SummaryDebtInvoiceReportPrint = SummaryDebtInvoiceReport + $"_{ActionPrint}";
        public const string SummaryDebtInvoiceReportExportExcel = SummaryDebtInvoiceReport + $"_{ActionExportExcel}";

        public const string DebtAccordingInvoiceReport = "DebtAccordingInvoice_Report";
        public const string DebtAccordingInvoiceReportView = DebtAccordingInvoiceReport + $"_{ActionView}";
        public const string DebtAccordingInvoiceReportPrint = DebtAccordingInvoiceReport + $"_{ActionPrint}";
        public const string DebtAccordingInvoiceReportExportExcel = DebtAccordingInvoiceReport + $"_{ActionExportExcel}";

        public const string VoucherPaymentBookReport = "VoucherPaymentBook_Report";
        public const string VoucherPaymentBookReportView = VoucherPaymentBookReport + $"_{ActionView}";
        public const string VoucherPaymentBookReportPrint = VoucherPaymentBookReport + $"_{ActionPrint}";
        public const string VoucherPaymentBookReportExportExcel = VoucherPaymentBookReport + $"_{ActionExportExcel}";

        //Báo cáo tài chính
        public const string AccBalanceSheetReport = "AccBalanceSheet_Report";
        public const string AccBalanceSheetReportView = AccBalanceSheetReport + $"_{ActionView}";
        public const string AccBalanceSheetReportPrint = AccBalanceSheetReport + $"_{ActionPrint}";
        public const string AccBalanceSheetReportExportExcel = AccBalanceSheetReport + $"_{ActionExportExcel}";

        public const string CashFlowReport = "CashFlow_Report";
        public const string CashFlowReportView = CashFlowReport + $"_{ActionView}";
        public const string CashFlowReportPrint = CashFlowReport + $"_{ActionPrint}";
        public const string CashFlowReportExportExcel = CashFlowReport + $"_{ActionExportExcel}";

        public const string BusinessResultReport = "BusinessResult_Report";
        public const string BusinessResultReportView = BusinessResultReport + $"_{ActionView}";
        public const string BusinessResultReportPrint = BusinessResultReport + $"_{ActionPrint}";
        public const string BusinessResultReportExportExcel = BusinessResultReport + $"_{ActionExportExcel}";

        public const string FinancialStatementReport = "FinancialStatement_Report";
        public const string FinancialStatementReportView = FinancialStatementReport + $"_{ActionView}";
        public const string FinancialStatementReportPrint = FinancialStatementReport + $"_{ActionPrint}";
        public const string FinancialStatementReportExportExcel = FinancialStatementReport + $"_{ActionExportExcel}";

        public const string FinancialStatement133Report = "FinancialStatement133_Report";
        public const string FinancialStatement133ReportView = FinancialStatement133Report + $"_{ActionView}";
        public const string FinancialStatement133ReportPrint = FinancialStatement133Report + $"_{ActionPrint}";
        public const string FinancialStatement133ReportExportExcel = FinancialStatement133Report + $"_{ActionExportExcel}";

        //Báo cáo tài sản
        public const string AssetBookReport = "AssetBook_Report";
        public const string AssetBookReportView = AssetBookReport + $"_{ActionView}";
        public const string AssetBookReportPrint = AssetBookReport + $"_{ActionPrint}";
        public const string AssetBookReportExportExcel = AssetBookReport + $"_{ActionExportExcel}";

        public const string DepreciationAssetTotalReport = "DepreciationAssetTotal_Report";
        public const string DepreciationAssetTotalReportView = DepreciationAssetTotalReport + $"_{ActionView}";
        public const string DepreciationAssetTotalReportPrint = DepreciationAssetTotalReport + $"_{ActionPrint}";
        public const string DepreciationAssetTotalReportExportExcel = DepreciationAssetTotalReport + $"_{ActionExportExcel}";

        public const string DepreciationAssetCapitalReport = "DepreciationAssetCap_Report";
        public const string DepreciationAssetCapitalReportView = DepreciationAssetCapitalReport + $"_{ActionView}";
        public const string DepreciationAssetCapitalReportPrint = DepreciationAssetCapitalReport + $"_{ActionPrint}";
        public const string DepreciationAssetCapitalReportExportExcel = DepreciationAssetCapitalReport + $"_{ActionExportExcel}";

        public const string DepreciationAssetMonthReport = "DepreciationAssetMonth_Report";
        public const string DepreciationAssetMonthReportView = DepreciationAssetMonthReport + $"_{ActionView}";
        public const string DepreciationAssetMonthReportPrint = DepreciationAssetMonthReport + $"_{ActionPrint}";
        public const string DepreciationAssetMonthReportExportExcel = DepreciationAssetMonthReport + $"_{ActionExportExcel}";

        public const string AssetAllocateListReport = "AssetAllocateList_Report";
        public const string AssetAllocateListReportView = AssetAllocateListReport + $"_{ActionView}";
        public const string AssetAllocateListReportPrint = AssetAllocateListReport + $"_{ActionPrint}";
        public const string AssetAllocateListReportExportExcel = AssetAllocateListReport + $"_{ActionExportExcel}";

        public const string AssetCardReport = "AssetCard_Report";
        public const string AssetCardReportView = AssetCardReport + $"_{ActionView}";
        public const string AssetCardReportPrint = AssetCardReport + $"_{ActionPrint}";
        public const string AssetCardReportExportExcel = AssetCardReport + $"_{ActionExportExcel}";

        public const string AssetUpDownReport = "AssetUpDown_Report";
        public const string AssetUpDownReportView = AssetUpDownReport + $"_{ActionView}";
        public const string AssetUpDownReportPrint = AssetUpDownReport + $"_{ActionPrint}";
        public const string AssetUpDownReportExportExcel = AssetUpDownReport + $"_{ActionExportExcel}";

        //Báo cáo công cụ
        public const string ToolBookReport = "ToolBook_Report";
        public const string ToolBookReportView = ToolBookReport + $"_{ActionView}";
        public const string ToolBookReportPrint = ToolBookReport + $"_{ActionPrint}";
        public const string ToolBookReportExportExcel = ToolBookReport + $"_{ActionExportExcel}";

        public const string ToolAllocateListReport = "ToolAllocateList_Report";
        public const string ToolAllocateListReportView = ToolAllocateListReport + $"_{ActionView}";
        public const string ToolAllocateListReportPrint = ToolAllocateListReport + $"_{ActionPrint}";
        public const string ToolAllocateListReportExportExcel = ToolAllocateListReport + $"_{ActionExportExcel}";

        public const string DepreciationToolTotalReport = "DepreciationToolTotal_Report";
        public const string DepreciationToolTotalReportView = DepreciationToolTotalReport + $"_{ActionView}";
        public const string DepreciationToolTotalReportPrint = DepreciationToolTotalReport + $"_{ActionPrint}";
        public const string DepreciationToolTotalReportExportExcel = DepreciationToolTotalReport + $"_{ActionExportExcel}";

        public const string DepreciationToolCapitalReport = "DepreciationToolCap_Report";
        public const string DepreciationToolCapitalReportView = DepreciationToolCapitalReport + $"_{ActionView}";
        public const string DepreciationToolCapitalReportPrint = DepreciationToolCapitalReport + $"_{ActionPrint}";
        public const string DepreciationToolCapitalReportExportExcel = DepreciationToolCapitalReport + $"_{ActionExportExcel}";

        public const string DepreciationToolMonthReport = "DepreciationToolMonth_Report";
        public const string DepreciationToolMonthReportView = DepreciationToolMonthReport + $"_{ActionView}";
        public const string DepreciationToolMonthReportPrint = DepreciationToolMonthReport + $"_{ActionPrint}";
        public const string DepreciationToolMonthReportExportExcel = DepreciationToolMonthReport + $"_{ActionExportExcel}";

        public const string ToolCardReport = "ToolCard_Report";
        public const string ToolCardReportView = ToolCardReport + $"_{ActionView}";
        public const string ToolCardReportPrint = ToolCardReport + $"_{ActionPrint}";
        public const string ToolCardReportExportExcel = ToolCardReport + $"_{ActionExportExcel}";

        public const string ToolUpDownReport = "ToolUpDown_Report";
        public const string ToolUpDownReportView = ToolUpDownReport + $"_{ActionView}";
        public const string ToolUpDownReportPrint = ToolUpDownReport + $"_{ActionPrint}";
        public const string ToolUpDownReportExportExcel = ToolUpDownReport + $"_{ActionExportExcel}";

        //Báo cáo thuế
        public const string SalesTaxListReport = "SalesTaxList_Report";
        public const string SalesTaxListReportView = SalesTaxListReport + $"_{ActionView}";
        public const string SalesTaxListReportPrint = SalesTaxListReport + $"_{ActionPrint}";
        public const string SalesTaxListReportExportExcel = SalesTaxListReport + $"_{ActionExportExcel}";

        public const string PurchaseTaxListReport = "PurchaseTaxList_Report";
        public const string PurchaseTaxListReportView = PurchaseTaxListReport + $"_{ActionView}";
        public const string PurchaseTaxListReportPrint = PurchaseTaxListReport + $"_{ActionPrint}";
        public const string PurchaseTaxListReportExportExcel = PurchaseTaxListReport + $"_{ActionExportExcel}";

        public const string VatStatementReport = "VatStatement_Report";
        public const string VatStatementReportView = VatStatementReport + $"_{ActionView}";
        public const string VatStatementReportPrint = VatStatementReport + $"_{ActionPrint}";
        public const string VatStatementReportExportExcel = VatStatementReport + $"_{ActionExportExcel}";

        public const string ExciseTaxSalesListReport = "ExciseTaxSalesList_Report";
        public const string ExciseTaxSalesListReportView = ExciseTaxSalesListReport + $"_{ActionView}";
        public const string ExciseTaxSalesListReportPrint = ExciseTaxSalesListReport + $"_{ActionPrint}";
        public const string ExciseTaxSalesListReportExportExcel = ExciseTaxSalesListReport + $"_{ActionExportExcel}";

        public const string DeclarationExciseTaxReport = "DeclarationExciseTax_Report";
        public const string DeclarationExciseTaxReportView = DeclarationExciseTaxReport + $"_{ActionView}";
        public const string DeclarationExciseTaxReportPrint = DeclarationExciseTaxReport + $"_{ActionPrint}";
        public const string DeclarationExciseTaxReportExportExcel = DeclarationExciseTaxReport + $"_{ActionExportExcel}";

        public const string VatDirectStatementReport = "VatDirectStatement_Report";
        public const string VatDirectStatementReportView = VatDirectStatementReport + $"_{ActionView}";
        public const string VatDirectStatementReportPrint = VatDirectStatementReport + $"_{ActionPrint}";
        public const string VatDirectStatementReportExportExcel = VatDirectStatementReport + $"_{ActionExportExcel}";

        public const string ReducingVatQH15Report = "ReducingVatQH15_Report";
        public const string ReducingVatQH15ReportView = ReducingVatQH15Report + $"_{ActionView}";
        public const string ReducingVatQH15ReportPrint = ReducingVatQH15Report + $"_{ActionPrint}";
        public const string ReducingVatQH15ReportExportExcel = ReducingVatQH15Report + $"_{ActionExportExcel}";

        //Báo cáo chi phí giá thành
        public const string ProductQuotaReport = "ProductQuota_Report";
        public const string ProductQuotaReportView = ProductQuotaReport + $"_{ActionView}";
        public const string ProductQuotaReportPrint = ProductQuotaReport + $"_{ActionPrint}";
        public const string ProductQuotaReportExportExcel = ProductQuotaReport + $"_{ActionExportExcel}";

        public const string WorkPriceCardReport = "WorkPriceCard_Report";
        public const string WorkPriceCardReportView = WorkPriceCardReport + $"_{ActionView}";
        public const string WorkPriceCardReportPrint = WorkPriceCardReport + $"_{ActionPrint}";
        public const string WorkPriceCardReportExportExcel = WorkPriceCardReport + $"_{ActionExportExcel}";

        public const string ProductionCostCardReport = "ProductionCostCard_Report";
        public const string ProductionCostCardReportView = ProductionCostCardReport + $"_{ActionView}";
        public const string ProductionCostCardReportPrint = ProductionCostCardReport + $"_{ActionPrint}";
        public const string ProductionCostCardReportExportExcel = ProductionCostCardReport + $"_{ActionExportExcel}";

        public const string ProductionCostBookReport = "ProductionCostBook_Report";
        public const string ProductionCostBookReportView = ProductionCostBookReport + $"_{ActionView}";
        public const string ProductionCostBookReportPrint = ProductionCostBookReport + $"_{ActionPrint}";
        public const string ProductionCostBookReportExportExcel = ProductionCostBookReport + $"_{ActionExportExcel}";

        public const string WorkPriceBookReport = "WorkPriceBook_Report";
        public const string WorkPriceBookReportView = WorkPriceBookReport + $"_{ActionView}";
        public const string WorkPriceBookReportPrint = WorkPriceBookReport + $"_{ActionPrint}";
        public const string WorkPriceBookReportExportExcel = WorkPriceBookReport + $"_{ActionExportExcel}";

        public const string ProductionCostReport = "ProductionCost_Report";
        public const string ProductionCostReportView = ProductionCostReport + $"_{ActionView}";
        public const string ProductionCostReportPrint = ProductionCostReport + $"_{ActionPrint}";
        public const string ProductionCostReportExportExcel = ProductionCostReport + $"_{ActionExportExcel}";

        public const string ContructionCostReport = "ContructionCost_Report";
        public const string ContructionCostReportView = ContructionCostReport + $"_{ActionView}";
        public const string ContructionCostReportPrint = ContructionCostReport + $"_{ActionPrint}";
        public const string ContructionCostReportExportExcel = ContructionCostReport + $"_{ActionExportExcel}";

        public const string CashBookHkdReport = "CashBook_Hkd_Report";
        public const string CashBookHkdReportView = CashBookHkdReport + $"_{ActionView}";
        public const string CashBookHkdReportPrint = CashBookHkdReport + $"_{ActionPrint}";
        public const string CashBookHkdReportExportExcel = CashBookHkdReport + $"_{ActionExportExcel}";

        public const string TotalRevenueHkdReport = "TotalRevenue_Hkd_Report";
        public const string TotalRevenueHkdReportView = TotalRevenueHkdReport + $"_{ActionView}";
        public const string TotalRevenueHkdReportPrint = TotalRevenueHkdReport + $"_{ActionPrint}";
        public const string TotalRevenueHkdReportExportExcel = TotalRevenueHkdReport + $"_{ActionExportExcel}";

        public const string DetailRevenueHkdReport = "DetailRevenue_Hkd_Report";
        public const string DetailRevenueHkdReportView = DetailRevenueHkdReport + $"_{ActionView}";
        public const string DetailRevenueHkdReportPrint = DetailRevenueHkdReport + $"_{ActionPrint}";
        public const string DetailRevenueHkdReportExportExcel = DetailRevenueHkdReport + $"_{ActionExportExcel}";

        public const string SummaryCostsHkdReport = "SummaryCosts_Hkd_Report";
        public const string SummaryCostHkdReportView = SummaryCostsHkdReport + $"_{ActionView}";
        public const string SummaryCostsHkdReportPrint = SummaryCostsHkdReport + $"_{ActionPrint}";
        public const string SummaryCostsHkdReportExportExcel = SummaryCostsHkdReport + $"_{ActionExportExcel}";

        public const string BookCostsHkdReport = "BookCosts_Hkd_Report";
        public const string BookCostHkdReportView = BookCostsHkdReport + $"_{ActionView}";
        public const string BookCostsHkdReportPrint = BookCostsHkdReport + $"_{ActionPrint}";
        public const string BookCostsHkdReportExportExcel = BookCostsHkdReport + $"_{ActionExportExcel}";

        public const string ReducingVatHkdReport = "ReducingVat_Hkd_Report";
        public const string ReducingVatHkdReportView = ReducingVatHkdReport + $"_{ActionView}";
        public const string ReducingVatHkdReportPrint = ReducingVatHkdReport + $"_{ActionPrint}";
        public const string ReducingVatHkdReportExportExcel = ReducingVatHkdReport + $"_{ActionExportExcel}";

        public const string SalarySheetHkdReport = "SalarySheet_Hkd_Report";
        public const string SalarySheetHkdReportView = SalarySheetHkdReport + $"_{ActionView}";
        public const string SalarySheetHkdReportPrint = SalarySheetHkdReport + $"_{ActionPrint}";
        public const string SalarySheetHkdReportExportExcel = SalarySheetHkdReport + $"_{ActionExportExcel}";

        public const string ImportExportHkdReport = "ImportExport_Hkd_Report";
        public const string ImportExportHkdReportView = ImportExportHkdReport + $"_{ActionView}";
        public const string ImportExportHkdReportPrint = ImportExportHkdReport + $"_{ActionPrint}";
        public const string ImportExportHkdReportExportExcel = ImportExportHkdReport + $"_{ActionExportExcel}";

        public const string DetailInventoryHkdReport = "DetailInventory_Hkd_Report";
        public const string DetailInventoryHkdReportView = DetailInventoryHkdReport + $"_{ActionView}";
        public const string DetailInventoryHkdReportPrint = DetailInventoryHkdReport + $"_{ActionPrint}";
        public const string DetailInventoryHkdReportExportExcel = DetailInventoryHkdReport + $"_{ActionExportExcel}";

        public const string CashInBankHkdReport = "CashInBank_Hkd_Report";
        public const string CashInBankHkdReportView = CashInBankHkdReport + $"_{ActionView}";
        public const string CashInBankHkdReportPrint = CashInBankHkdReport + $"_{ActionPrint}";
        public const string CashInBankHkdReportExportExcel = CashInBankHkdReport + $"_{ActionExportExcel}";

        public const string DebtBalanceHkdReport = "DebtBalance_Hkd_Report";
        public const string DebtBalanceHkdReportView = DebtBalanceHkdReport + $"_{ActionView}";
        public const string DebtBalanceHkdReportPrint = DebtBalanceHkdReport + $"_{ActionPrint}";
        public const string DebtBalanceHkdReportExportExcel = DebtBalanceHkdReport + $"_{ActionExportExcel}";

        public const string DebtDetailHkdReport = "DebtDetail_Hkd_Report";
        public const string DebtDetailHkdReportView = DebtDetailHkdReport + $"_{ActionView}";
        public const string DebtDetailHkdReportPrint = DebtDetailHkdReport + $"_{ActionPrint}";
        public const string DebtDetailHkdReportExportExcel = DebtDetailHkdReport + $"_{ActionExportExcel}";

        public const string TaxObligationHkdReport = "TaxObligation_Hkd_Report";
        public const string TaxObligationHkdReportView = TaxObligationHkdReport + $"_{ActionView}";
        public const string TaxObligationHkdReportPrint = TaxObligationHkdReport + $"_{ActionPrint}";
        public const string TaxObligationHkdReportExportExcel = TaxObligationHkdReport + $"_{ActionExportExcel}";

        public const string SummaryAccBalanceSheetReport = "SummaryAccBalanceSheet_Report";
        public const string SummaryAccBalanceSheetReportView = SummaryAccBalanceSheetReport + $"_{ActionView}";
        public const string SummaryAccBalanceSheetReportPrint = SummaryAccBalanceSheetReport + $"_{ActionPrint}";
        public const string SummaryAccBalanceSheetReportExportExcel = SummaryAccBalanceSheetReport + $"_{ActionExportExcel}";

        public const string SummaryCashFlowReport = "SummaryCashFlow_Report";
        public const string SummaryCashFlowReportView = SummaryCashFlowReport + $"_{ActionView}";
        public const string SummaryCashFlowReportPrint = SummaryCashFlowReport + $"_{ActionPrint}";
        public const string SummaryCashFlowReportExportExcel = SummaryCashFlowReport + $"_{ActionExportExcel}";

        public const string SummaryBusinessResultReport = "SummaryBusinessResult_Report";
        public const string SummaryBusinessResultReportView = SummaryBusinessResultReport + $"_{ActionView}";
        public const string SummaryBusinessResultReportPrint = SummaryBusinessResultReport + $"_{ActionPrint}";
        public const string SummaryBusinessResultReportExportExcel = SummaryBusinessResultReport + $"_{ActionExportExcel}";

        public const string SummaryBalanceSheetReport = "SummaryBalanceSheet_Report";
        public const string SummaryBalanceSheetReportView = SummaryBalanceSheetReport + $"_{ActionView}";
        public const string SummaryBalanceSheetReportPrint = SummaryBalanceSheetReport + $"_{ActionPrint}";
        public const string SummaryBalanceSheetReportExportExcel = SummaryBalanceSheetReport + $"_{ActionExportExcel}";

        public const string PartnerCategoryReport = "PartnerCategory_Report";
        public const string PartnerCategoryReportView = PartnerCategoryReport + $"_{ActionView}";
        public const string PartnerCategoryReportPrint = PartnerCategoryReport + $"_{ActionPrint}";
        public const string PartnerCategoryReportExportExcel = PartnerCategoryReport + $"_{ActionExportExcel}";

        public const string ProductCategoryReport = "ProductCategory_Report";
        public const string ProductCategoryReportView = ProductCategoryReport + $"_{ActionView}";
        public const string ProductCategoryReportPrint = ProductCategoryReport + $"_{ActionPrint}";
        public const string ProductCategoryReportExportExcel = ProductCategoryReport + $"_{ActionExportExcel}";

        public const string SalesTaxDirectListReport = "SalesTaxDirectList_Report";
        public const string SalesTaxDirectListReportView = SalesTaxDirectListReport + $"_{ActionView}";
        public const string SalesTaxDirectListReportPrint = SalesTaxDirectListReport + $"_{ActionPrint}";
        public const string SalesTaxDirectListReportExportExcel = SalesTaxDirectListReport + $"_{ActionExportExcel}";

        public const string ContractPerformanceReport = "ContractPerformance_Report";
        public const string ContractPerformanceReportView = ContractPerformanceReport + $"_{ActionView}";
        public const string ContractPerformanceReportPrint = ContractPerformanceReport + $"_{ActionPrint}";
        public const string ContractPerformanceReportExportExcel = ContractPerformanceReport + $"_{ActionExportExcel}";
    }
}
