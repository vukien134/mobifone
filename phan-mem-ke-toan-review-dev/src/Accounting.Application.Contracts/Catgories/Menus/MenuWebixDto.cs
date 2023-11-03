using System.Collections.Generic;

namespace Accounting.Catgories.Menus
{
    public class MenuWebixDto
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string JavaScriptCode { get; set; }
        public string WindowId { get; set; }
        public List<MenuWebixDto> Submenu { get; set; }
    }
}
