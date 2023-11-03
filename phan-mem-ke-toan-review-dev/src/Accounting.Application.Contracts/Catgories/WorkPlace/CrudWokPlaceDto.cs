using Accounting.BaseDtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.WorkPlace
{
    public class CrudWokPlaceDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
    }
}
