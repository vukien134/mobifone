using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class AccountSystemService : BaseDomainService<AccountSystem, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public AccountSystemService(IRepository<AccountSystem, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<AccountSystem> GetParentAccountByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return await this.GetAsync(id);
        }
        public async Task<AccountSystem> GetAccountByAccCodeAsync(string accCode, string ordCode, 
                                                                int Year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AccCode == accCode 
                                    && p.OrgCode == ordCode && p.Year == Year);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<AccountSystem> GetAccountByAccCodeAsync(List<AccountSystem> lstAccountSystem, string accCode, string ordCode,
                                                                int Year)
        {
            var account = lstAccountSystem.Where(p => p.AccCode == accCode && p.OrgCode== ordCode && p.Year== Year).FirstOrDefault();
            return account;
        }
        public async Task<List<AccountSystem>> GetAccounts(string orgCode, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode) && p.Year == year);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<AccountSystem>> GetAccounts(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode));
            return await AsyncExecuter.ToListAsync(queryable);
        }        
        public async Task<List<AccountSystem>> GetAccountByRank(int rank, string orgCode, int year)
        {
            var lstAcc = new List<AccountSystem>();
            var lstAccChild = new List<AccountSystem>();
            var iQAccSystem = await this.GetQueryableAsync();
            var lstAccSystem = iQAccSystem.Where(p => p.OrgCode == orgCode && p.Year == year 
                                    && (p.ParentAccId ?? "") == "").ToList();
            if (lstAccSystem == null || lstAccSystem.Count == 0) return lstAcc;
            lstAccChild.AddRange(lstAccSystem);
            foreach (var item in lstAccChild)
            {
                item.AccRank = 1;
            }
            lstAcc.AddRange(lstAccSystem);
            for (var i = 2; i <= rank; i++)
            {
                lstAccChild = iQAccSystem.Where(p => lstAccChild.Select(p => p.Id)
                                        .Contains(p.ParentAccId)).ToList();
                foreach (var item in lstAccChild)
                {
                    item.AccRank = i;
                }
                lstAcc.AddRange(lstAccChild);
            }
            return lstAcc;
        }

        public async Task<List<AccountSystem>> GetAccountAllRank(string orgCode, int year)
        {
            var lstAcc = new List<AccountSystem>();
            var lstAccChild = new List<AccountSystem>();
            var iQAccSystem = await this.GetQueryableAsync();
            var lstAccSystem = iQAccSystem.Where(p => p.OrgCode == orgCode && p.Year == year 
                                    && (p.ParentAccId ?? "") == "").ToList();
            if (lstAccSystem == null || lstAccSystem.Count == 0) return lstAcc;
            lstAccChild.AddRange(lstAccSystem);
            foreach (var item in lstAccChild)
            {
                item.AccRank = 1;
            }
            lstAcc.AddRange(lstAccSystem);
            int i = 2;
            while (iQAccSystem.Any(p => lstAccChild.Select(p => p.Id).Contains(p.ParentAccId)))
            {
                lstAccChild = iQAccSystem.Where(p => lstAccChild.Select(p => p.Id)
                                            .Contains(p.ParentAccId)).ToList();
                foreach (var item in lstAccChild)
                {
                    item.AccRank = i;
                }
                lstAcc.AddRange(lstAccChild);
                i++;
            }
            return lstAcc;
        }

        public async Task<bool> IsExistAccCodeAsync(AccountSystem entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.AccCode == entity.AccCode
                                && p.Year == entity.Year
                                && p.Id != entity.Id
                               );
        }      
        public override async Task CheckDuplicate(AccountSystem entity)
        {
            bool isExistCode = await IsExistAccCodeAsync(entity);
            if (isExistCode)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccountSystem, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.AccCode]);
            }
        }
        public async Task<List<AccountSystem>> GetListAsync(string orgCode,int year)
        {
            var accountingSystem = await this.GetRepository()
                                .GetListAsync(p => p.OrgCode == orgCode
                            && p.Year == year);
            return accountingSystem;
        }
        public async Task<List<AccountSystem>> GetListAttachAsync(string attachPartner, string attachSection, string attachWorkPlace)
        {
            var accountingSystem = await this.GetRepository()
                                .GetListAsync(p => p.AttachPartner == attachPartner|| p.AttachAccSection == attachSection || p.AttachWorkPlace==attachWorkPlace);
            return accountingSystem;
        }
        public async Task<bool> IsExistAsync(string orgCode, int year)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode && p.Year == year);
        }
        public async Task<bool> IsParentGroup(string id)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.ParentAccId == id);
        }
    }
}
