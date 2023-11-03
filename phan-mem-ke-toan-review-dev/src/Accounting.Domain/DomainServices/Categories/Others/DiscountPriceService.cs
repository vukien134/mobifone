using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class DiscountPriceService : BaseDomainService<DiscountPrice, string>
    {
        #region Fields
        private readonly DiscountPricePartnerService _discountPricePartnerService;
        #endregion
        public DiscountPriceService(IRepository<DiscountPrice, string> repository,
                DiscountPricePartnerService discountPricePartnerService 
            ) : base(repository)
        {
            _discountPricePartnerService = discountPricePartnerService;
        }
        public async Task<bool> IsExistCode(DiscountPrice entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(DiscountPrice entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.DiscountPrice, ErrorCode.Duplicate),
                        $"DiscountPrice Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<DiscountPriceDetail> GetSalePriceAsync(string orgCode,DateTime voucherDate,
                                            string productCode,string partnerCode)
        {
            var queryable = await this.GetRepository().WithDetailsAsync(p => p.DiscountPriceDetails);                                                      
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode)
                                    && p.BeginDate <= voucherDate && p.EndDate >= voucherDate
                                    && p.DiscountPriceDetails.Any(d => d.OrgCode.Equals(orgCode) && d.ProductCode.Equals(productCode)))
                                .OrderByDescending(p => p.EndDate).ThenByDescending(p => p.CreationTime);
            var discountPrices = await AsyncExecuter.ToListAsync(queryable);
            if (discountPrices.Count == 0) return null;

            var priceIds = discountPrices.Select(p => p.Id).ToArray();
            var queryablePartner = await _discountPricePartnerService.GetQueryableAsync();
            queryablePartner = queryablePartner.Where(p => priceIds.Contains(p.DiscountPriceId)
                                    && p.PartnerCode.Equals(partnerCode));
            var discountPricePartners = await AsyncExecuter.ToListAsync(queryablePartner);
            var prices = discountPrices[0];
            foreach(var item in discountPrices)
            {
                if (discountPricePartners.Any(p => p.DiscountPriceId.Equals(item.Id)))
                {
                    prices = item;
                    break;
                }
            }
            return prices.DiscountPriceDetails.Where(p => p.ProductCode.Equals(productCode))
                                .FirstOrDefault();
        }
    }
}
