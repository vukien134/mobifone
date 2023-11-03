using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines
{
    public class AssetToolGroupCustomineDto
    {
        public string AssetOrTool { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string Id { get; set; }
        public int Rank { get; set; }
        public string OrdGroup { get; set; }
    }
}
