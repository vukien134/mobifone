using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class ButtonDto: BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public int? Width { get; set; }
        public string Caption { get; set; }
        public string OnClick { get; set; }
        public string WindowId { get; set; }
        public string ReportTemplateId { get; set; }
        public string MenuClick { get; set; }
        public string IsGroup { get; set; }
        public string ShortCut { get; set; }
    }
}
