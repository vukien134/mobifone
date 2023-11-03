using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class WarehouseService : BaseDomainService<Warehouse, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public WarehouseService(IRepository<Warehouse, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ): base(repository)
        {
            _localizer = localizer;
        }

        public async Task<bool> IsExistCode(Warehouse entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(Warehouse entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Warehouse, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<List<Warehouse>> GetByWarehouseAllAsync(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ParentId == id);

            return await AsyncExecuter.ToListAsync(queryable);
        }

        public async Task<List<Warehouse>> GetAllWareHouse(string parentId, List<Warehouse> warehouses, List<Warehouse> Listwarehouses)
        {

            var queryable = await this.GetQueryableAsync();
            List<Warehouse> warehouseList = new List<Warehouse>();

            if (warehouses.Count == 0)
            {
                return Listwarehouses;
            }
            var listWareHouse = queryable.Where(p => p.ParentId == parentId)
                                .OrderBy(p => p.Code).ToList();
            if (listWareHouse.Count == 0) return Listwarehouses;
            foreach (var item in listWareHouse)
            {

                await GetAllWareHouse(item.Id, warehouseList, Listwarehouses);
                warehouseList.Add(item);
                Listwarehouses.Add(item);
            }            

            return await AsyncExecuter.ToListAsync(Listwarehouses.AsQueryable());
        }
        public async Task<List<Warehouse>> GetByWarehouseAsync(string ordCode, string wareHouse)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == ordCode && p.Code == wareHouse);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<Warehouse>> GetDataReference(string orgCode, string filterValue)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue)))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
        public IQueryable<Warehouse> GetQueryableQuickSearch(IQueryable<Warehouse> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue));
            return queryable;
        }
        public async Task<bool> IsParentGroup(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.ParentId == id);
        }
        public async Task<List<Warehouse>> GetListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
