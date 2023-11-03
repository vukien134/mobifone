using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Reports
{
    public class ReportMenuShortcutDto : BaseDto
    {
        public string Caption { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconColor { get; set; }
        public string VisibleWhen { get; set; }
        public string Parameter { get; set; }
        public string OriginReportId { get; set; }
        public string ReferenceReportId { get; set; }
        public string ReferenceWindowId { get; set; }
    }
}
