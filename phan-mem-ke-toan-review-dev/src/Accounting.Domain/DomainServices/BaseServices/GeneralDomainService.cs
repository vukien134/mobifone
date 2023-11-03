using Accounting.Categories.AssetTools;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Accounting.DomainServices.BaseServices
{
    public class GeneralDomainService : DomainService
    {
        #region Fields
        private readonly IServiceProvider _serviceProvider;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public GeneralDomainService(IServiceProvider serviceProvider,
                                    IStringLocalizer<AccountingResource> localizer
            )
        {
            _serviceProvider = serviceProvider;
            _localizer = localizer;
        }
        #endregion
        #region Methods
        public async Task<Dictionary<Type,bool>> IsExistCode(Dictionary<Type,object[]> dict)
        {
            Dictionary<Type, bool> result = new();
            foreach(var entry in dict)
            {
                var type = entry.Key;
                var service = _serviceProvider.GetService(type);
                var methods = type.GetMethods();
                foreach(var item in methods)
                {
                    if (item.Name.Contains("IsExistCode"))
                    {
                        int countParament = item.GetParameters().Length;
                        if (countParament != 2) continue;
                        var isExist = await item.InvokeAsync(service, entry.Value);
                        result.Add(type,(bool) isExist);
                    }
                }
            }
            
            return result;
        }
        public AccountingException GetExistException(string name, string code)
        {
            var exception = name switch
            {
                nameof(AccPartnerService) => new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.NotFoundEntity),
                        _localizer["Err:PartnerCodeNotExist"]),
                nameof(WorkPlaceSevice) => new AccountingException(ErrorCode.Get(GroupErrorCodes.WorkPlace, ErrorCode.NotFoundEntity),
                        _localizer["Err:WorkPlaceCodeNotExist"]),
                nameof(FProductWorkService) => new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWork, ErrorCode.NotFoundEntity),
                        _localizer["Err:FProductCodeNotExist"]),
                nameof(AccSectionService) => new AccountingException(ErrorCode.Get(GroupErrorCodes.AccSection, ErrorCode.NotFoundEntity),
                        _localizer["Err:SectionCodeNotExist"]),
                nameof(AccCaseService) => new AccountingException(ErrorCode.Get(GroupErrorCodes.AccCase, ErrorCode.NotFoundEntity),
                        _localizer["Err:CaseCodeNotExist"]),            
                _ => null,               
            };
            return exception;
        }
        public async Task<IQueryable<TEntity>> GetQueryableAsync<TEntity, TKey>() where TEntity : class, IEntity<TKey>
        {
            var repository = (IRepository<TEntity, TKey>)_serviceProvider.GetService(typeof(IRepository<TEntity, TKey>));
            var queryable = await repository.GetQueryableAsync();
            return await repository.GetQueryableAsync();
        }
        public async Task<IQueryable<TResult>> GetQueryableAsync<TEntity, TKey, TResult>(Expression<Func<TEntity,TResult>> select) where TEntity : class, IEntity<TKey>
        {
            var repository = (IRepository<TEntity, TKey>)_serviceProvider.GetService(typeof(IRepository<TEntity, TKey>));
            var queryable = await repository.GetQueryableAsync();
            return queryable.Select(select);
        }
        public async Task<List<TEntity>> GetListAsync<TEntity,TKey>() where TEntity : class, IEntity<TKey>
        {
            var repository = (IRepository<TEntity, TKey>)_serviceProvider.GetService(typeof(IRepository<TEntity, TKey>));
            return await repository.ToListAsync();
        }
        public async Task<List<TResult>> GetListAsync<TEntity, TKey, TResult>(Expression<Func<TEntity,TResult>> selector) where TEntity : class, IEntity<TKey>
        {
            var repository = (IRepository<TEntity, TKey>)_serviceProvider.GetService(typeof(IRepository<TEntity, TKey>));
            var queryable = await repository.GetQueryableAsync();
            var queryableSelect = queryable.Select<TEntity, TResult>(selector);
            return await AsyncExecuter.ToListAsync(queryableSelect);
        }
        public async Task<List<TResult>> GetListAsync<TEntity, TKey, TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> selector) where TEntity : class, IEntity<TKey>
        {
            var repository = (IRepository<TEntity, TKey>)_serviceProvider.GetService(typeof(IRepository<TEntity, TKey>));
            var queryable = await repository.GetQueryableAsync();
            queryable = queryable.Where(predicate);
            var queryableSelect = queryable.Select<TEntity, TResult>(selector);
            return await AsyncExecuter.ToListAsync(queryableSelect);
        }
        #endregion
    }
}
