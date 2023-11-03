using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Windows
{
    public class TabDto : BaseDto
    {
        public int Ord { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TabTable { get; set; }
        public string TabType { get; set; }
        public string TabView { get; set; }
        public string UrlApiCrud { get; set; }
        public string UrlApiData { get; set; }
        public string UrlApiDetail { get; set; }
        public string UrlApiTabDetail { get; set; }
        public string OrderBy { get; set; }
        public string WindowId { get; set; }        
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }
        public bool? HasQuickSearch { get; set; }
        public string CellEditStop { get; set; }
        public List<FieldDto> Fields { get; set; }
    }
}
