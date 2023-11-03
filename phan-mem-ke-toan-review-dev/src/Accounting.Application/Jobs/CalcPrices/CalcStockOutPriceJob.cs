using Accounting.Constants;
using Accounting.DomainServices.Generals;
using Accounting.DomainServices.Users;
using Accounting.Generals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Jobs.CalcPrices
{
    public class CalcStockOutPriceJob
        : AsyncBackgroundJob<CalcStockOutPriceArg>, ITransientDependency
    {
        #region Fields
        private readonly InfoCalcPriceStockOutService _infoCalcPriceStockService;
        private readonly ICurrentTenant _currentTenant;
        private readonly PricingOutwardAppService _pricingOutwardAppService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserService _userService;
        #endregion
        #region Ctor
        public CalcStockOutPriceJob(InfoCalcPriceStockOutService infoCalcPriceStockService,
                            ICurrentTenant currentTenant,
                            PricingOutwardAppService pricingOutwardAppService,
                            IUnitOfWorkManager unitOfWorkManager,
                            UserService userService)
        {
            _infoCalcPriceStockService = infoCalcPriceStockService;
            _currentTenant = currentTenant;
            _pricingOutwardAppService = pricingOutwardAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _userService = userService;
        }
        #endregion
        public override async Task ExecuteAsync(CalcStockOutPriceArg args)
        {


            using (var uow = _unitOfWorkManager.Begin(isTransactional: false))
            {
                using (_currentTenant.Change(args.TenantId))
                {
                    var infoCalc = await _infoCalcPriceStockService.GetAsync(args.InfoCalcStockOutId);
                    infoCalc.BeginDate = DateTime.Now;
                    infoCalc.CreatorName = await _userService.GetCurrentUserNameAsync();
                    await _infoCalcPriceStockService.UpdateAsync(infoCalc, true);
                    await _pricingOutwardAppService.CreatePricingOutwardAsync(infoCalc.Id);
                    infoCalc.EndDate = DateTime.Now;
                    infoCalc.Status = ExcutionStatus.Ended;
                    await _infoCalcPriceStockService.UpdateAsync(infoCalc, true);
                }
            }
        }
    }
}
