using Accounting.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Accounting.Permissions
{
    public class ReportPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var group = context.AddGroup(ReportPermissions.GroupName,
                                        G(ReportPermissions.GroupName),
                                        MultiTenancySides.Tenant);

            ConfigGeneralDiaryReportGroup(group);
            ConfigFinacialReportGroup(group);
            ConfigImportExportGroup(group);
            ConfigBookRecordingGroup(group);
            ConfigDebtGroup(group);
            ConfigAssetGroup(group);
            ConfigToolGroup(group);
            ConfigCostGroup(group);
            ConfigTaxGroup(group);
            ConfigSummaryGroup(group);
            ConfigHkdGroup(group);
            ConfigVoucherReportGroup(group);
            // ConfigCategoryReportGroup(group);
        }
        // private void ConfigCategoryReportGroup(PermissionGroupDefinition group)
        // {
        //     var categoryGroup = group.AddPermission(ReportPermissions.CategoryGroup,
        //         G(ReportPermissions.CategoryGroup), MultiTenancySides.Tenant);
        //
        //     var report = categoryGroup.AddChild(ReportPermissions.PartnerCategoryReport,
        //         G(ReportPermissions.PartnerCategoryReport), MultiTenancySides.Tenant);
        //     report.AddChild(ReportPermissions.PartnerCategoryReportView,
        //             G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
        //     report.AddChild(ReportPermissions.PartnerCategoryReportPrint,
        //             G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
        //     report.AddChild(ReportPermissions.PartnerCategoryReportExportExcel,
        //             G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        //
        //     report = categoryGroup.AddChild(ReportPermissions.ProductCategoryReport,
        //         G(ReportPermissions.ProductCategoryReport), MultiTenancySides.Tenant);
        //     report.AddChild(ReportPermissions.ProductCategoryReportView,
        //             G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
        //     report.AddChild(ReportPermissions.ProductCategoryReportPrint,
        //             G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
        //     report.AddChild(ReportPermissions.ProductCategoryReportExportExcel,
        //             G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        // }
        private void ConfigVoucherReportGroup(PermissionGroupDefinition group)
        {
            var voucherGroup = group.AddPermission(ReportPermissions.VoucherGroup,
                G(ReportPermissions.VoucherGroup), MultiTenancySides.Tenant);

            var listAccVoucherReport = voucherGroup.AddChild(ReportPermissions.ListAccVoucherReport,
                G(ReportPermissions.ListAccVoucherReport), MultiTenancySides.Tenant);
            listAccVoucherReport.AddChild(ReportPermissions.ListAccVoucherReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            listAccVoucherReport.AddChild(ReportPermissions.ListAccVoucherReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            listAccVoucherReport.AddChild(ReportPermissions.ListAccVoucherReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var listProductVoucherReport = voucherGroup.AddChild(ReportPermissions.ListProductVoucherReport,
                G(ReportPermissions.ListProductVoucherReport), MultiTenancySides.Tenant);
            listProductVoucherReport.AddChild(ReportPermissions.ListProductVoucherReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            listProductVoucherReport.AddChild(ReportPermissions.ListProductVoucherReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            listProductVoucherReport.AddChild(ReportPermissions.ListProductVoucherReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }
        private void ConfigGeneralDiaryReportGroup(PermissionGroupDefinition group)
        {
            var generalAccVoucherGroup = group.AddPermission(ReportPermissions.GeneralDiaryGroup,
                G(ReportPermissions.GeneralDiaryGroup), MultiTenancySides.Tenant);

            var generalDiaryBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.GeneralDiaryBookReport,
                G(ReportPermissions.GeneralDiaryBookReport), MultiTenancySides.Tenant);
            generalDiaryBookReport.AddChild(ReportPermissions.GeneralDiaryBookReportView,
                    G(ReportPermissions.GeneralDiaryBookReportView), MultiTenancySides.Tenant);
            generalDiaryBookReport.AddChild(ReportPermissions.GeneralDiaryBookReportPrint,
                    G(ReportPermissions.GeneralDiaryBookReportPrint), MultiTenancySides.Tenant);
            generalDiaryBookReport.AddChild(ReportPermissions.GeneralDiaryBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var accountDetailBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.AccountDetailBookReport,
                G(ReportPermissions.AccountDetailBookReport), MultiTenancySides.Tenant);
            accountDetailBookReport.AddChild(ReportPermissions.AccountDetailBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            accountDetailBookReport.AddChild(ReportPermissions.AccountDetailBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            accountDetailBookReport.AddChild(ReportPermissions.AccountDetailBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var cashBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.CashBookReport,
                G(ReportPermissions.CashBookReport), MultiTenancySides.Tenant);
            cashBookReport.AddChild(ReportPermissions.CashBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            cashBookReport.AddChild(ReportPermissions.CashBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            cashBookReport.AddChild(ReportPermissions.CashBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var cashInBankBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.CashInBankBookReport,
                G(ReportPermissions.CashInBankBookReport), MultiTenancySides.Tenant);
            cashInBankBookReport.AddChild(ReportPermissions.CashInBankBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            cashInBankBookReport.AddChild(ReportPermissions.CashInBankBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            cashInBankBookReport.AddChild(ReportPermissions.CashInBankBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var ledgerBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.LedgerBookReport,
                G(ReportPermissions.LedgerBookReport), MultiTenancySides.Tenant);
            ledgerBookReport.AddChild(ReportPermissions.LedgerBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            ledgerBookReport.AddChild(ReportPermissions.LedgerBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            ledgerBookReport.AddChild(ReportPermissions.LedgerBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var multiAccountDetailBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.MultiAccountDetailBookReport,
                G(ReportPermissions.MultiAccountDetailBookReport), MultiTenancySides.Tenant);
            multiAccountDetailBookReport.AddChild(ReportPermissions.MultiAccountDetailBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            multiAccountDetailBookReport.AddChild(ReportPermissions.MultiAccountDetailBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            multiAccountDetailBookReport.AddChild(ReportPermissions.MultiAccountDetailBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var multiLedgerBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.MultiLedgerBookReport,
                G(ReportPermissions.MultiLedgerBookReport), MultiTenancySides.Tenant);
            multiLedgerBookReport.AddChild(ReportPermissions.MultiLedgerBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            multiLedgerBookReport.AddChild(ReportPermissions.MultiLedgerBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            multiLedgerBookReport.AddChild(ReportPermissions.MultiLedgerBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var cashCollectionDiaryReport = generalAccVoucherGroup.AddChild(ReportPermissions.CashCollectionDiaryReport,
                G(ReportPermissions.CashCollectionDiaryReport), MultiTenancySides.Tenant);
            cashCollectionDiaryReport.AddChild(ReportPermissions.CashCollectionDiaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            cashCollectionDiaryReport.AddChild(ReportPermissions.CashCollectionDiaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            cashCollectionDiaryReport.AddChild(ReportPermissions.CashCollectionDiaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var cashSpendDiaryReport = generalAccVoucherGroup.AddChild(ReportPermissions.CashSpendDiaryReport,
                G(ReportPermissions.CashSpendDiaryReport), MultiTenancySides.Tenant);
            cashSpendDiaryReport.AddChild(ReportPermissions.CashSpendDiaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            cashSpendDiaryReport.AddChild(ReportPermissions.CashSpendDiaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            cashSpendDiaryReport.AddChild(ReportPermissions.CashSpendDiaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depositCollectionDiaryReport = generalAccVoucherGroup.AddChild(ReportPermissions.DepositCollectionDiaryReport,
                G(ReportPermissions.DepositCollectionDiaryReport), MultiTenancySides.Tenant);
            depositCollectionDiaryReport.AddChild(ReportPermissions.DepositCollectionDiaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depositCollectionDiaryReport.AddChild(ReportPermissions.DepositCollectionDiaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depositCollectionDiaryReport.AddChild(ReportPermissions.DepositCollectionDiaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depositSpendDiaryReport = generalAccVoucherGroup.AddChild(ReportPermissions.DepositSpendDiaryReport,
                G(ReportPermissions.DepositSpendDiaryReport), MultiTenancySides.Tenant);
            depositSpendDiaryReport.AddChild(ReportPermissions.DepositSpendDiaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depositSpendDiaryReport.AddChild(ReportPermissions.DepositSpendDiaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depositSpendDiaryReport.AddChild(ReportPermissions.DepositSpendDiaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var purchaseDiaryReport = generalAccVoucherGroup.AddChild(ReportPermissions.PurchaseDiaryReport,
                G(ReportPermissions.PurchaseDiaryReport), MultiTenancySides.Tenant);
            purchaseDiaryReport.AddChild(ReportPermissions.PurchaseDiaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            purchaseDiaryReport.AddChild(ReportPermissions.PurchaseDiaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            purchaseDiaryReport.AddChild(ReportPermissions.PurchaseDiaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var sellingDiaryReport = generalAccVoucherGroup.AddChild(ReportPermissions.SellingDiaryReport,
                G(ReportPermissions.SellingDiaryReport), MultiTenancySides.Tenant);
            sellingDiaryReport.AddChild(ReportPermissions.SellingDiaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            sellingDiaryReport.AddChild(ReportPermissions.SellingDiaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            sellingDiaryReport.AddChild(ReportPermissions.SellingDiaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var accountingTReport = generalAccVoucherGroup.AddChild(ReportPermissions.AccountingTReport,
                G(ReportPermissions.AccountingTReport), MultiTenancySides.Tenant);
            accountingTReport.AddChild(ReportPermissions.AccountingTReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            accountingTReport.AddChild(ReportPermissions.AccountingTReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            accountingTReport.AddChild(ReportPermissions.AccountingTReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var listOfVouchersBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.ListOfVouchersBookReport,
                G(ReportPermissions.ListOfVouchersBookReport), MultiTenancySides.Tenant);
            listOfVouchersBookReport.AddChild(ReportPermissions.ListOfVouchersBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            listOfVouchersBookReport.AddChild(ReportPermissions.ListOfVouchersBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            listOfVouchersBookReport.AddChild(ReportPermissions.ListOfVouchersBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var listOfVoucherMultiBookReport = generalAccVoucherGroup.AddChild(ReportPermissions.ListOfVoucherMultiBookReport,
                G(ReportPermissions.ListOfVoucherMultiBookReport), MultiTenancySides.Tenant);
            listOfVoucherMultiBookReport.AddChild(ReportPermissions.ListOfVoucherMultiBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            listOfVoucherMultiBookReport.AddChild(ReportPermissions.ListOfVoucherMultiBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            listOfVoucherMultiBookReport.AddChild(ReportPermissions.ListOfVoucherMultiBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);            

            var accountAnalysisReport = generalAccVoucherGroup.AddChild(ReportPermissions.AccountAnalysisReport,
                G(ReportPermissions.AccountAnalysisReport), MultiTenancySides.Tenant);
            accountAnalysisReport.AddChild(ReportPermissions.AccountAnalysisReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            accountAnalysisReport.AddChild(ReportPermissions.AccountAnalysisReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            accountAnalysisReport.AddChild(ReportPermissions.AccountAnalysisReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var importTempVoucherReport = generalAccVoucherGroup.AddChild(ReportPermissions.ImportTempVoucherReport,
                G(ReportPermissions.ImportTempVoucherReport), MultiTenancySides.Tenant);
            importTempVoucherReport.AddChild(ReportPermissions.ImportTempVoucherReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            importTempVoucherReport.AddChild(ReportPermissions.ImportTempVoucherReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            importTempVoucherReport.AddChild(ReportPermissions.ImportTempVoucherReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }
        private void ConfigFinacialReportGroup(PermissionGroupDefinition group)
        {
            var finacialGroup = group.AddPermission(ReportPermissions.FinacialGroup,
                G(ReportPermissions.FinacialGroup), MultiTenancySides.Tenant);

            var balanaceSheetAccReport = finacialGroup.AddChild(ReportPermissions.BalanceSheetAccReport,
                G(ReportPermissions.BalanceSheetAccReport), MultiTenancySides.Tenant);
            balanaceSheetAccReport.AddChild(ReportPermissions.BalanceSheetAccReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            balanaceSheetAccReport.AddChild(ReportPermissions.BalanceSheetAccReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            balanaceSheetAccReport.AddChild(ReportPermissions.BalanceSheetAccReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var accBalanceSheetReport = finacialGroup.AddChild(ReportPermissions.AccBalanceSheetReport,
                G(ReportPermissions.AccBalanceSheetReport), MultiTenancySides.Tenant);
            accBalanceSheetReport.AddChild(ReportPermissions.AccBalanceSheetReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            accBalanceSheetReport.AddChild(ReportPermissions.AccBalanceSheetReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            accBalanceSheetReport.AddChild(ReportPermissions.AccBalanceSheetReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var cashFlowReport = finacialGroup.AddChild(ReportPermissions.CashFlowReport,
                G(ReportPermissions.CashFlowReport), MultiTenancySides.Tenant);
            cashFlowReport.AddChild(ReportPermissions.CashFlowReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            cashFlowReport.AddChild(ReportPermissions.CashFlowReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            cashFlowReport.AddChild(ReportPermissions.CashFlowReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var businessResultReport = finacialGroup.AddChild(ReportPermissions.BusinessResultReport,
                G(ReportPermissions.BusinessResultReport), MultiTenancySides.Tenant);
            businessResultReport.AddChild(ReportPermissions.BusinessResultReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            businessResultReport.AddChild(ReportPermissions.BusinessResultReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            businessResultReport.AddChild(ReportPermissions.BusinessResultReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var financialStatementReport = finacialGroup.AddChild(ReportPermissions.FinancialStatementReport,
                G(ReportPermissions.FinancialStatementReport), MultiTenancySides.Tenant);
            financialStatementReport.AddChild(ReportPermissions.FinancialStatementReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            financialStatementReport.AddChild(ReportPermissions.FinancialStatementReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            financialStatementReport.AddChild(ReportPermissions.FinancialStatementReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            financialStatementReport = finacialGroup.AddChild(ReportPermissions.FinancialStatement133Report,
                G(ReportPermissions.FinancialStatement133Report), MultiTenancySides.Tenant);
            financialStatementReport.AddChild(ReportPermissions.FinancialStatement133ReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            financialStatementReport.AddChild(ReportPermissions.FinancialStatement133ReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            financialStatementReport.AddChild(ReportPermissions.FinancialStatement133ReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }

        private void ConfigImportExportGroup(PermissionGroupDefinition group)
        {
            var importExportGroup = group.AddPermission(ReportPermissions.ImportExportGroup,
                G(ReportPermissions.ImportExportGroup), MultiTenancySides.Tenant);

            var detailedInventoryBookReport = importExportGroup.AddChild(ReportPermissions.DetailedInventoryBookReport,
                G(ReportPermissions.DetailedInventoryBookReport), MultiTenancySides.Tenant);
            detailedInventoryBookReport.AddChild(ReportPermissions.DetailedInventoryBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            detailedInventoryBookReport.AddChild(ReportPermissions.DetailedInventoryBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            detailedInventoryBookReport.AddChild(ReportPermissions.DetailedInventoryBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var inventorySummaryReport = importExportGroup.AddChild(ReportPermissions.InventorySummaryReport,
                G(ReportPermissions.InventorySummaryReport), MultiTenancySides.Tenant);
            inventorySummaryReport.AddChild(ReportPermissions.InventorySummaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            inventorySummaryReport.AddChild(ReportPermissions.InventorySummaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            inventorySummaryReport.AddChild(ReportPermissions.InventorySummaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var closingInventoryReport = importExportGroup.AddChild(ReportPermissions.ClosingInventoryReport,
                G(ReportPermissions.ClosingInventoryReport), MultiTenancySides.Tenant);
            closingInventoryReport.AddChild(ReportPermissions.ClosingInventoryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            closingInventoryReport.AddChild(ReportPermissions.ClosingInventoryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            closingInventoryReport.AddChild(ReportPermissions.ClosingInventoryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var openingInventoryReport = importExportGroup.AddChild(ReportPermissions.OpeningInventoryReport,
                G(ReportPermissions.OpeningInventoryReport), MultiTenancySides.Tenant);
            openingInventoryReport.AddChild(ReportPermissions.OpeningInventoryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            openingInventoryReport.AddChild(ReportPermissions.OpeningInventoryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            openingInventoryReport.AddChild(ReportPermissions.OpeningInventoryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var issueTransactionListReport = importExportGroup.AddChild(ReportPermissions.IssueTransactionListReport,
                G(ReportPermissions.IssueTransactionListReport), MultiTenancySides.Tenant);
            issueTransactionListReport.AddChild(ReportPermissions.IssueTransactionListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            issueTransactionListReport.AddChild(ReportPermissions.IssueTransactionListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            issueTransactionListReport.AddChild(ReportPermissions.IssueTransactionListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var saleDetailBookReport = importExportGroup.AddChild(ReportPermissions.SaleDetailBookReport,
                G(ReportPermissions.SaleDetailBookReport), MultiTenancySides.Tenant);
            saleDetailBookReport.AddChild(ReportPermissions.SaleDetailBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            saleDetailBookReport.AddChild(ReportPermissions.SaleDetailBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            saleDetailBookReport.AddChild(ReportPermissions.SaleDetailBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var summaryPurchaseReport = importExportGroup.AddChild(ReportPermissions.SummaryPurchaseReport,
                G(ReportPermissions.SummaryPurchaseReport), MultiTenancySides.Tenant);
            summaryPurchaseReport.AddChild(ReportPermissions.SummaryPurchaseReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            summaryPurchaseReport.AddChild(ReportPermissions.SummaryPurchaseReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            summaryPurchaseReport.AddChild(ReportPermissions.SummaryPurchaseReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var receiptTransactionListReport = importExportGroup.AddChild(ReportPermissions.ReceiptTransactionListReport,
                G(ReportPermissions.ReceiptTransactionListReport), MultiTenancySides.Tenant);
            receiptTransactionListReport.AddChild(ReportPermissions.ReceiptTransactionListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            receiptTransactionListReport.AddChild(ReportPermissions.ReceiptTransactionListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            receiptTransactionListReport.AddChild(ReportPermissions.ReceiptTransactionListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var salesTransactionListReport = importExportGroup.AddChild(ReportPermissions.SalesTransactionListReport,
                G(ReportPermissions.SalesTransactionListReport), MultiTenancySides.Tenant);
            salesTransactionListReport.AddChild(ReportPermissions.SalesTransactionListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            salesTransactionListReport.AddChild(ReportPermissions.SalesTransactionListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            salesTransactionListReport.AddChild(ReportPermissions.SalesTransactionListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var returnToSupplierListReport = importExportGroup.AddChild(ReportPermissions.ReturnToSupplierListReport,
                G(ReportPermissions.ReturnToSupplierListReport), MultiTenancySides.Tenant);
            returnToSupplierListReport.AddChild(ReportPermissions.ReturnToSupplierListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            returnToSupplierListReport.AddChild(ReportPermissions.ReturnToSupplierListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            returnToSupplierListReport.AddChild(ReportPermissions.ReturnToSupplierListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var purchaseImportTaxReport = importExportGroup.AddChild(ReportPermissions.PurchaseImportTaxReport,
                G(ReportPermissions.PurchaseImportTaxReport), MultiTenancySides.Tenant);
            purchaseImportTaxReport.AddChild(ReportPermissions.PurchaseImportTaxReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            purchaseImportTaxReport.AddChild(ReportPermissions.PurchaseImportTaxReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            purchaseImportTaxReport.AddChild(ReportPermissions.PurchaseImportTaxReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var exportSummaryReport = importExportGroup.AddChild(ReportPermissions.ExportSummaryReport,
                G(ReportPermissions.ExportSummaryReport), MultiTenancySides.Tenant);
            exportSummaryReport.AddChild(ReportPermissions.ExportSummaryReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            exportSummaryReport.AddChild(ReportPermissions.ExportSummaryReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            exportSummaryReport.AddChild(ReportPermissions.ExportSummaryReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var issueTransactionListMultiReport = importExportGroup.AddChild(ReportPermissions.IssueTransactionListMultiReport,
                G(ReportPermissions.IssueTransactionListMultiReport), MultiTenancySides.Tenant);
            issueTransactionListMultiReport.AddChild(ReportPermissions.IssueTransactionListMultiReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            issueTransactionListMultiReport.AddChild(ReportPermissions.IssueTransactionListMultiReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            issueTransactionListMultiReport.AddChild(ReportPermissions.IssueTransactionListMultiReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var salesTransactionListMultiReport = importExportGroup.AddChild(ReportPermissions.SalesTransactionListMultiReport,
                G(ReportPermissions.SalesTransactionListMultiReport), MultiTenancySides.Tenant);
            salesTransactionListMultiReport.AddChild(ReportPermissions.SalesTransactionListMultiReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            salesTransactionListMultiReport.AddChild(ReportPermissions.SalesTransactionListMultiReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            salesTransactionListMultiReport.AddChild(ReportPermissions.SalesTransactionListMultiReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var summaryPurchaseMultiReport = importExportGroup.AddChild(ReportPermissions.SummaryPurchaseMultiReport,
                G(ReportPermissions.SummaryPurchaseMultiReport), MultiTenancySides.Tenant);
            summaryPurchaseMultiReport.AddChild(ReportPermissions.SummaryPurchaseMultiReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            summaryPurchaseMultiReport.AddChild(ReportPermissions.SummaryPurchaseMultiReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            summaryPurchaseMultiReport.AddChild(ReportPermissions.SummaryPurchaseMultiReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var summaryExportMultiReport = importExportGroup.AddChild(ReportPermissions.SummaryExportMultiReport,
                G(ReportPermissions.SummaryExportMultiReport), MultiTenancySides.Tenant);
            summaryExportMultiReport.AddChild(ReportPermissions.SummaryExportMultiReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            summaryExportMultiReport.AddChild(ReportPermissions.SummaryExportMultiReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            summaryExportMultiReport.AddChild(ReportPermissions.SummaryExportMultiReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var salesReturnReport = importExportGroup.AddChild(ReportPermissions.SalesReturnReport,
                G(ReportPermissions.SalesReturnReport), MultiTenancySides.Tenant);
            salesReturnReport.AddChild(ReportPermissions.SalesReturnReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            salesReturnReport.AddChild(ReportPermissions.SalesReturnReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            salesReturnReport.AddChild(ReportPermissions.SalesReturnReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);            

            var purchaseWithTaxListReport = importExportGroup.AddChild(ReportPermissions.PurchaseWithTaxListReport,
                G(ReportPermissions.PurchaseWithTaxListReport), MultiTenancySides.Tenant);
            purchaseWithTaxListReport.AddChild(ReportPermissions.PurchaseWithTaxListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            purchaseWithTaxListReport.AddChild(ReportPermissions.PurchaseWithTaxListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            purchaseWithTaxListReport.AddChild(ReportPermissions.PurchaseWithTaxListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var directImportExportListReport = importExportGroup.AddChild(ReportPermissions.DirectImportExportListReport,
                G(ReportPermissions.DirectImportExportListReport), MultiTenancySides.Tenant);
            directImportExportListReport.AddChild(ReportPermissions.DirectImportExportListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            directImportExportListReport.AddChild(ReportPermissions.DirectImportExportListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            directImportExportListReport.AddChild(ReportPermissions.DirectImportExportListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var summarySalesReport = importExportGroup.AddChild(ReportPermissions.SummarySalesReport,
                G(ReportPermissions.SummarySalesReport), MultiTenancySides.Tenant);
            summarySalesReport.AddChild(ReportPermissions.SummarySalesReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            summarySalesReport.AddChild(ReportPermissions.SummarySalesReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            summarySalesReport.AddChild(ReportPermissions.SummarySalesReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var salesAnalysisReport = importExportGroup.AddChild(ReportPermissions.SalesAnalysisReport,
                G(ReportPermissions.SalesAnalysisReport), MultiTenancySides.Tenant);
            salesAnalysisReport.AddChild(ReportPermissions.SalesAnalysisReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            salesAnalysisReport.AddChild(ReportPermissions.SalesAnalysisReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            salesAnalysisReport.AddChild(ReportPermissions.SalesAnalysisReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var summarySalesMultiReport = importExportGroup.AddChild(ReportPermissions.SummarySalesMultiReport,
                G(ReportPermissions.SummarySalesMultiReport), MultiTenancySides.Tenant);
            summarySalesMultiReport.AddChild(ReportPermissions.SummarySalesMultiReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            summarySalesMultiReport.AddChild(ReportPermissions.SummarySalesMultiReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            summarySalesMultiReport.AddChild(ReportPermissions.SummarySalesMultiReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var listPurchaseMultiReport = importExportGroup.AddChild(ReportPermissions.ListPurchaseMultiReport,
                G(ReportPermissions.ListPurchaseMultiReport), MultiTenancySides.Tenant);
            listPurchaseMultiReport.AddChild(ReportPermissions.ListPurchaseMultiReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            listPurchaseMultiReport.AddChild(ReportPermissions.ListPurchaseMultiReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            listPurchaseMultiReport.AddChild(ReportPermissions.ListPurchaseMultiReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var stockCardReport = importExportGroup.AddChild(ReportPermissions.StockCardReport,
                G(ReportPermissions.StockCardReport), MultiTenancySides.Tenant);
            stockCardReport.AddChild(ReportPermissions.StockCardReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            stockCardReport.AddChild(ReportPermissions.StockCardReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            stockCardReport.AddChild(ReportPermissions.StockCardReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var salesOrderTrackingReport = importExportGroup.AddChild(ReportPermissions.SalesOrderTrackingReport,
                G(ReportPermissions.SalesOrderTrackingReport), MultiTenancySides.Tenant);
            salesOrderTrackingReport.AddChild(ReportPermissions.SalesOrderTrackingReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            salesOrderTrackingReport.AddChild(ReportPermissions.SalesOrderTrackingReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            salesOrderTrackingReport.AddChild(ReportPermissions.SalesOrderTrackingReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var purchaseOrderTrackingReport = importExportGroup.AddChild(ReportPermissions.PurchaseOrderTrackingReport,
                G(ReportPermissions.PurchaseOrderTrackingReport), MultiTenancySides.Tenant);
            purchaseOrderTrackingReport.AddChild(ReportPermissions.PurchaseOrderTrackingReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            purchaseOrderTrackingReport.AddChild(ReportPermissions.PurchaseOrderTrackingReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            purchaseOrderTrackingReport.AddChild(ReportPermissions.PurchaseOrderTrackingReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var contractPerformanceReport = importExportGroup.AddChild(ReportPermissions.ContractPerformanceReport,
                G(ReportPermissions.ContractPerformanceReport), MultiTenancySides.Tenant);
            contractPerformanceReport.AddChild(ReportPermissions.ContractPerformanceReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            contractPerformanceReport.AddChild(ReportPermissions.ContractPerformanceReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            contractPerformanceReport.AddChild(ReportPermissions.ContractPerformanceReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }

        private void ConfigBookRecordingGroup(PermissionGroupDefinition group)
        {
            var bookRecordingGroup = group.AddPermission(ReportPermissions.BookRecordingGroup,
                G(ReportPermissions.BookRecordingGroup), MultiTenancySides.Tenant);

            var ledgerRecordingVoucherReport = bookRecordingGroup.AddChild(ReportPermissions.LedgerRecordingVoucherReport,
                G(ReportPermissions.LedgerRecordingVoucherReport), MultiTenancySides.Tenant);
            ledgerRecordingVoucherReport.AddChild(ReportPermissions.LedgerRecordingVoucherReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            ledgerRecordingVoucherReport.AddChild(ReportPermissions.LedgerRecordingVoucherReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            ledgerRecordingVoucherReport.AddChild(ReportPermissions.LedgerRecordingVoucherReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var accountDetailBookRVReport = bookRecordingGroup.AddChild(ReportPermissions.AccountDetailBookRVReport,
                G(ReportPermissions.AccountDetailBookRVReport), MultiTenancySides.Tenant);
            accountDetailBookRVReport.AddChild(ReportPermissions.AccountDetailBookRVReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            accountDetailBookRVReport.AddChild(ReportPermissions.AccountDetailBookRVReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            accountDetailBookRVReport.AddChild(ReportPermissions.AccountDetailBookRVReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var recordingVoucherRegisterBookReport = bookRecordingGroup.AddChild(ReportPermissions.RecordingVoucherRegisterBookReport,
                G(ReportPermissions.RecordingVoucherRegisterBookReport), MultiTenancySides.Tenant);
            recordingVoucherRegisterBookReport.AddChild(ReportPermissions.RecordingVoucherRegisterBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            recordingVoucherRegisterBookReport.AddChild(ReportPermissions.RecordingVoucherRegisterBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            recordingVoucherRegisterBookReport.AddChild(ReportPermissions.RecordingVoucherRegisterBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var recordingVoucherReport = bookRecordingGroup.AddChild(ReportPermissions.RecordingVoucherReport,
                G(ReportPermissions.RecordingVoucherReport), MultiTenancySides.Tenant);
            recordingVoucherReport.AddChild(ReportPermissions.RecordingVoucherReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            recordingVoucherReport.AddChild(ReportPermissions.RecordingVoucherReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            recordingVoucherReport.AddChild(ReportPermissions.RecordingVoucherReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }

        private void ConfigDebtGroup(PermissionGroupDefinition group)
        {
            var debtGroup = group.AddPermission(ReportPermissions.DebtGroup,
                G(ReportPermissions.DebtGroup), MultiTenancySides.Tenant);

            var debtBalanceSheetReport = debtGroup.AddChild(ReportPermissions.DebtBalanceSheetReport,
                G(ReportPermissions.DebtBalanceSheetReport), MultiTenancySides.Tenant);
            debtBalanceSheetReport.AddChild(ReportPermissions.DebtBalanceSheetReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            debtBalanceSheetReport.AddChild(ReportPermissions.DebtBalanceSheetReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            debtBalanceSheetReport.AddChild(ReportPermissions.DebtBalanceSheetReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var debtDetailBookReport = debtGroup.AddChild(ReportPermissions.DebtDetailBookReport,
                G(ReportPermissions.DebtDetailBookReport), MultiTenancySides.Tenant);
            debtDetailBookReport.AddChild(ReportPermissions.DebtDetailBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            debtDetailBookReport.AddChild(ReportPermissions.DebtDetailBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            debtDetailBookReport.AddChild(ReportPermissions.DebtDetailBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var debtClosingBalanceReport = debtGroup.AddChild(ReportPermissions.DebtClosingBalanceReport,
                G(ReportPermissions.DebtClosingBalanceReport), MultiTenancySides.Tenant);
            debtClosingBalanceReport.AddChild(ReportPermissions.DebtClosingBalanceReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            debtClosingBalanceReport.AddChild(ReportPermissions.DebtClosingBalanceReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            debtClosingBalanceReport.AddChild(ReportPermissions.DebtClosingBalanceReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var debtOpeningBalanceReport = debtGroup.AddChild(ReportPermissions.DebtOpeningBalanceReport,
                G(ReportPermissions.DebtOpeningBalanceReport), MultiTenancySides.Tenant);
            debtOpeningBalanceReport.AddChild(ReportPermissions.DebtOpeningBalanceReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            debtOpeningBalanceReport.AddChild(ReportPermissions.DebtOpeningBalanceReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            debtOpeningBalanceReport.AddChild(ReportPermissions.DebtOpeningBalanceReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var debtproductDetailReport = debtGroup.AddChild(ReportPermissions.DebtProductDetailReport,
                G(ReportPermissions.DebtProductDetailReport), MultiTenancySides.Tenant);
            debtproductDetailReport.AddChild(ReportPermissions.DebtProductDetailReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            debtproductDetailReport.AddChild(ReportPermissions.DebtProductDetailReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            debtproductDetailReport.AddChild(ReportPermissions.DebtProductDetailReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var debtCompareConfirmReport = debtGroup.AddChild(ReportPermissions.DebtCompareConfirmReport,
                G(ReportPermissions.DebtCompareConfirmReport), MultiTenancySides.Tenant);
            debtCompareConfirmReport.AddChild(ReportPermissions.DebtCompareConfirmReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            debtCompareConfirmReport.AddChild(ReportPermissions.DebtCompareConfirmReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            debtCompareConfirmReport.AddChild(ReportPermissions.DebtCompareConfirmReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var detailDebtInvoiceReport = debtGroup.AddChild(ReportPermissions.DetailDebtInvoiceReport,
                G(ReportPermissions.DetailDebtInvoiceReport), MultiTenancySides.Tenant);
            detailDebtInvoiceReport.AddChild(ReportPermissions.DetailDebtInvoiceReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            detailDebtInvoiceReport.AddChild(ReportPermissions.DetailDebtInvoiceReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            detailDebtInvoiceReport.AddChild(ReportPermissions.DetailDebtInvoiceReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var summaryDebtInvoiceReport = debtGroup.AddChild(ReportPermissions.SummaryDebtInvoiceReport,
                G(ReportPermissions.SummaryDebtInvoiceReport), MultiTenancySides.Tenant);
            summaryDebtInvoiceReport.AddChild(ReportPermissions.SummaryDebtInvoiceReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            summaryDebtInvoiceReport.AddChild(ReportPermissions.SummaryDebtInvoiceReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            summaryDebtInvoiceReport.AddChild(ReportPermissions.SummaryDebtInvoiceReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var debtAccordingInvoiceReport = debtGroup.AddChild(ReportPermissions.DebtAccordingInvoiceReport,
                G(ReportPermissions.DebtAccordingInvoiceReport), MultiTenancySides.Tenant);
            debtAccordingInvoiceReport.AddChild(ReportPermissions.DebtAccordingInvoiceReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            debtAccordingInvoiceReport.AddChild(ReportPermissions.DebtAccordingInvoiceReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            debtAccordingInvoiceReport.AddChild(ReportPermissions.DebtAccordingInvoiceReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var voucherPaymentBookReport = debtGroup.AddChild(ReportPermissions.VoucherPaymentBookReport,
                G(ReportPermissions.VoucherPaymentBookReport), MultiTenancySides.Tenant);
            voucherPaymentBookReport.AddChild(ReportPermissions.VoucherPaymentBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            voucherPaymentBookReport.AddChild(ReportPermissions.VoucherPaymentBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            voucherPaymentBookReport.AddChild(ReportPermissions.VoucherPaymentBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }

        private void ConfigAssetGroup(PermissionGroupDefinition group)
        {
            var assetGroup = group.AddPermission(ReportPermissions.AssetGroup,
                G(ReportPermissions.AssetGroup), MultiTenancySides.Tenant);

            var assetBookReport = assetGroup.AddChild(ReportPermissions.AssetBookReport,
                G(ReportPermissions.AssetBookReport), MultiTenancySides.Tenant);
            assetBookReport.AddChild(ReportPermissions.AssetBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            assetBookReport.AddChild(ReportPermissions.AssetBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            assetBookReport.AddChild(ReportPermissions.AssetBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depreciationTotalListReport = assetGroup.AddChild(ReportPermissions.DepreciationAssetTotalReport,
                G(ReportPermissions.DepreciationAssetTotalReport), MultiTenancySides.Tenant);
            depreciationTotalListReport.AddChild(ReportPermissions.DepreciationAssetTotalReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depreciationTotalListReport.AddChild(ReportPermissions.DepreciationAssetTotalReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depreciationTotalListReport.AddChild(ReportPermissions.DepreciationAssetTotalReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depreciationAssetCapitalReport = assetGroup.AddChild(ReportPermissions.DepreciationAssetCapitalReport,
                G(ReportPermissions.DepreciationAssetCapitalReport), MultiTenancySides.Tenant);
            depreciationAssetCapitalReport.AddChild(ReportPermissions.DepreciationAssetCapitalReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depreciationAssetCapitalReport.AddChild(ReportPermissions.DepreciationAssetCapitalReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depreciationAssetCapitalReport.AddChild(ReportPermissions.DepreciationAssetCapitalReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depreciationAssetMonthReport = assetGroup.AddChild(ReportPermissions.DepreciationAssetMonthReport,
                G(ReportPermissions.DepreciationAssetMonthReport), MultiTenancySides.Tenant);
            depreciationAssetMonthReport.AddChild(ReportPermissions.DepreciationAssetMonthReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depreciationAssetMonthReport.AddChild(ReportPermissions.DepreciationAssetMonthReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depreciationAssetMonthReport.AddChild(ReportPermissions.DepreciationAssetMonthReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var assetAllocateListReport = assetGroup.AddChild(ReportPermissions.AssetAllocateListReport,
                G(ReportPermissions.AssetAllocateListReport), MultiTenancySides.Tenant);
            assetAllocateListReport.AddChild(ReportPermissions.AssetAllocateListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            assetAllocateListReport.AddChild(ReportPermissions.AssetAllocateListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            assetAllocateListReport.AddChild(ReportPermissions.AssetAllocateListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var assetCardReport = assetGroup.AddChild(ReportPermissions.AssetCardReport,
                G(ReportPermissions.AssetCardReport), MultiTenancySides.Tenant);
            assetCardReport.AddChild(ReportPermissions.AssetCardReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            assetCardReport.AddChild(ReportPermissions.AssetCardReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            assetCardReport.AddChild(ReportPermissions.AssetCardReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var assetUpDownReport = assetGroup.AddChild(ReportPermissions.AssetUpDownReport,
                G(ReportPermissions.AssetUpDownReport), MultiTenancySides.Tenant);
            assetUpDownReport.AddChild(ReportPermissions.AssetUpDownReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            assetUpDownReport.AddChild(ReportPermissions.AssetUpDownReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            assetUpDownReport.AddChild(ReportPermissions.AssetUpDownReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }

        private void ConfigToolGroup(PermissionGroupDefinition group)
        {
            var toolGroup = group.AddPermission(ReportPermissions.ToolGroup,
                G(ReportPermissions.ToolGroup), MultiTenancySides.Tenant);

            var toolBookReport = toolGroup.AddChild(ReportPermissions.ToolBookReport,
                G(ReportPermissions.ToolBookReport), MultiTenancySides.Tenant);
            toolBookReport.AddChild(ReportPermissions.ToolBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            toolBookReport.AddChild(ReportPermissions.ToolBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            toolBookReport.AddChild(ReportPermissions.ToolBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var toolAllocateListReport = toolGroup.AddChild(ReportPermissions.ToolAllocateListReport,
                G(ReportPermissions.ToolAllocateListReport), MultiTenancySides.Tenant);
            toolAllocateListReport.AddChild(ReportPermissions.ToolAllocateListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            toolAllocateListReport.AddChild(ReportPermissions.ToolAllocateListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            toolAllocateListReport.AddChild(ReportPermissions.ToolAllocateListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depreciationTotalListReport = toolGroup.AddChild(ReportPermissions.DepreciationToolTotalReport,
                G(ReportPermissions.DepreciationToolTotalReport), MultiTenancySides.Tenant);
            depreciationTotalListReport.AddChild(ReportPermissions.DepreciationToolTotalReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depreciationTotalListReport.AddChild(ReportPermissions.DepreciationToolTotalReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depreciationTotalListReport.AddChild(ReportPermissions.DepreciationToolTotalReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depreciationToolCapitalReport = toolGroup.AddChild(ReportPermissions.DepreciationToolCapitalReport,
                G(ReportPermissions.DepreciationToolCapitalReport), MultiTenancySides.Tenant);
            depreciationToolCapitalReport.AddChild(ReportPermissions.DepreciationToolCapitalReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depreciationToolCapitalReport.AddChild(ReportPermissions.DepreciationToolCapitalReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depreciationToolCapitalReport.AddChild(ReportPermissions.DepreciationToolCapitalReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var depreciationToolMonthReport = toolGroup.AddChild(ReportPermissions.DepreciationToolMonthReport,
                G(ReportPermissions.DepreciationToolMonthReport), MultiTenancySides.Tenant);
            depreciationToolMonthReport.AddChild(ReportPermissions.DepreciationToolMonthReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            depreciationToolMonthReport.AddChild(ReportPermissions.DepreciationToolMonthReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            depreciationToolMonthReport.AddChild(ReportPermissions.DepreciationToolMonthReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var toolCardReport = toolGroup.AddChild(ReportPermissions.ToolCardReport,
                G(ReportPermissions.ToolCardReport), MultiTenancySides.Tenant);
            toolCardReport.AddChild(ReportPermissions.ToolCardReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            toolCardReport.AddChild(ReportPermissions.ToolCardReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            toolCardReport.AddChild(ReportPermissions.ToolCardReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var toolUpDownReport = toolGroup.AddChild(ReportPermissions.ToolUpDownReport,
                G(ReportPermissions.ToolUpDownReport), MultiTenancySides.Tenant);
            toolUpDownReport.AddChild(ReportPermissions.ToolUpDownReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            toolUpDownReport.AddChild(ReportPermissions.ToolUpDownReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            toolUpDownReport.AddChild(ReportPermissions.ToolUpDownReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }
        private void ConfigTaxGroup(PermissionGroupDefinition group)
        {
            var taxGroup = group.AddPermission(ReportPermissions.TaxGroup,
                G(ReportPermissions.TaxGroup), MultiTenancySides.Tenant);

            var salesTaxListReport = taxGroup.AddChild(ReportPermissions.SalesTaxListReport,
                G(ReportPermissions.SalesTaxListReport), MultiTenancySides.Tenant);
            salesTaxListReport.AddChild(ReportPermissions.SalesTaxListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            salesTaxListReport.AddChild(ReportPermissions.SalesTaxListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            salesTaxListReport.AddChild(ReportPermissions.SalesTaxListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var purchaseTaxListReport = taxGroup.AddChild(ReportPermissions.PurchaseTaxListReport,
                G(ReportPermissions.PurchaseTaxListReport), MultiTenancySides.Tenant);
            purchaseTaxListReport.AddChild(ReportPermissions.PurchaseTaxListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            purchaseTaxListReport.AddChild(ReportPermissions.PurchaseTaxListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            purchaseTaxListReport.AddChild(ReportPermissions.PurchaseTaxListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var vatStatementReport = taxGroup.AddChild(ReportPermissions.VatStatementReport,
                G(ReportPermissions.VatStatementReport), MultiTenancySides.Tenant);
            vatStatementReport.AddChild(ReportPermissions.VatStatementReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            vatStatementReport.AddChild(ReportPermissions.VatStatementReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            vatStatementReport.AddChild(ReportPermissions.VatStatementReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var exciseTaxSalesListReport = taxGroup.AddChild(ReportPermissions.ExciseTaxSalesListReport,
                G(ReportPermissions.ExciseTaxSalesListReport), MultiTenancySides.Tenant);
            exciseTaxSalesListReport.AddChild(ReportPermissions.ExciseTaxSalesListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            exciseTaxSalesListReport.AddChild(ReportPermissions.ExciseTaxSalesListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            exciseTaxSalesListReport.AddChild(ReportPermissions.ExciseTaxSalesListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var declarationExciseTaxReport = taxGroup.AddChild(ReportPermissions.DeclarationExciseTaxReport,
                G(ReportPermissions.DeclarationExciseTaxReport), MultiTenancySides.Tenant);
            declarationExciseTaxReport.AddChild(ReportPermissions.DeclarationExciseTaxReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            declarationExciseTaxReport.AddChild(ReportPermissions.DeclarationExciseTaxReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            declarationExciseTaxReport.AddChild(ReportPermissions.DeclarationExciseTaxReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var vatDirectStatementReport = taxGroup.AddChild(ReportPermissions.VatDirectStatementReport,
                G(ReportPermissions.VatDirectStatementReport), MultiTenancySides.Tenant);
            vatDirectStatementReport.AddChild(ReportPermissions.VatDirectStatementReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            vatDirectStatementReport.AddChild(ReportPermissions.VatDirectStatementReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            vatDirectStatementReport.AddChild(ReportPermissions.VatDirectStatementReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var reducingVatQH15Report = taxGroup.AddChild(ReportPermissions.ReducingVatQH15Report,
                G(ReportPermissions.ReducingVatQH15Report), MultiTenancySides.Tenant);
            reducingVatQH15Report.AddChild(ReportPermissions.ReducingVatQH15ReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            reducingVatQH15Report.AddChild(ReportPermissions.ReducingVatQH15ReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            reducingVatQH15Report.AddChild(ReportPermissions.ReducingVatQH15ReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var salesTaxDirectListReport = taxGroup.AddChild(ReportPermissions.SalesTaxDirectListReport,
                G(ReportPermissions.SalesTaxDirectListReport), MultiTenancySides.Tenant);
            salesTaxDirectListReport.AddChild(ReportPermissions.SalesTaxDirectListReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            salesTaxDirectListReport.AddChild(ReportPermissions.SalesTaxDirectListReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            salesTaxDirectListReport.AddChild(ReportPermissions.SalesTaxDirectListReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }
        private void ConfigCostGroup(PermissionGroupDefinition group)
        {
            var costGroup = group.AddPermission(ReportPermissions.CostGroup,
                G(ReportPermissions.CostGroup), MultiTenancySides.Tenant);

            var productQuotaReport = costGroup.AddChild(ReportPermissions.ProductQuotaReport,
                G(ReportPermissions.ProductQuotaReport), MultiTenancySides.Tenant);
            productQuotaReport.AddChild(ReportPermissions.ProductQuotaReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            productQuotaReport.AddChild(ReportPermissions.ProductQuotaReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            productQuotaReport.AddChild(ReportPermissions.ProductQuotaReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var workPriceCardReport = costGroup.AddChild(ReportPermissions.WorkPriceCardReport,
                G(ReportPermissions.WorkPriceCardReport), MultiTenancySides.Tenant);
            workPriceCardReport.AddChild(ReportPermissions.WorkPriceCardReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            workPriceCardReport.AddChild(ReportPermissions.WorkPriceCardReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            workPriceCardReport.AddChild(ReportPermissions.WorkPriceCardReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var productionCostCardReport = costGroup.AddChild(ReportPermissions.ProductionCostCardReport,
                G(ReportPermissions.ProductionCostCardReport), MultiTenancySides.Tenant);
            productionCostCardReport.AddChild(ReportPermissions.ProductionCostCardReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            productionCostCardReport.AddChild(ReportPermissions.ProductionCostCardReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            productionCostCardReport.AddChild(ReportPermissions.ProductionCostCardReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var productionCostBookReport = costGroup.AddChild(ReportPermissions.ProductionCostBookReport,
                G(ReportPermissions.ProductionCostBookReport), MultiTenancySides.Tenant);
            productionCostBookReport.AddChild(ReportPermissions.ProductionCostBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            productionCostBookReport.AddChild(ReportPermissions.ProductionCostBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            productionCostBookReport.AddChild(ReportPermissions.ProductionCostBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var workPriceBookReport = costGroup.AddChild(ReportPermissions.WorkPriceBookReport,
                G(ReportPermissions.WorkPriceBookReport), MultiTenancySides.Tenant);
            workPriceBookReport.AddChild(ReportPermissions.WorkPriceBookReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            workPriceBookReport.AddChild(ReportPermissions.WorkPriceBookReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            workPriceBookReport.AddChild(ReportPermissions.WorkPriceBookReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var productionCostReport = costGroup.AddChild(ReportPermissions.ProductionCostReport,
                G(ReportPermissions.ProductionCostReport), MultiTenancySides.Tenant);
            productionCostReport.AddChild(ReportPermissions.ProductionCostReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            productionCostReport.AddChild(ReportPermissions.ProductionCostReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            productionCostReport.AddChild(ReportPermissions.ProductionCostReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            var contructionCostReport = costGroup.AddChild(ReportPermissions.ContructionCostReport,
                G(ReportPermissions.ContructionCostReport), MultiTenancySides.Tenant);
            contructionCostReport.AddChild(ReportPermissions.ContructionCostReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            contructionCostReport.AddChild(ReportPermissions.ContructionCostReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            contructionCostReport.AddChild(ReportPermissions.ContructionCostReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }
        private void ConfigSummaryGroup(PermissionGroupDefinition group)
        {
            var hKdGroup = group.AddPermission(ReportPermissions.SummaryGroup,
                G(ReportPermissions.SummaryGroup), MultiTenancySides.Tenant);

            var report = hKdGroup.AddChild(ReportPermissions.SummaryAccBalanceSheetReport,
                G(ReportPermissions.SummaryAccBalanceSheetReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryAccBalanceSheetReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryAccBalanceSheetReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryAccBalanceSheetReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.SummaryCashFlowReport,
                G(ReportPermissions.SummaryCashFlowReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryCashFlowReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryCashFlowReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryCashFlowReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.SummaryBusinessResultReport,
                G(ReportPermissions.SummaryBusinessResultReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryBusinessResultReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryBusinessResultReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryBusinessResultReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.SummaryBalanceSheetReport,
                G(ReportPermissions.SummaryBalanceSheetReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryBalanceSheetReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryBalanceSheetReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryBalanceSheetReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);            
        }
        private void ConfigHkdGroup(PermissionGroupDefinition group)
        {
            var hKdGroup = group.AddPermission(ReportPermissions.HkdGroup,
                G(ReportPermissions.HkdGroup), MultiTenancySides.Tenant);

            var report = hKdGroup.AddChild(ReportPermissions.CashBookHkdReport,
                G(ReportPermissions.CashBookHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.CashBookHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.CashBookHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.CashBookHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.CashInBankHkdReport,
                G(ReportPermissions.CashInBankHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.CashInBankHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.CashInBankHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.CashInBankHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.TotalRevenueHkdReport,
                G(ReportPermissions.TotalRevenueHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.TotalRevenueHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.TotalRevenueHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.TotalRevenueHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.DetailRevenueHkdReport,
                G(ReportPermissions.DetailRevenueHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DetailRevenueHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DetailRevenueHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DetailRevenueHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.SummaryCostsHkdReport,
                G(ReportPermissions.SummaryCostsHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryCostHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryCostsHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SummaryCostsHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.BookCostsHkdReport,
                G(ReportPermissions.BookCostsHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.BookCostHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.BookCostsHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.BookCostsHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.ReducingVatHkdReport,
                G(ReportPermissions.ReducingVatHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.ReducingVatHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.ReducingVatHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.ReducingVatHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.SalarySheetHkdReport,
                G(ReportPermissions.SalarySheetHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SalarySheetHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SalarySheetHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.SalarySheetHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.ImportExportHkdReport,
                G(ReportPermissions.ImportExportHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.ImportExportHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.ImportExportHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.ImportExportHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.DetailInventoryHkdReport,
                G(ReportPermissions.DetailInventoryHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DetailInventoryHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DetailInventoryHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DetailInventoryHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.DebtBalanceHkdReport,
                G(ReportPermissions.DebtBalanceHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DebtBalanceHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DebtBalanceHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DebtBalanceHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.DebtDetailHkdReport,
                G(ReportPermissions.DebtDetailHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DebtDetailHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DebtDetailHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.DebtDetailHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);

            report = hKdGroup.AddChild(ReportPermissions.TaxObligationHkdReport,
                G(ReportPermissions.TaxObligationHkdReport), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.TaxObligationHkdReportView,
                    G(ReportPermissions.ActionView), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.TaxObligationHkdReportPrint,
                    G(ReportPermissions.ActionPrint), MultiTenancySides.Tenant);
            report.AddChild(ReportPermissions.TaxObligationHkdReportExportExcel,
                    G(ReportPermissions.ActionExportExcel), MultiTenancySides.Tenant);
        }
        private LocalizableString G(string name)
        {
            return L($"{AccountingPermissions.Prefix}:{name}");
        }
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AccountingResource>(name);
        }
    }
}
