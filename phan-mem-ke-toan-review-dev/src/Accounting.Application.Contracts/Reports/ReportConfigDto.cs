using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class ReportConfigDto
    {
        public ReportTemplateDto ReportTemplate { get; set; }
        public List<ReportTemplateColumnDto> ReportTemplateColumns { get; set; }
        public InfoWindowDto InfoWindow { get; set; }
        public Dictionary<string, string> CurrencyFormats { get; set; }
        public Dictionary<string, string> SymbolSeparateNumber { get; set; }
        public Dictionary<string, ReferenceDto> References { get; set; }
        public List<ReportMenuShortcutDto> ReportMenuShortcuts { get; set; }
        public List<ButtonDto> Buttons { get; set; }
    }
}
