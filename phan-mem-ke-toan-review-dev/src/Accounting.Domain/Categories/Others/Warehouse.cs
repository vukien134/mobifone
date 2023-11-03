using Accounting.TenantEntities;

namespace Accounting.Categories.Others
{
    public class Warehouse : TenantOrgEntity
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
