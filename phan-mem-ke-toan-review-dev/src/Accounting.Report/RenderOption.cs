using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Report
{
    public class RenderOption
    {
        public object DataSource { get; set; }
        public string TypePrint { get; set; }
        public string TemplateFile { get; set; }
        public Dictionary<string,string> CurrencyFormats { get; set; }
        public Dictionary<string,string> SymbolSeparateNumber { get; set; }
        public bool? IsJsonObject { get; set; }
    }
}
