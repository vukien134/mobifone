using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Accounting.Catgories.Accounts.AccOpeningBalances
{
    public class AccOpeningBalanceParaDto
    {
        public List<CrudAccOpeningBalanceDto> Data { get; set; }
    }
}
