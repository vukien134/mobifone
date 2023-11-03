using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.Menus
{
    public class MenuAccountingDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string Url { get; set; }
        public string Detail { get; set; }
        public string Icon { get; set; }
        public string Caption { get; set; }
        public string windowId { get; set; }
        public int? Order { get; set; }
        public string JavaScriptCode { get; set; }
    }
}
