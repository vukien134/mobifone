using Accounting.BaseDtos;
using Accounting.Catgories.Others.CostOfGoods;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.CalculatePriceFifoes
{
    public interface ICalculatePriceFifoAppService
    {
        Task<ResultDto> CalculatePriceFifo(CostOfGoodsDto dto);
    }
}
