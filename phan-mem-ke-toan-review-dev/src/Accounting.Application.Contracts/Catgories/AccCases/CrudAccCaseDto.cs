using Accounting.BaseDtos;
using Accounting.Localization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.AccCases
{
    public class CrudAccCaseDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
        public string CaseType { get; set; }
    }
}
