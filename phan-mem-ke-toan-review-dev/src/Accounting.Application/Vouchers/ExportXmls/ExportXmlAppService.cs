using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Others.Careers;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Reports;
using Accounting.Vouchers.ResetVoucherNumbers;
using Accounting.Vouchers.TransMigrations;
using Accounting.Vouchers.VoucherNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using Microsoft.AspNetCore.Hosting;
using System.Xml;
using Accounting.Vouchers.GetFixDataVouchers;
using Accounting.Reports.ImportExports;
using Accounting.Reports.HouseholdBusiness;
using Accounting.Reports.ImportExports.Parameters;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Accounting.Constants;
using Accounting.Reports.Financials;
using Accounting.DomainServices.Reports;
using Accounting.Reports.Taxes;
using Accounting.Exceptions;
using Accounting.Reports.DebitBooks;
using Accounting.Business;

namespace Accounting.Vouchers.ExportXmls
{
    public class ExportXmlAppService : AccountingAppService, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly OrgUnitService _orgUnitService;
        private readonly CareerService _careerService;
        private readonly DefaultHtkkNumberCodeService _defaultHtkkNumberCodeService;
        private readonly TotalRevenueBusiness _totalRevenueBusiness;
        private readonly SummaryCostBusiness _summaryCostBusiness;
        private readonly ImportExportBusiness _importExportBusiness;
        private readonly ReducingVatBusiness _reducingVatBusiness;
        private readonly TenantSettingService _tenantSettingService;
        private readonly StatementOfValueAddedBusiness _statementOfValueAddedBusiness;
        private readonly ReducingVatUnderResolutionBusiness _reducingVatUnderResolutionBusiness;
        private readonly AccBalanceSheetReportBusiness _accBalanceSheetReportBusiness;
        private readonly BusinessResultBusiness _businessResultBusiness;
        private readonly CashFlowBusiness _cashFlowBusiness;
        private readonly BalanceSheetAccBusiness _balanceSheetAccBusiness;
        private readonly YearCategoryService _yearCategoryService;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion
        #region Ctor
        public ExportXmlAppService(
                            OrgUnitService orgUnitService,
                            CareerService careerService,
                            DefaultHtkkNumberCodeService defaultHtkkNumberCodeService,
                            TenantSettingService tenantSettingService,
                            StatementOfValueAddedBusiness statementOfValueAddedBusiness,
                            ReducingVatUnderResolutionBusiness reducingVatUnderResolutionBusiness,
                            AccBalanceSheetReportBusiness accBalanceSheetReportBusiness,
                            BusinessResultBusiness businessResultBusiness,
                            CashFlowBusiness cashFlowBusiness,
                            BalanceSheetAccBusiness balanceSheetAccBusiness,
                            YearCategoryService yearCategoryService,
                            TotalRevenueBusiness totalRevenueBusiness,
                            SummaryCostBusiness summaryCostBusiness,
                            ImportExportBusiness importExportBusiness,
                            ReducingVatBusiness reducingVatBusiness,
                            IUnitOfWorkManager unitOfWorkManager,
                            WebHelper webHelper,
                            IHostingEnvironment hostingEnvironment
                            )
        {
            _orgUnitService = orgUnitService;
            _careerService = careerService;
            _defaultHtkkNumberCodeService = defaultHtkkNumberCodeService;
            _tenantSettingService = tenantSettingService;
            _statementOfValueAddedBusiness = statementOfValueAddedBusiness;
            _reducingVatUnderResolutionBusiness = reducingVatUnderResolutionBusiness;
            _accBalanceSheetReportBusiness = accBalanceSheetReportBusiness;
            _businessResultBusiness = businessResultBusiness;
            _cashFlowBusiness = cashFlowBusiness;
            _balanceSheetAccBusiness = balanceSheetAccBusiness;
            _yearCategoryService = yearCategoryService;
            _totalRevenueBusiness = totalRevenueBusiness;
            _summaryCostBusiness = summaryCostBusiness;
            _importExportBusiness = importExportBusiness;
            _reducingVatBusiness = reducingVatBusiness;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion
        public async Task<FileContentResult> ExportXmlGtgtAsync(ExportXmlGTGT01ParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string fileName = "01_GTGT.xml";
            bool lessthanHtkk452 = true;
            var htkkVersion = await _tenantSettingService.GetTenantSettingByKeyAsync("HTKK_VERSION", orgCode);
            var orgUnit = await _orgUnitService.GetByCodeAsync(orgCode);
            var career = (await _careerService.GetQueryableAsync()).Where(p => p.Id == orgUnit.CareerId).FirstOrDefault();
            if (htkkVersion.Value.CompareTo("4.5.2") >= 0)
            {
                fileName = "01_GTGT_v452.xml";
                lessthanHtkk452 = false;
            }

            if (htkkVersion.Value.CompareTo("4.6.8") >= 0)
            {
                fileName = "01_GTGT_v468.xml";
                lessthanHtkk452 = false;
            }
            string path = _hostingEnvironment.WebRootPath + @"/Content/HTKK/" + fileName;

            // lấy dữ liệu báo cáo StatementOfValueAddedTax
            var statementOfValueAddedTaxParameterDto = new StatementOfValueAddedTaxParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Extend = dto.Extend,
                DeductPre = dto.DeductPre,
                IncreasePre = dto.IncreasePre,
                ReducePre = dto.ReducePre,
                SuggestionReturn = dto.SuggestionReturn,
            };
            var statementVATParameter = new ReportRequestDto<StatementOfValueAddedTaxParameterDto>
            {
                Parameters = statementOfValueAddedTaxParameterDto,
            };
            var dataStatementVAT = (await _statementOfValueAddedBusiness.CreateDataAsync(statementVATParameter)).Data;
            XmlDocument xmlDoc = new XmlDocument();
            //load the xml into the XmlDocument
            xmlDoc.Load(path);
            string thang = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            string ngay = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();

            DateTime tungay = dto.FromDate;
            DateTime denngay = dto.ToDate;
            if (tungay.Day == 1 && tungay.Month == 1 && denngay.Day == 31 && denngay.Month == 3)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "1/" + tungay.Year;
            }
            else if (tungay.Day == 1 && tungay.Month == 4 && denngay.Day == 30 && denngay.Month == 6)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "2/" + tungay.Year;
            }
            else if (tungay.Day == 1 && tungay.Month == 7 && denngay.Day == 30 && denngay.Month == 9)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "3/" + tungay.Year;
            }
            else if (tungay.Day == 1 && tungay.Month == 10 && denngay.Day == 31 && denngay.Month == 12)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "4/" + tungay.Year;
            }
            else
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "M";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = tungay.Month < 10 ? "0" + tungay.Month.ToString() + "/" + tungay.Year : tungay.Month.ToString() + "/" + tungay.Year;
            }

            XmlNode mst = xmlDoc.GetElementsByTagName("mst")[0];
            mst.InnerText = orgUnit.TaxCode;

            XmlNode tenNNT = xmlDoc.GetElementsByTagName("tenNNT")[0];
            tenNNT.InnerText = orgUnit.NameE;

            XmlNode dchiNNT = xmlDoc.GetElementsByTagName("dchiNNT")[0];
            dchiNNT.InnerText = orgUnit.Address;

            XmlNode phuongXa = xmlDoc.GetElementsByTagName("phuongXa")[0];
            phuongXa.InnerText = orgUnit.Wards;

            XmlNode tenHuyenNNT = xmlDoc.GetElementsByTagName("tenHuyenNNT")[0];
            tenHuyenNNT.InnerText = orgUnit.Province;

            XmlNode tenTinhNNT = xmlDoc.GetElementsByTagName("tenTinhNNT")[0];
            tenTinhNNT.InnerText = orgUnit.District;

            XmlNode nodeTungay = xmlDoc.GetElementsByTagName("kyKKhaiTuNgay")[0];
            //nodeTungay.InnerText = ngay1;
            nodeTungay.InnerText = (tungay.Day < 10 ? "0" + tungay.Day.ToString() : tungay.Day.ToString()) + "/" + (tungay.Month < 10 ? "0" + tungay.Month.ToString() : tungay.Month.ToString()) + "/" + tungay.Year;

            XmlNode nodeDenngay = xmlDoc.GetElementsByTagName("kyKKhaiDenNgay")[0];
            //nodeDenngay.InnerText = ngay2;
            nodeDenngay.InnerText = (denngay.Day < 10 ? "0" + denngay.Day.ToString() : denngay.Day.ToString()) + "/" + (denngay.Month < 10 ? "0" + denngay.Month.ToString() : denngay.Month.ToString()) + "/" + denngay.Year;

            XmlNode nodeNgaylap = xmlDoc.GetElementsByTagName("ngayLapTKhai")[0];
            nodeNgaylap.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;

            XmlNode nodeNgayky = xmlDoc.GetElementsByTagName("ngayKy")[0];
            nodeNgayky.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;


            if (!lessthanHtkk452)
            {
                if (career != null)
                {
                    XmlNode nodeMaNganhNghe = xmlDoc.GetElementsByTagName("ma_NganhNghe")[0];
                    nodeMaNganhNghe.InnerText = career.Code;

                    XmlNode nodeTenNganhNghe = xmlDoc.GetElementsByTagName("ten_NganhNghe")[0];
                    nodeTenNganhNghe.InnerText = career.Name;
                }
            }

            var defaultHtkkNumberCode = await _defaultHtkkNumberCodeService.GetQueryableAsync();
            var htkkNumberCode = defaultHtkkNumberCode.Where(p => p.CircularCode == "GTGT01").ToList();
            List<string> lstMaso = new List<string>();

            string _lstMaso = "";

            for (int i = 0; i < htkkNumberCode.Count; i++)
            {
                lstMaso.Add("[" + htkkNumberCode[i].NumberCode + "]");
                _lstMaso = _lstMaso + "[" + htkkNumberCode[i].NumberCode + "],";
            }

            for (int i = 0; i < dataStatementVAT.Count; i++)
            {
                if (!string.IsNullOrEmpty(dataStatementVAT[i].NumberCode1) && lstMaso.Contains(dataStatementVAT[i].NumberCode1))
                {
                    string ms = dataStatementVAT[i].NumberCode1.Replace("[", "").Replace("]", "");

                    if (xmlDoc.GetElementsByTagName("ct" + ms).Count > 0)
                    {
                        XmlNode node = xmlDoc.GetElementsByTagName("ct" + ms)[0];
                        node.InnerText = Convert.ToDouble(dataStatementVAT[i].AmountWithoutVat).ToString();
                    }

                }

                if (!string.IsNullOrEmpty(dataStatementVAT[i].NumberCode2) && _lstMaso.Contains(dataStatementVAT[i].NumberCode2))
                {
                    string ms = dataStatementVAT[i].NumberCode2.Replace("[", "").Replace("]", "");

                    if (xmlDoc.GetElementsByTagName("ct" + ms).Count > 0)
                    {
                        XmlNode node = xmlDoc.GetElementsByTagName("ct" + ms)[0];
                        node.InnerText = Convert.ToDouble(dataStatementVAT[i].AmountVat).ToString();
                    }

                }
            }
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            String XmlString = sw.ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(XmlString);

            return new FileContentResult(bytes, MIMETYPE.GetContentType("xml"))
            {
                FileDownloadName = $"GTGT01.xml"
            };
        }

        public async Task<FileContentResult> ExportXmlGtgtPlAsync(ExportXmlGTGT01ParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string fileName = "01_GTGT_PL.xml";
            bool lessthanHtkk452 = true;
            var htkkVersion = await _tenantSettingService.GetTenantSettingByKeyAsync("HTKK_VERSION", orgCode);
            var orgUnit = await _orgUnitService.GetByCodeAsync(orgCode);
            var career = (await _careerService.GetQueryableAsync()).Where(p => p.Id == orgUnit.CareerId).FirstOrDefault();
            string path = _hostingEnvironment.WebRootPath + @"/Content/HTKK/" + fileName;

            // lấy dữ liệu báo cáo StatementOfValueAddedTax
            var statementOfValueAddedTaxParameterDto = new StatementOfValueAddedTaxParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                Extend = dto.Extend,
                DeductPre = dto.DeductPre,
                IncreasePre = dto.IncreasePre,
                ReducePre = dto.ReducePre,
                SuggestionReturn = dto.SuggestionReturn,
            };
            var statementVATParameter = new ReportRequestDto<StatementOfValueAddedTaxParameterDto>
            {
                Parameters = statementOfValueAddedTaxParameterDto,
            };
            var dataStatementVAT = (await _statementOfValueAddedBusiness.CreateDataAsync(statementVATParameter)).Data;
            XmlDocument xmlDoc = new XmlDocument();
            //load the xml into the XmlDocument
            xmlDoc.Load(path);
            string thang = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            string ngay = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();

            DateTime tungay = dto.FromDate;
            DateTime denngay = dto.ToDate;
            if (tungay.Day == 1 && tungay.Month == 1 && denngay.Day == 31 && denngay.Month == 3)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "1/" + tungay.Year;
            }
            else if (tungay.Day == 1 && tungay.Month == 4 && denngay.Day == 30 && denngay.Month == 6)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "2/" + tungay.Year;
            }
            else if (tungay.Day == 1 && tungay.Month == 7 && denngay.Day == 30 && denngay.Month == 9)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "3/" + tungay.Year;
            }
            else if (tungay.Day == 1 && tungay.Month == 10 && denngay.Day == 31 && denngay.Month == 12)
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "Q";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = "4/" + tungay.Year;
            }
            else
            {
                XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
                kieuKy.InnerText = "M";
                XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
                kyKKhai.InnerText = tungay.Month < 10 ? "0" + tungay.Month.ToString() + "/" + tungay.Year : tungay.Month.ToString() + "/" + tungay.Year;
            }

            XmlNode mst = xmlDoc.GetElementsByTagName("mst")[0];
            mst.InnerText = orgUnit.TaxCode;

            XmlNode tenNNT = xmlDoc.GetElementsByTagName("tenNNT")[0];
            tenNNT.InnerText = orgUnit.NameE;

            XmlNode dchiNNT = xmlDoc.GetElementsByTagName("dchiNNT")[0];
            dchiNNT.InnerText = orgUnit.Address;

            XmlNode phuongXa = xmlDoc.GetElementsByTagName("phuongXa")[0];
            phuongXa.InnerText = orgUnit.Wards;

            XmlNode tenHuyenNNT = xmlDoc.GetElementsByTagName("tenHuyenNNT")[0];
            tenHuyenNNT.InnerText = orgUnit.Province;

            XmlNode tenTinhNNT = xmlDoc.GetElementsByTagName("tenTinhNNT")[0];
            tenTinhNNT.InnerText = orgUnit.District;

            XmlNode nodeTungay = xmlDoc.GetElementsByTagName("kyKKhaiTuNgay")[0];
            //nodeTungay.InnerText = ngay1;
            nodeTungay.InnerText = (tungay.Day < 10 ? "0" + tungay.Day.ToString() : tungay.Day.ToString()) + "/" + (tungay.Month < 10 ? "0" + tungay.Month.ToString() : tungay.Month.ToString()) + "/" + tungay.Year;

            XmlNode nodeDenngay = xmlDoc.GetElementsByTagName("kyKKhaiDenNgay")[0];
            //nodeDenngay.InnerText = ngay2;
            nodeDenngay.InnerText = (denngay.Day < 10 ? "0" + denngay.Day.ToString() : denngay.Day.ToString()) + "/" + (denngay.Month < 10 ? "0" + denngay.Month.ToString() : denngay.Month.ToString()) + "/" + denngay.Year;

            XmlNode nodeNgaylap = xmlDoc.GetElementsByTagName("ngayLapTKhai")[0];
            nodeNgaylap.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;

            XmlNode nodeNgayky = xmlDoc.GetElementsByTagName("ngayKy")[0];
            nodeNgayky.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;


            if (!lessthanHtkk452)
            {
                if (career != null)
                {
                    XmlNode nodeMaNganhNghe = xmlDoc.GetElementsByTagName("ma_NganhNghe")[0];
                    nodeMaNganhNghe.InnerText = career.Code;

                    XmlNode nodeTenNganhNghe = xmlDoc.GetElementsByTagName("ten_NganhNghe")[0];
                    nodeTenNganhNghe.InnerText = career.Name;
                }
            }

            var defaultHtkkNumberCode = await _defaultHtkkNumberCodeService.GetQueryableAsync();
            var htkkNumberCode = defaultHtkkNumberCode.Where(p => p.CircularCode == "GTGT01").ToList();
            List<string> lstMaso = new List<string>();

            string _lstMaso = "";

            for (int i = 0; i < htkkNumberCode.Count; i++)
            {
                lstMaso.Add("[" + htkkNumberCode[i].NumberCode + "]");
                _lstMaso = _lstMaso + "[" + htkkNumberCode[i].NumberCode + "],";
            }

            for (int i = 0; i < dataStatementVAT.Count; i++)
            {
                if (!string.IsNullOrEmpty(dataStatementVAT[i].NumberCode1) && lstMaso.Contains(dataStatementVAT[i].NumberCode1))
                {
                    string ms = dataStatementVAT[i].NumberCode1.Replace("[", "").Replace("]", "");

                    if (xmlDoc.GetElementsByTagName("ct" + ms).Count > 0)
                    {
                        XmlNode node = xmlDoc.GetElementsByTagName("ct" + ms)[0];
                        node.InnerText = Convert.ToDouble(dataStatementVAT[i].AmountWithoutVat).ToString();
                    }

                }

                if (!string.IsNullOrEmpty(dataStatementVAT[i].NumberCode2) && _lstMaso.Contains(dataStatementVAT[i].NumberCode2))
                {
                    string ms = dataStatementVAT[i].NumberCode2.Replace("[", "").Replace("]", "");

                    if (xmlDoc.GetElementsByTagName("ct" + ms).Count > 0)
                    {
                        XmlNode node = xmlDoc.GetElementsByTagName("ct" + ms)[0];
                        node.InnerText = Convert.ToDouble(dataStatementVAT[i].AmountVat).ToString();
                    }

                }
            }

            // lấy dữ liệu báo cáo StatementOfValueAddedTax
            var reducingVatUnderResolutionParameterDto = new TotalRevenueParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                PartnerCode = "",
                PartnerGroupCode = "",
                ProductCode = "",
                ProductGroupCode = "",
            };
            var reducingVatUnderResolutionParameter = new ReportRequestDto<TotalRevenueParameterDto>
            {
                Parameters = reducingVatUnderResolutionParameterDto,
            };
            var reducingVatUnderResolution = (await _reducingVatUnderResolutionBusiness.CreateDataAsync(reducingVatUnderResolutionParameter)).Data;
            decimal TongDoanhThu = 0;
            decimal ThueGiam = 0;
            // add an edition elementfo
            if (reducingVatUnderResolution.Count == 1)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ExportXml, "500"),
                        $"Không có dữ liệu Giảm thuế GTGT theo nghị quyết 43/2022/NQ-QH15");
            }
            for (int i = 0; i < (reducingVatUnderResolution.Count - 1); i++)
            {
                var c = i + 1;
                XmlElement newEle = xmlDoc.CreateElement("BangKeTenHHDV");
                newEle.SetAttribute("ID", "ID_" + c);
                newEle.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");

                XmlElement tenHHDV = xmlDoc.CreateElement("tenHHDV");
                tenHHDV.InnerText = reducingVatUnderResolution[i].ProductName;
                newEle.AppendChild(tenHHDV);

                XmlElement giaTriHHDV = xmlDoc.CreateElement("giaTriHHDV");
                giaTriHHDV.InnerText = reducingVatUnderResolution[i].Turnover.ToString() == "0" || reducingVatUnderResolution[i].Turnover.ToString() == "" ? "0" : (String.Format("{0:0.####}", reducingVatUnderResolution[i].Turnover));
                newEle.AppendChild(giaTriHHDV);

                XmlElement thueSuatTheoQuyDinh = xmlDoc.CreateElement("thueSuatTheoQuyDinh");
                thueSuatTheoQuyDinh.InnerText = reducingVatUnderResolution[i].VatPercentage.ToString() == "0" || reducingVatUnderResolution[i].VatPercentage.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(reducingVatUnderResolution[i].VatPercentage.ToString())));
                newEle.AppendChild(thueSuatTheoQuyDinh);

                XmlElement thueSuatSauGiam = xmlDoc.CreateElement("thueSuatSauGiam");
                thueSuatSauGiam.InnerText = reducingVatUnderResolution[i].ReductionPercent.ToString() == "0" || reducingVatUnderResolution[i].ReductionPercent.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(reducingVatUnderResolution[i].ReductionPercent.ToString())));
                newEle.AppendChild(thueSuatSauGiam);

                XmlElement thueGTGTDuocGiam = xmlDoc.CreateElement("thueGTGTDuocGiam");
                thueGTGTDuocGiam.InnerText = reducingVatUnderResolution[i].VatReduction.ToString() == "0" || reducingVatUnderResolution[i].VatReduction.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(reducingVatUnderResolution[i].VatReduction.ToString())));
                newEle.AppendChild(thueGTGTDuocGiam);

                var x = xmlDoc.GetElementsByTagName("PL_NQ101_GTGT")[0];
                x.AppendChild(newEle);
                TongDoanhThu += decimal.Parse(reducingVatUnderResolution[i].Turnover.ToString());
                ThueGiam += decimal.Parse(reducingVatUnderResolution[i].VatReduction.ToString());
            }
            var PL_NQ101_GTGT = xmlDoc.GetElementsByTagName("PL_NQ101_GTGT")[0];
            XmlElement tongCongGiaTriHHDV = xmlDoc.CreateElement("tongCongGiaTriHHDV");
            tongCongGiaTriHHDV.InnerText = String.Format("{0:0.####}", TongDoanhThu);
            tongCongGiaTriHHDV.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");
            PL_NQ101_GTGT.AppendChild(tongCongGiaTriHHDV);

            XmlElement tongCongThueGTGTDuocGiam = xmlDoc.CreateElement("tongCongThueGTGTDuocGiam");
            tongCongThueGTGTDuocGiam.InnerText = String.Format("{0:0.####}", ThueGiam);
            tongCongThueGTGTDuocGiam.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");
            PL_NQ101_GTGT.AppendChild(tongCongThueGTGTDuocGiam);

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            String XmlString = sw.ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(XmlString);

            return new FileContentResult(bytes, MIMETYPE.GetContentType("xml"))
            {
                FileDownloadName = $"GTGT01.xml"
            };
        }

        public async Task<FileContentResult> ExportXmlBctc406Async(ExportXmlBctc406ParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var orgUnit = await _orgUnitService.GetByCodeAsync(orgCode);
            var yearCategory = await _yearCategoryService.GetByYearAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            var usingDecision = yearCategory.UsingDecision;
            string path = _hostingEnvironment.WebRootPath + @"/Content/HTKK/" + usingDecision + ".xml";
            // lấy dữ liệu báo cáo AccBalanceSheetReport
            var accBalanceSheetParameterDto = new AccBalanceSheetParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                UsingDecision = usingDecision,
            };
            var accBalanceSheetParameter = new ReportRequestDto<AccBalanceSheetParameterDto>
            {
                Parameters = accBalanceSheetParameterDto,
            };
            var dataAccBalanceSheet = (await _accBalanceSheetReportBusiness.CreateDataAsync(accBalanceSheetParameter)).Data;

            // lấy dữ liệu báo cáo BusinessResult
            var businessResultParameterDto = new CashFlowBResultParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                FromDateLast = dto.FromDatePre,
                ToDateLast = dto.ToDatePre,
                UsingDecision = usingDecision,
            };
            var businessResultParameter = new ReportRequestDto<CashFlowBResultParameterDto>
            {
                Parameters = businessResultParameterDto,
            };
            var dataBusinessResult = (await _businessResultBusiness.CreateDataAsync(businessResultParameter)).Data;

            // lấy dữ liệu báo cáo CashFlow
            var dataCashFlow = (await _cashFlowBusiness.CreateDataAsync(businessResultParameter)).Data;

            // lấy dữ liệu báo cáo BalanceSheetAcc
            var balanceSheetAccParameterDto = new ReportBaseParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                AccCode = "",
                AccRank = 3,

            };
            var balanceSheetAccParameter = new ReportRequestDto<ReportBaseParameterDto>
            {
                Parameters = balanceSheetAccParameterDto,
            };
            var dataBalanceSheetAcc = (await _balanceSheetAccBusiness.CreateDataAsync(balanceSheetAccParameter)).Data;

            //Lấy danh sách mã số HTKK 
            var defaultHtkkNumberCode = await _defaultHtkkNumberCodeService.GetQueryableAsync();
            var lstNumberCode = defaultHtkkNumberCode.Where(p => p.CircularCode.Substring(4,3) == usingDecision.ToString()).Select(p => p.NumberCode).ToList();

            //Lấy thông tin nộp thuế DVCS
            string _mst = orgUnit.TaxCode;
            string _macqt = orgUnit.TaxAuthorityCode;
            string _tencqt = orgUnit.TaxAuthorityName;
            string _nguoiky = orgUnit.Signee;
            string _dv = orgUnit.NameE;
            string _dc = orgUnit.Address;
            string _xa = orgUnit.Wards;
            string _huyen = orgUnit.District;
            string _tinh = orgUnit.Province;
            string _nguoilap = orgUnit.Producer;
            string _ktt = orgUnit.ChiefAccountant;
            string _gd = orgUnit.Director;

            //tạo file xml
            XmlDocument xmlDoc = new XmlDocument();
            //load the xml into the XmlDocument
            xmlDoc.Load(path);

            string thang = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            string ngay = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();

            DateTime tungay = Convert.ToDateTime(dto.FromDate);
            DateTime denngay = Convert.ToDateTime(dto.ToDate);

            XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
            kyKKhai.InnerText = tungay.Year.ToString();

            XmlNode nodeTungay = xmlDoc.GetElementsByTagName("kyKKhaiTuNgay")[0];
            nodeTungay.InnerText = (tungay.Day < 10 ? "0" + tungay.Day.ToString() : tungay.Day.ToString()) + "/" + (tungay.Month < 10 ? "0" + tungay.Month.ToString() : tungay.Month.ToString()) + "/" + tungay.Year;

            XmlNode nodeDenngay = xmlDoc.GetElementsByTagName("kyKKhaiDenNgay")[0];
            nodeDenngay.InnerText = (denngay.Day < 10 ? "0" + denngay.Day.ToString() : denngay.Day.ToString()) + "/" + (denngay.Month < 10 ? "0" + denngay.Month.ToString() : denngay.Month.ToString()) + "/" + denngay.Year;

            XmlNode nodengayLapTKhai = xmlDoc.GetElementsByTagName("ngayLapTKhai")[0];
            nodengayLapTKhai.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;

            XmlNode nodeNgayky = xmlDoc.GetElementsByTagName("ngayKy")[0];
            nodeNgayky.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;

            XmlNodeList nodengayLaps = xmlDoc.GetElementsByTagName("ngayLap");

            if (nodengayLaps != null)
            {
                for (int i = 0; i < nodengayLaps.Count; i++)
                {
                    XmlNode node = nodengayLaps[i];
                    node.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;
                }
            }

            XmlNode nodemaCQTNoiNop = xmlDoc.GetElementsByTagName("maCQTNoiNop")[0];
            nodemaCQTNoiNop.InnerText = _macqt;

            XmlNode nodetenCQTNoiNop = xmlDoc.GetElementsByTagName("tenCQTNoiNop")[0];
            nodetenCQTNoiNop.InnerText = _tencqt;

            XmlNode nodenguoiKy = xmlDoc.GetElementsByTagName("nguoiKy")[0];
            nodenguoiKy.InnerText = _nguoiky;

            XmlNode mstNNT = xmlDoc.GetElementsByTagName("mst")[0];
            mstNNT.InnerText = _mst;

            XmlNode nodetenNNT = xmlDoc.GetElementsByTagName("tenNNT")[0];
            nodetenNNT.InnerText = _dv;

            XmlNode nodedchiNNT = xmlDoc.GetElementsByTagName("dchiNNT")[0];
            nodedchiNNT.InnerText = _dc;

            XmlNode nodedphuongXa = xmlDoc.GetElementsByTagName("phuongXa")[0];
            nodedphuongXa.InnerText = _xa;

            XmlNode nodedtenHuyenNNT = xmlDoc.GetElementsByTagName("tenHuyenNNT")[0];
            nodedtenHuyenNNT.InnerText = _huyen;

            XmlNode nodedtenTinhNNT = xmlDoc.GetElementsByTagName("tenTinhNNT")[0];
            nodedtenTinhNNT.InnerText = _tinh;

            XmlNode nodednguoiLapBieu = xmlDoc.GetElementsByTagName("nguoiLapBieu")[0];
            if (nodednguoiLapBieu != null)
                nodednguoiLapBieu.InnerText = _nguoilap;

            XmlNode nodedkeToanTruong = xmlDoc.GetElementsByTagName("keToanTruong")[0];
            if (nodedkeToanTruong != null)
                nodedkeToanTruong.InnerText = _ktt;

            XmlNodeList nodedgiamDocs = xmlDoc.GetElementsByTagName("giamDoc");

            foreach (XmlNode node in nodedgiamDocs)
            {
                node.InnerText = _gd;
            }

            string _chitieuTC = usingDecision == 200 ? "CDKT_HoatDongLienTuc" : "CTieuTKhaiChinh";

            for (int i = 0; i < dataAccBalanceSheet.Count; i++)
            {
                if (lstNumberCode.Contains(dataAccBalanceSheet[i].NumberCode.ToString()))
                {
                    XmlNodeList xnList = xmlDoc.GetElementsByTagName("ct" + dataAccBalanceSheet[i].NumberCode.ToString());
                    foreach (XmlNode nd in xnList)
                    {
                        if (nd.ParentNode.ParentNode.Name == _chitieuTC && nd.ParentNode.Name == "ThuyetMinh")
                            nd.InnerText = dataAccBalanceSheet[i].TargetCode.ToString();
                        if (nd.ParentNode.ParentNode.Name == _chitieuTC && nd.ParentNode.Name == "SoCuoiNam")
                            nd.InnerText = dataAccBalanceSheet[i].EndingAmount == null ? "0" : Convert.ToInt64(dataAccBalanceSheet[i].EndingAmount).ToString();
                        if (nd.ParentNode.ParentNode.Name == _chitieuTC && nd.ParentNode.Name == "SoDauNam")
                            nd.InnerText = dataAccBalanceSheet[i].OpeningAmount == null ? "0" : Convert.ToInt64(dataAccBalanceSheet[i].OpeningAmount).ToString();
                    }}
            }

            for (int i = 0; i < dataBusinessResult.Count; i++)
            {
                if (lstNumberCode.Contains(dataBusinessResult[i].NumberCode))
                {
                    XmlNodeList xnList = xmlDoc.GetElementsByTagName("ct" + dataBusinessResult[i].NumberCode);
                    foreach (XmlNode nd in xnList)
                    {
                        if (nd.ParentNode.ParentNode.Name == "PL_KQHDSXKD" && nd.ParentNode.Name == "ThuyetMinh")
                            nd.InnerText = dataBusinessResult[i].TargetCode;
                        if (nd.ParentNode.ParentNode.Name == "PL_KQHDSXKD" && nd.ParentNode.Name == "NamNay")
                            nd.InnerText = dataBusinessResult[i].ThisPeriod == null ? "0" : Convert.ToInt64(dataBusinessResult[i].ThisPeriod).ToString();
                        if (nd.ParentNode.ParentNode.Name == "PL_KQHDSXKD" && nd.ParentNode.Name == "NamTruoc")
                            nd.InnerText = dataBusinessResult[i].LastPeriod == null ? "0" : Convert.ToInt64(dataBusinessResult[i].LastPeriod).ToString();
                    }
                }
            }

            for (int i = 0; i < dataCashFlow.Count; i++)
            {
                int soam = Convert.ToInt32(dataCashFlow[i].Negative);

                if (lstNumberCode.Contains(dataCashFlow[i].NumberCode))
                {
                    XmlNodeList xnList = xmlDoc.GetElementsByTagName("ct" + dataCashFlow[i].NumberCode);
                    foreach (XmlNode nd in xnList)
                    {
                        if (nd.ParentNode.ParentNode.Name == "PL_LCTTTT" && nd.ParentNode.Name == "ThuyetMinh")
                            nd.InnerText = dataCashFlow[i].TargetCode;
                        if (nd.ParentNode.ParentNode.Name == "PL_LCTTTT" && nd.ParentNode.Name == "NamNay")
                        {
                            Int64 kynay = dataCashFlow[i].ThisPeriod == null ? 0 : Convert.ToInt64(dataCashFlow[i].ThisPeriod) * soam;
                            nd.InnerText = kynay.ToString();
                        }
                        if (nd.ParentNode.ParentNode.Name == "PL_LCTTTT" && nd.ParentNode.Name == "NamTruoc")
                        {
                            Int64 kytruoc = dataCashFlow[i].LastPeriod == null ? 0 : Convert.ToInt64(dataCashFlow[i].LastPeriod) * soam;
                            nd.InnerText = kytruoc.ToString();
                        }
                    }
                }
            }

            for (int i = 0; i < dataBalanceSheetAcc.Count; i++)
            {
                if (lstNumberCode.Contains(dataBalanceSheetAcc[i].AccCode))
                {
                    XmlNodeList xnList = xmlDoc.GetElementsByTagName("ct" + dataBalanceSheetAcc[i].AccCode);
                    foreach (XmlNode nd in xnList)
                    {
                        if (nd.ParentNode.ParentNode.Name == "SoDuDauKy")
                        {
                            if (nd.ParentNode.Name == "No")
                                nd.InnerText = dataBalanceSheetAcc[i].Debit1 == null ? "0" : Convert.ToInt64(dataBalanceSheetAcc[i].Debit1).ToString();
                            if (nd.ParentNode.Name == "Co")
                                nd.InnerText = dataBalanceSheetAcc[i].Credit1 == null ? "0" : Convert.ToInt64(dataBalanceSheetAcc[i].Credit1).ToString();
                        }
                        if (nd.ParentNode.ParentNode.Name == "SoPhatSinhTrongKy")
                        {
                            if (nd.ParentNode.Name == "No")
                                nd.InnerText = dataBalanceSheetAcc[i].DebitIncurred == null ? "0" : Convert.ToInt64(dataBalanceSheetAcc[i].DebitIncurred).ToString();
                            if (nd.ParentNode.Name == "Co")
                                nd.InnerText = dataBalanceSheetAcc[i].CreditIncurred == null ? "0" : Convert.ToInt64(dataBalanceSheetAcc[i].CreditIncurred).ToString();
                        }
                        if (nd.ParentNode.ParentNode.Name == "SoDuCuoiKy")
                        {
                            if (nd.ParentNode.Name == "No")
                                nd.InnerText = dataBalanceSheetAcc[i].Debit2 == null ? "0" : Convert.ToInt64(dataBalanceSheetAcc[i].Debit2).ToString();
                            if (nd.ParentNode.Name == "Co")
                                nd.InnerText = dataBalanceSheetAcc[i].Credit2 == null ? "0" : Convert.ToInt64(dataBalanceSheetAcc[i].Credit2).ToString();
                        }
                    }
                }
            }
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            String XmlString = sw.ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(XmlString);
            return new FileContentResult(bytes, MIMETYPE.GetContentType("xml"))
            {
                FileDownloadName = usingDecision + ".xml"
            };
        }

        public async Task<FileContentResult> ExportXmlHKDAsync(ExportXmlHKDParameterDto dto)
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var orgUnit = (await _orgUnitService.GetQueryableAsync()).Where(p => p.Code == orgCode).FirstOrDefault();
            if (orgUnit == null) throw new Exception("Không tìm thấy DVCS " + orgCode);
            var p_Ngay_ct1 = dto.FromDate;
            var p_Ngay_ct2 = dto.ToDate;

            string path = _hostingEnvironment.WebRootPath + @"/Content/HTKK/TKHKD.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNode mst = xmlDoc.GetElementsByTagName("mst")[0];
            mst.InnerText = orgUnit.TaxCode;

            XmlNode tenNNT = xmlDoc.GetElementsByTagName("tenNNT")[0];
            tenNNT.InnerText = orgUnit.NameE;

            XmlNode dchiNNT = xmlDoc.GetElementsByTagName("dchiNNT")[0];
            dchiNNT.InnerText = orgUnit.Address;

            XmlNode phuongXa = xmlDoc.GetElementsByTagName("phuongXa")[0];
            phuongXa.InnerText = orgUnit.Wards;

            XmlNode tenHuyenNNT = xmlDoc.GetElementsByTagName("tenHuyenNNT")[0];
            tenHuyenNNT.InnerText = orgUnit.Province;

            XmlNode tenTinhNNT = xmlDoc.GetElementsByTagName("tenTinhNNT")[0];
            tenTinhNNT.InnerText = orgUnit.District;

            XmlNode maCQTNoiNop = xmlDoc.GetElementsByTagName("maCQTNoiNop")[0];
            maCQTNoiNop.InnerText = orgUnit.TaxAuthorityCode;

            XmlNode nodeTenCQTNoiNop = xmlDoc.GetElementsByTagName("tenCQTNoiNop")[0];
            nodeTenCQTNoiNop.InnerText = orgUnit.TaxAuthorityName;

            string thang = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            string ngay = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string tu_ngay = Convert.ToString(Convert.ToDateTime(p_Ngay_ct1).Day + "/" + Convert.ToDateTime(p_Ngay_ct1).Month + "/" + Convert.ToDateTime(p_Ngay_ct1).Year);
            string den_ngay = Convert.ToString(Convert.ToDateTime(p_Ngay_ct2).Day + "/" + Convert.ToDateTime(p_Ngay_ct2).Month + "/" + Convert.ToDateTime(p_Ngay_ct2).Year);
            DateTime tungay = Convert.ToDateTime(p_Ngay_ct1);
            DateTime denngay = Convert.ToDateTime(p_Ngay_ct2);
            string ngay_tuNgay = tungay.Day >= 10 ? tungay.Day.ToString() : "0" + tungay.Day.ToString();
            string thang_tuNgay = tungay.Month >= 10 ? tungay.Month.ToString() : "0" + tungay.Month.ToString();

            string ngay_denngay = denngay.Day >= 10 ? denngay.Day.ToString() : "0" + denngay.Day.ToString();
            string thang_denngay = denngay.Month >= 10 ? denngay.Month.ToString() : "0" + denngay.Month.ToString();
            XmlNode kieuKy = xmlDoc.GetElementsByTagName("kieuKy")[0];
            XmlNode kyKKhai = xmlDoc.GetElementsByTagName("kyKKhai")[0];
            if (thang_tuNgay != thang_denngay)
            {
                // Trường hợp đẩy theo quý
                kieuKy.InnerText = "Q";
                if (denngay.Month <= 3)
                {
                    kyKKhai.InnerText = "1" + "/" + denngay.Year.ToString();
                }
                else if (3 < denngay.Month && denngay.Month <= 6)
                {
                    kyKKhai.InnerText = "2" + "/" + denngay.Year.ToString();
                }
                else if (6 < denngay.Month && denngay.Month <= 9)
                {
                    kyKKhai.InnerText = "3" + "/" + denngay.Year.ToString();
                }
                else
                {
                    kyKKhai.InnerText = "4" + "/" + denngay.Year.ToString();
                }
                XmlNode kyKKhaiTuThang = xmlDoc.GetElementsByTagName("kyKKhaiTuThang")[0];
                kyKKhaiTuThang.InnerText = thang_tuNgay + "/" + tungay.Year.ToString();

                XmlNode kyKKhaiDenThang = xmlDoc.GetElementsByTagName("kyKKhaiDenThang")[0];
                kyKKhaiDenThang.InnerText = thang_denngay + "/" + denngay.Year.ToString();
            }
            else
            {
                // Trường hợp đẩy theo tháng
                kieuKy.InnerText = "M";
                kyKKhai.InnerText = thang_denngay + "/" + denngay.Year.ToString();
            }



            XmlNode kyKKhaiTuNgay = xmlDoc.GetElementsByTagName("kyKKhaiTuNgay")[0];
            kyKKhaiTuNgay.InnerText = ngay_tuNgay + "/" + thang_tuNgay + "/" + tungay.Year.ToString();

            XmlNode kyKKhaiDenNgay = xmlDoc.GetElementsByTagName("kyKKhaiDenNgay")[0];
            kyKKhaiDenNgay.InnerText = ngay_denngay + "/" + thang_denngay + "/" + denngay.Year.ToString();

            XmlNode ngayLapTKhai = xmlDoc.GetElementsByTagName("ngayLapTKhai")[0];
            ngayLapTKhai.InnerText = DateTime.Now.Year + "-" + thang + "-" + ngay;
            XmlNode ngayKy = xmlDoc.GetElementsByTagName("ngayKy")[0];
            ngayKy.InnerText = denngay.Year.ToString() + "-" + thang_denngay + "-" + ngay_denngay;

            // lấy dữ liệu báo cáo TotalRevenue
            var totalRevenueParameterDto = new TotalRevenueParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                PartnerCode = dto.PartnerCode,
                PartnerGroupCode = dto.PartnerGroupCode,
                ProductCode = dto.ProductCode,
                ProductGroupCode = dto.ProductGroupCode,
            };
            var totalRevenueParameter = new ReportRequestDto<TotalRevenueParameterDto>
            {
                Parameters = totalRevenueParameterDto,
            };
            var dataTotalRevenue = (await _totalRevenueBusiness.CreateDataAsync(totalRevenueParameter)).Data;
            // lấy dữ liệu báo cáo TotalRevenue SummaryCost
            var summaryCostParamaterDto = new SummaryCostParamaterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
            };
            var summaryCostParamater = new ReportRequestDto<SummaryCostParamaterDto>
            {
                Parameters = summaryCostParamaterDto,
            };
            var dataSummaryCost = (await _summaryCostBusiness.CreateDataAsync(summaryCostParamater)).Data;
            // --------------------------

            for (int i = 0; i < dataTotalRevenue.Count; i++)
            {
                if (dataTotalRevenue[i].NumberCode == "[28]")
                {
                    XmlNode ct280 = xmlDoc.GetElementsByTagName("ct28")[0];
                    ct280.InnerText = dataTotalRevenue[i].Turnover.ToString() == "0" || dataTotalRevenue[i].Turnover.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Turnover.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[28]")
                {
                    XmlNode ct281 = xmlDoc.GetElementsByTagName("ct28")[1];
                    ct281.InnerText = dataTotalRevenue[i].Vat.ToString() == "0" || dataTotalRevenue[i].Vat.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Vat.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[28]")
                {
                    XmlNode ct282 = xmlDoc.GetElementsByTagName("ct28")[2];
                    ct282.InnerText = dataTotalRevenue[i].TurnoverPersonal.ToString() == "0" || dataTotalRevenue[i].TurnoverPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].TurnoverPersonal.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[28]")
                {
                    XmlNode ct283 = xmlDoc.GetElementsByTagName("ct28")[3];
                    ct283.InnerText = dataTotalRevenue[i].VatPersonal.ToString() == "0" || dataTotalRevenue[i].VatPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].VatPersonal.ToString())));
                }


                if (dataTotalRevenue[i].NumberCode.ToString() == "[29]")
                {
                    XmlNode ct290 = xmlDoc.GetElementsByTagName("ct29")[0];
                    ct290.InnerText = dataTotalRevenue[i].Turnover.ToString() == "0" || dataTotalRevenue[i].Turnover.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Turnover.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[29]")
                {
                    XmlNode ct291 = xmlDoc.GetElementsByTagName("ct29")[1];
                    ct291.InnerText = dataTotalRevenue[i].Vat.ToString() == "0" || dataTotalRevenue[i].Vat.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Vat.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[29]")
                {
                    XmlNode ct292 = xmlDoc.GetElementsByTagName("ct29")[2];
                    ct292.InnerText = dataTotalRevenue[i].TurnoverPersonal.ToString() == "0" || dataTotalRevenue[i].TurnoverPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].TurnoverPersonal.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[29]")
                {
                    XmlNode ct293 = xmlDoc.GetElementsByTagName("ct29")[3];
                    ct293.InnerText = dataTotalRevenue[i].VatPersonal.ToString() == "0" || dataTotalRevenue[i].VatPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].VatPersonal.ToString())));
                }


                if (dataTotalRevenue[i].NumberCode.ToString() == "[30]")
                {
                    XmlNode ct300 = xmlDoc.GetElementsByTagName("ct30")[0];
                    ct300.InnerText = dataTotalRevenue[i].Turnover.ToString() == "0" || dataTotalRevenue[i].Turnover.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Turnover.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[30]")
                {
                    XmlNode ct301 = xmlDoc.GetElementsByTagName("ct30")[1];
                    ct301.InnerText = dataTotalRevenue[i].Vat.ToString() == "0" || dataTotalRevenue[i].Vat.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Vat.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[30]")
                {
                    XmlNode ct302 = xmlDoc.GetElementsByTagName("ct30")[2];
                    ct302.InnerText = dataTotalRevenue[i].TurnoverPersonal.ToString() == "0" || dataTotalRevenue[i].TurnoverPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].TurnoverPersonal.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[30]")
                {
                    XmlNode ct303 = xmlDoc.GetElementsByTagName("ct30")[3];
                    ct303.InnerText = dataTotalRevenue[i].VatPersonal.ToString() == "0" || dataTotalRevenue[i].VatPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].VatPersonal.ToString())));
                }



                if (dataTotalRevenue[i].NumberCode.ToString() == "[31]")
                {
                    XmlNode ct310 = xmlDoc.GetElementsByTagName("ct31")[0];
                    ct310.InnerText = dataTotalRevenue[i].Turnover.ToString() == "0" || dataTotalRevenue[i].Turnover.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Turnover.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[31]")
                {
                    XmlNode ct311 = xmlDoc.GetElementsByTagName("ct31")[1];
                    ct311.InnerText = dataTotalRevenue[i].Vat.ToString() == "0" || dataTotalRevenue[i].Vat.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Vat.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[31]")
                {
                    XmlNode ct312 = xmlDoc.GetElementsByTagName("ct31")[2];
                    ct312.InnerText = dataTotalRevenue[i].TurnoverPersonal.ToString() == "0" || dataTotalRevenue[i].TurnoverPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].TurnoverPersonal.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[31]")
                {
                    XmlNode ct313 = xmlDoc.GetElementsByTagName("ct31")[3];
                    ct313.InnerText = dataTotalRevenue[i].VatPersonal.ToString() == "0" || dataTotalRevenue[i].VatPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].VatPersonal.ToString())));
                }


                if (dataTotalRevenue[i].NumberCode.ToString() == "[32]")
                {
                    XmlNode ct320 = xmlDoc.GetElementsByTagName("ct32")[0];
                    ct320.InnerText = dataTotalRevenue[i].Turnover.ToString() == "0" || dataTotalRevenue[i].Turnover.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Turnover.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[32]")
                {
                    XmlNode ct321 = xmlDoc.GetElementsByTagName("ct32")[1];
                    ct321.InnerText = dataTotalRevenue[i].Vat.ToString() == "0" || dataTotalRevenue[i].Vat.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].Vat.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[32]")
                {
                    XmlNode ct322 = xmlDoc.GetElementsByTagName("ct32")[2];
                    ct322.InnerText = dataTotalRevenue[i].TurnoverPersonal.ToString() == "0" || dataTotalRevenue[i].TurnoverPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].TurnoverPersonal.ToString())));
                }
                if (dataTotalRevenue[i].NumberCode.ToString() == "[32]")
                {
                    XmlNode ct323 = xmlDoc.GetElementsByTagName("ct32")[3];
                    ct323.InnerText = dataTotalRevenue[i].VatPersonal.ToString() == "0" || dataTotalRevenue[i].VatPersonal.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataTotalRevenue[i].VatPersonal.ToString())));
                }
            }

            for (int i = 0; i < dataSummaryCost.Count; i++)
            {

                if (dataSummaryCost[i].NumberCode.ToString() == "[24]")
                {
                    XmlNode ct323 = xmlDoc.GetElementsByTagName("ct24")[1];
                    ct323.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));
                }
                if (dataSummaryCost[i].NumberCode.ToString() == "[25]")
                {
                    XmlNode ct25 = xmlDoc.GetElementsByTagName("ct25")[1];
                    ct25.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));
                }
                if (dataSummaryCost[i].NumberCode.ToString() == "[26]")
                {
                    XmlNode ct26 = xmlDoc.GetElementsByTagName("ct26")[1];
                    ct26.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));


                }
                if (dataSummaryCost[i].NumberCode.ToString() == "[27]")
                {
                    XmlNode ct27 = xmlDoc.GetElementsByTagName("ct27")[1];
                    ct27.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));
                }


                XmlNode xnList = xmlDoc.GetElementsByTagName("ChiPhiQL")[0];
                foreach (XmlNode nd in xnList)
                {
                    if (nd.Name == "ct28")
                    {
                        if (dataSummaryCost[i].NumberCode.ToString() == "[28]")
                        {
                            nd.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));
                        }
                    }
                    if (nd.Name == "ct29")
                    {
                        if (dataSummaryCost[i].NumberCode.ToString() == "[29]")
                        {
                            nd.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));
                        }
                    }
                    if (nd.Name == "ct30")
                    {
                        if (dataSummaryCost[i].NumberCode.ToString() == "[30]")
                        {
                            nd.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));
                        }
                    }

                    if (nd.Name == "ct31")
                    {
                        if (dataSummaryCost[i].NumberCode.ToString() == "[31]")
                        {
                            nd.InnerText = dataSummaryCost[i].Amount.ToString() == "0" || dataSummaryCost[i].Amount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataSummaryCost[i].Amount.ToString())));
                        }
                    }
                }
            }


            var inventorySummaryBookParameterDto = new InventorySummaryBookParameterDto
            {
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
            };
            var inventorySummaryBookParameter = new ReportRequestDto<InventorySummaryBookParameterDto>
            {
                Parameters = inventorySummaryBookParameterDto,
            };
            var dataImportExport = (await _importExportBusiness.CreateDataAsync(inventorySummaryBookParameter)).Data;


            // add an edition elementfo
            for (int i = 0; i < dataImportExport.Count; i++)
            {
                var c = i + 1;
                var newEle = xmlDoc.CreateElement("CTietHKDCNKD");
                newEle.SetAttribute("id", "ID_" + c);
                newEle.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");

                XmlElement ct06 = xmlDoc.CreateElement("ct06");
                ct06.InnerText = dataImportExport[i].ProductName.ToString();
                newEle.AppendChild(ct06);

                XmlElement ct07 = xmlDoc.CreateElement("ct07");
                ct07.InnerText = dataImportExport[i].UnitCode.ToString();
                newEle.AppendChild(ct07);

                XmlElement ct08 = xmlDoc.CreateElement("ct08");
                ct08.InnerText = dataImportExport[i].ImportQuantity1.ToString() == "0" || dataImportExport[i].ImportQuantity1.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ImportQuantity1.ToString())));
                newEle.AppendChild(ct08);

                XmlElement ct09 = xmlDoc.CreateElement("ct09");
                ct09.InnerText = dataImportExport[i].ImportAmount1.ToString() == "0" || dataImportExport[i].ImportAmount1.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ImportAmount1.ToString())));
                newEle.AppendChild(ct09);

                XmlElement ct10 = xmlDoc.CreateElement("ct10");
                ct10.InnerText = dataImportExport[i].ImportQuantity.ToString() == "0" || dataImportExport[i].ImportQuantity.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ImportQuantity.ToString())));
                newEle.AppendChild(ct10);

                XmlElement ct11 = xmlDoc.CreateElement("ct11");
                ct11.InnerText = dataImportExport[i].ImportAmount.ToString() == "0" || dataImportExport[i].ImportAmount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ImportAmount.ToString())));
                newEle.AppendChild(ct11);

                XmlElement ct12 = xmlDoc.CreateElement("ct12");
                ct12.InnerText = dataImportExport[i].ExportQuantity.ToString() == "0" || dataImportExport[i].ExportQuantity.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ExportQuantity.ToString())));
                newEle.AppendChild(ct12);

                XmlElement ct13 = xmlDoc.CreateElement("ct13");
                ct13.InnerText = dataImportExport[i].ExportAmount.ToString() == "0" || dataImportExport[i].ExportAmount.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ExportAmount.ToString())));
                newEle.AppendChild(ct13);

                XmlElement ct14 = xmlDoc.CreateElement("ct14");
                ct14.InnerText = dataImportExport[i].ImportQuantity2.ToString() == "0" || dataImportExport[i].ImportQuantity2.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ImportQuantity2.ToString())));
                newEle.AppendChild(ct14);

                XmlElement ct15 = xmlDoc.CreateElement("ct15");
                ct15.InnerText = dataImportExport[i].ImportAmount2.ToString() == "0" || dataImportExport[i].ImportAmount2.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataImportExport[i].ImportAmount2.ToString())));
                newEle.AppendChild(ct15);




                var x = xmlDoc.GetElementsByTagName("BKeVLDCSPHH")[0];

                x.AppendChild(newEle);


            }

            var dataReducingVat = (await _reducingVatBusiness.CreateDataAsync(totalRevenueParameter)).Data;
            decimal TongDoanhThu = 0;
            decimal ThueGiam = 0;
            // add an edition elementfo
            if (dataReducingVat.Count - 1 > 0)
            {
                XmlElement Ele_PL_NQ101_GTGT = xmlDoc.CreateElement("PL_NQ101_GTGT");
                Ele_PL_NQ101_GTGT.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");
                for (int i = 0; i < (dataReducingVat.Count - 1); i++)
                {
                    var c = i + 1;
                    XmlElement newEle = xmlDoc.CreateElement("BangKeTenHHDV");
                    newEle.SetAttribute("ID", "ID_" + c);
                    newEle.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");

                    XmlElement nganhNghe_ma = xmlDoc.CreateElement("nganhNghe_ma");
                    nganhNghe_ma.InnerText = dataReducingVat[i].CareerCode.ToString();
                    newEle.AppendChild(nganhNghe_ma);

                    XmlElement nganhNghe_ten = xmlDoc.CreateElement("nganhNghe_ten");
                    nganhNghe_ten.InnerText = dataReducingVat[i].CareerName.ToString();
                    newEle.AppendChild(nganhNghe_ten);

                    XmlElement tenHHDV = xmlDoc.CreateElement("tenHHDV");
                    tenHHDV.InnerText = dataReducingVat[i].ProductName.ToString();
                    newEle.AppendChild(tenHHDV);

                    XmlElement giaTriHHDV = xmlDoc.CreateElement("giaTriHHDV");
                    giaTriHHDV.InnerText = dataReducingVat[i].Turnover.ToString() == "0" || dataReducingVat[i].Turnover.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataReducingVat[i].Turnover.ToString())));
                    newEle.AppendChild(giaTriHHDV);

                    XmlElement thueSuatTheoQuyDinh = xmlDoc.CreateElement("thueSuatTheoQuyDinh");
                    thueSuatTheoQuyDinh.InnerText = dataReducingVat[i].VatPercentage.ToString() == "0" || dataReducingVat[i].VatPercentage.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataReducingVat[i].VatPercentage.ToString())));
                    newEle.AppendChild(thueSuatTheoQuyDinh);

                    XmlElement thueSuatSauGiam = xmlDoc.CreateElement("thueSuatSauGiam");
                    thueSuatSauGiam.InnerText = dataReducingVat[i].ReductionPercent.ToString() == "0" || dataReducingVat[i].ReductionPercent.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataReducingVat[i].ReductionPercent.ToString())));
                    newEle.AppendChild(thueSuatSauGiam);

                    XmlElement thueGTGTDuocGiam = xmlDoc.CreateElement("thueGTGTDuocGiam");
                    thueGTGTDuocGiam.InnerText = dataReducingVat[i].VatReduction.ToString() == "0" || dataReducingVat[i].VatReduction.ToString() == "" ? "0" : (String.Format("{0:0.####}", Decimal.Parse(dataReducingVat[i].VatReduction.ToString())));
                    newEle.AppendChild(thueGTGTDuocGiam);

                    Ele_PL_NQ101_GTGT.AppendChild(newEle);
                    TongDoanhThu += decimal.Parse(dataReducingVat[i].Turnover.ToString());
                    ThueGiam += decimal.Parse(dataReducingVat[i].VatReduction.ToString());
                }
                XmlElement tongCongGiaTriHHDV = xmlDoc.CreateElement("tongCongGiaTriHHDV");
                tongCongGiaTriHHDV.InnerText = String.Format("{0:0.####}", TongDoanhThu);
                Ele_PL_NQ101_GTGT.AppendChild(tongCongGiaTriHHDV);

                XmlElement tongCongThueGTGTDuocGiam = xmlDoc.CreateElement("tongCongThueGTGTDuocGiam");
                tongCongThueGTGTDuocGiam.InnerText = String.Format("{0:0.####}", ThueGiam);
                Ele_PL_NQ101_GTGT.AppendChild(tongCongThueGTGTDuocGiam);

                var PLuc = xmlDoc.GetElementsByTagName("PLuc")[0];
                PLuc.AppendChild(Ele_PL_NQ101_GTGT);
            }


            string path2 = _hostingEnvironment.WebRootPath + @"/Content/HTKK/TKHKD2.xml";
            xmlDoc.Save(path2);

            var BKeVLDCSPHH = xmlDoc.GetElementsByTagName("BKeVLDCSPHH")[0];

            var ct17 = xmlDoc.CreateElement("ct17", "");
            ct17.InnerText = "0";

            ct17.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");
            BKeVLDCSPHH.AppendChild(ct17);
            var ct19 = xmlDoc.CreateElement("ct19");
            ct19.InnerText = "0";
            ct19.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");
            BKeVLDCSPHH.AppendChild(ct19);
            var ct21 = xmlDoc.CreateElement("ct21");
            ct21.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");
            ct21.InnerText = "0";
            BKeVLDCSPHH.AppendChild(ct21);

            var ct23 = xmlDoc.CreateElement("ct23");
            ct23.SetAttribute("xmlns", "http://kekhaithue.gdt.gov.vn/TKhaiThue");
            ct23.InnerText = "0";
            BKeVLDCSPHH.AppendChild(ct23);
            xmlDoc.OuterXml.Replace(" xmlns=\"http://kekhaithue.gdt.gov.vn/TKhaiThue\"", "");
            string path3 = _hostingEnvironment.WebRootPath + @"/Content/HTKK/TKHKD2.xml";
            xmlDoc.Save(path3);

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xmlDoc.WriteTo(xw);
            String XmlString = sw.ToString();

            byte[] bytes = Encoding.UTF8.GetBytes(XmlString);

            return new FileContentResult(bytes, MIMETYPE.GetContentType("xml"))
            {
                FileDownloadName = $"TKHKD.xml"
            };
        }

        #region Private
        #endregion
    }
}
