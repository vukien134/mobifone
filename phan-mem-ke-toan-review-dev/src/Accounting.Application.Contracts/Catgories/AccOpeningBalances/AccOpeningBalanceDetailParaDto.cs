using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Accounting.Catgories.Accounts.AccOpeningBalances
{
    public class AccOpeningBalanceDetailParaDto
    {
        public string AccCode { get; set; }
        public List<CrudAccOpeningBalanceDto> Data { get; set; }
    }
}
