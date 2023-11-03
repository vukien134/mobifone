using Accounting.BaseDtos;

namespace Accounting.Catgories.Others.Departments
{
    public class DepartmentDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameTemp { get; set; }
        public string ParentId { get; set; }
        public string DepartmentType { get; set; }
        public string ParentCode { get; set; }
    }
}
