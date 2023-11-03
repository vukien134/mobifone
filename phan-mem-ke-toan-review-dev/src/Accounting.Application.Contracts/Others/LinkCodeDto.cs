using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Others
{
    public class LinkCodeDto : BaseDto
    {
        public int? Ord { get; set; }
        public string FieldCode { get; set; }
        public string RefTableName { get; set; }
        public string RefFieldCode { get; set; }
        public bool? IsChkDel { get; set; }
        public bool? AttachYear { get; set; }
    }
}
