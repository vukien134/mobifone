using Accounting.Catgories.AssetTools;
using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.BaseDtos.Customines.AssetTool
{
    public class ObjectListAssetToolAccountDto
    {
        public string AssetToolId { get; set; }
        public List<CrudAssetToolAccountDto> Data { get; set; }
    }
}
