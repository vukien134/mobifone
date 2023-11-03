using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class WindowDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string WindowType { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? MaxRowEditInForm { get; set; }
        public int? OrdRowTab { get; set; }
        public string VoucherCode { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }
        public string FormLayout { get; set; }
        public string UrlApiExportExcel { get; set; }
        public string UrlApiImportExcel { get; set; }
        public string InfoWindowId { get; set; }
        public List<TabDto> Tabs { get; set; }
    }
}
