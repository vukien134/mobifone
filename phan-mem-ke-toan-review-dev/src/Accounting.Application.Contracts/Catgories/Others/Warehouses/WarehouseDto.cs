using Accounting.BaseDtos;
using System;

namespace Accounting.Catgories.Others.Warehouses
{
    public class WarehouseDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameTemp { get; set; }
        public string WarehouseType { get; set; }
        public int WarehouseRank { get; set; }
        public string ParentId { get; set; }
        public string Address { get; set; }
        public string WarehouseAcc { get; set; }
    }
}
