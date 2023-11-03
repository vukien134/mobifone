using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.CostProductions
{
    public interface ICostProductionAppService
    {
        Task<ResultDto> ProductionCostCalculationAsync(ProductionCostCalculationFilterDto filterDto);
        Task<ResultDto> ProjectCostCalculationAsync(ProjectCostCalculationFilterDto filterDto);
        Task<ResultDto> ForwardAutoAsync(ForwardAutoFilterDto filterDto);
    }
}
