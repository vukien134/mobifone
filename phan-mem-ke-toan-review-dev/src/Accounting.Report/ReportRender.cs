using Accounting.Report.Constants;
using DevExpress.DataAccess.Json;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Accounting.Report
{
    public class ReportRender
    {
        #region Fields
        private RenderOption _renderOption;
        private XtraReport _report;
        #endregion
        #region Ctor        
        public ReportRender(RenderOption renderOption)
        {
            _renderOption = renderOption;
        }
        #endregion
        #region Methods
        public void CreateMultiReport(object reportDocument)
        {
            XtraReport report = (XtraReport)reportDocument;
            if (_report == null)
            {
                _report = new XtraReport();
            }

            for (int i = 0; i < report.Pages.Count; i++)
            {
                _report.Pages.Add(report.Pages[i]);
            }            
        }
        public object CreateDocument()
        {
            return this.RenderDocument();
        }
        public byte[] Execute()
        {
            var report = this.RenderDocument();
            return this.RenderOutputFile(report);
        }
        public byte[] ExportMultiReport()
        {
            return this.RenderOutputFile(_report);
        }
        #endregion
        #region Private
        private XtraReport CreateSubReport(XRSubreport xRSubreport)
        {
            string name = xRSubreport.Name.Replace("subrp_", "");
            string filename = name.Substring(0, name.Length - 3) + ".xml";
            string folder = Path.GetDirectoryName(_renderOption.TemplateFile);
            string path = Path.Combine(folder, filename);

            var report = new XtraReport();
            report.LoadLayoutFromXml(path);
            report.DataSource = _renderOption.DataSource;
            ChangeCurrencyFormat(report);

            return report;
        }
        private void ChangeCurrencyFormat(XtraReport report)
        {
            if (_renderOption.CurrencyFormats == null) return;

            var controls = report.AllControls<XRControl>().Where(p => !string.IsNullOrEmpty(p.Tag.ToString()));
            foreach(var control in controls)
            {
                string tag = control.Tag.ToString();
                if (!_renderOption.CurrencyFormats.ContainsKey(tag)) continue;
                string format = _renderOption.CurrencyFormats[tag];
                if (control is XRLabel)
                {
                    var label = control as XRLabel;
                    label.TextFormatString = "{0:" + format + "}";
                }
                if (control is XRTableCell)
                {
                    var tableCell = control as XRTableCell;
                    tableCell.TextFormatString = "{0:" + format + "}";
                }
            }
        }
        private XtraReport RenderDocument()
        {
            var report = new XtraReport();
            report.LoadLayoutFromXml(_renderOption.TemplateFile);
            report.DataSource = this.GetDataSource();
            ChangeCurrencyFormat(report);
            var subReports = report.AllControls<XRSubreport>();
            foreach (var xrSubReport in subReports)
            {
                var subReport = CreateSubReport(xrSubReport);
                xrSubReport.ReportSourceUrl = null;
                xrSubReport.ReportSource = subReport;
            }

            report.CreateDocument();
            return report;
        }
        private object GetDataSource()
        {
            if (_renderOption.IsJsonObject != true) return _renderOption.DataSource;
            JsonDataSource jsonDataSource = new();
            string jsonString = JsonSerializer.Serialize(_renderOption.DataSource);
            jsonDataSource.JsonSource = new CustomJsonSource(jsonString);
            jsonDataSource.Fill();
            return jsonDataSource;
        }
        private byte[] RenderOutputFile(XtraReport report)
        {
            using var ms = new MemoryStream();
            string typePrint = _renderOption.TypePrint.ToLower();
            if (typePrint.Equals(ReportTypeConst.Pdf))
            {
                report.ExportToPdf(ms);
            }
            else if (typePrint.Equals(ReportTypeConst.Xlsx))
            {
                report.ExportToXlsx(ms);
            }

            return ms.ToArray();
        }
        #endregion
    }
}
