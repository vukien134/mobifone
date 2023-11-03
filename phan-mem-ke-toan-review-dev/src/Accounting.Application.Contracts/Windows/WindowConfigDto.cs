using Accounting.Catgories.Menus;
using Accounting.Reports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class WindowConfigDto
    {
        public WindowDto Window { get; set; }
        public MenuAccountingDto Menu { get; set; }
        public Dictionary<string, ReferenceDto> References { get; set; }
        public Dictionary<string,string> WindowEvents { get; set; }
        public List<VoucherTemplateDto> VoucherTemplates { get; set; }
        public Dictionary<string,string> CurrencyFormats { get; set; }
        public Dictionary<string,string> SymbolSeparateNumber { get; set; }
        public Dictionary<string,Dictionary<string,string>> TabEvents { get; set; }
        public Dictionary<string, Dictionary<string,string>> FieldEvents { get; set; }
        public Dictionary<string, ReferenceDto> InfoWindowReferences { get; set; }
        public InfoWindowDto InfoWindow { get; set; }
        public List<ButtonDto> Buttons { get; set; }
        public List<ReportMenuShortcutDto> ReportMenuShortcutDtos { get; set; }
    }
}
