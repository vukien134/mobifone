using Accounting.Categories.Partners;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class PersonService : BaseDomainService<Person, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public PersonService(IRepository<Person, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(Person entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Name == entity.Name
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(Person entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Person, ErrorCode.Duplicate),
                        _localizer["Err:NameAlreadyExist", entity.Name]);
            }
        }
        public IQueryable<Person> GetQueryableQuickSearch(IQueryable<Person> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.OrgCode, filterValue) || EF.Functions.ILike(p.Name, filterValue));
            return queryable;
        }

        public async Task<List<Person>> GetDataReference(string orgCode, string filterValue)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Address, filterValue) || EF.Functions.ILike(p.Name, filterValue)))
                                .OrderBy(p => p.Name);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
    }
}
