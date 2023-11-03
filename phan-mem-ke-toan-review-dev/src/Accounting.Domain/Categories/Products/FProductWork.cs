using Accounting.TenantEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Products
{
    //Công trình, sản phẩm
    public class FProductWork : TenantOrgEntity
    {
        //CT_SP
        public string FProductOrWork { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameTemp { get; set; }
        public string FPWType { get; set; }
        public int? Rank { get; set; }
        public string ParentCode { get; set; }
        public DateTime? BeginningDate { get; set; }
        public DateTime? EndingDate { get; set; }
        //CHU_TC
        public string WorkOwner { get; set; }
        public string Note { get; set; }
        public string ParentId { get; set; }
    }
}
