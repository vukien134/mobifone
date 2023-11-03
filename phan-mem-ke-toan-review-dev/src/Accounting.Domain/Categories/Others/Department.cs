using Accounting.TenantEntities;

namespace Accounting.Categories.Others
{
    public class Department : TenantOrgEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameTemp { get; set; }
        public string ParentId { get; set; }
        public string DepartmentType { get; set; }
        public string ParentCode { get; set; }
    }
}
