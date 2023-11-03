using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Catgories.AssetTools
{
    public class DefaultAssetToolGroupDto : BaseDto
    {
        public string AssetOrTool { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
    }
}
