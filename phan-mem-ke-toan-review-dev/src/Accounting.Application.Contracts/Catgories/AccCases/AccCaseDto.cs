using Accounting.BaseDtos;

namespace Accounting.Catgories.AccCases
{
    public class AccCaseDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CaseType { get; set; }
    }
}
