using Accounting.BaseDtos;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Catgories.Others.Warehouses
{
    public class CrudWarehouseDto : CruOrgBaseDto
    {
        [Required]
        [MinLength(3)]
        [DisplayName("code")]
        public string Code { get; set; }

        [Required]
        [DisplayName("name")]
        public string Name { get; set; }
        public string NameTemp { get; set; }
        public string WarehouseType { get; set; }
        public int WarehouseRank { get; set; }
        public string ParentId { get; set; }
        public string Address { get; set; }
        public string WarehouseAcc { get; set; }
    }
}
