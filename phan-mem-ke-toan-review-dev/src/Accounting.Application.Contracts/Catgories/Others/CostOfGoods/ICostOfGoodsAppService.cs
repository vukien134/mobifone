using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.CostOfGoods
{
    public interface ICostOfGoodsAppService
    {
        Task<List<CostOfGoodsDto>> CostOfGoodsAsync(CostOfGoodsDto costOfGoodsAppService);
    }
}
