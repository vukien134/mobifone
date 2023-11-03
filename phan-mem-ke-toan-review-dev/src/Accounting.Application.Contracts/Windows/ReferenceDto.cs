using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class ReferenceDto : BaseDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string RefType { get; set; }
        public string ValueField { get; set; }
        public string DisplayField { get; set; }
        public string UrlApiData { get; set; }
        public string WindowId { get; set; }
        public string ListValue { get; set; }
        public string ListType { get; set; }
        public bool? FetchData { get; set; }
        public object RefData { get; set; }
        public List<ReferenceDetailDto> ReferenceDetails { get; set; }
    }
}
