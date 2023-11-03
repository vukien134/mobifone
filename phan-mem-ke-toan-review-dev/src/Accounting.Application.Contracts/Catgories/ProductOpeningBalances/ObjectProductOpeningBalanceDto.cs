using System.Collections.Generic;

namespace Accounting.Catgories.ProductOpeningBalances
{
    public class ObjectProductOpeningBalanceDto
    {
        public List<CrudProductOpeningBalanceDto> Data { get; set; }
        public string WarehouseCode { get; set; }
    }
}
