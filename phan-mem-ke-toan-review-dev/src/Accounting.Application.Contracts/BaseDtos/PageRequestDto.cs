using System.Collections.Generic;

namespace Accounting.BaseDtos
{
    public class PageRequestDto 
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public bool? Continue { get; set; }
        public string QuickSearch { get; set; }
        public string WindowId { get; set; }        
        public List<FilterRowItemDto> FilterRows { get; set; }
        public List<FilterAdvancedItemDto> FilterAdvanced { get; set; }
    }
}
