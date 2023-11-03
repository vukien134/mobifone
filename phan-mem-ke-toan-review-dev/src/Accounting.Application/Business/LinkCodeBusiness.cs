
using Accounting.Caching;
using Accounting.DomainServices.Windows;
using Accounting.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Accounting.Business
{
    public class LinkCodeBusiness : ITransientDependency
    {
        #region Fields
        private readonly LinkCodeService _linkCodeService;
        private readonly AccountingDb _accountingDb;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public LinkCodeBusiness(LinkCodeService linkCodeService,
                AccountingDb accountingDb,
                AccountingCacheManager accountingCacheManager
            )
        {
            _linkCodeService = linkCodeService;
            _accountingDb = accountingDb;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        public async Task<bool> IsCodeUsing(string fieldCode,string value,string orgCode)
        {
            var linkCodes = await _accountingCacheManager.GetLinkCodeAsync(fieldCode);
            if (linkCodes.Count == 0) return false;

            var dict = new Dictionary<string, object>();
            foreach(var item in linkCodes)
            {
                if (dict.Count > 0) dict.Clear();

                string sql = $"select \"{item.RefFieldCode}\" from \"{item.RefTableName}\" "
                        + $"where \"{item.RefFieldCode}\"=@fieldCode ";
                dict.Add("fieldCode", value);
                if (!string.IsNullOrEmpty(orgCode))
                {
                    sql = sql + " and \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCode);
                }

                var dataTable = await _accountingDb.GetDataTableAsync(sql, dict);
                if (dataTable.Rows.Count > 0) return true;
            }
            return false;
        }

        public async Task<string> IsCodeUsingString(string fieldCode, string value, string orgCode)
        {
            var linkCodes = await _accountingCacheManager.GetLinkCodeAsync(fieldCode);
            if (linkCodes.Count == 0) return "";

            var dict = new Dictionary<string, object>();
            foreach (var item in linkCodes)
            {
                if (dict.Count > 0) dict.Clear();

                string sql = $"select \"{item.RefFieldCode}\" from \"{item.RefTableName}\" "
                        + $"where \"{item.RefFieldCode}\"=@fieldCode ";
                dict.Add("fieldCode", value);
                if (!string.IsNullOrEmpty(orgCode))
                {
                    sql = sql + " and \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCode);
                }

                var dataTable = await _accountingDb.GetDataTableAsync(sql, dict);
                if (dataTable.Rows.Count > 0) 
                return $"Dữ liệu {item.RefFieldCode} = {value} ở bảng {item.RefTableName} đã có phát sinh";
            }
            return "";
        }

        public async Task<string> IsCodeUsingString(string fieldCode, string value, string orgCode, int? year)
        {
            var linkCodes = await _accountingCacheManager.GetLinkCodeAsync(fieldCode);
            if (linkCodes.Count == 0) return "";

            var dict = new Dictionary<string, object>();
            foreach (var item in linkCodes)
            {
                if (dict.Count > 0) dict.Clear();

                string sql = $"select \"{item.RefFieldCode}\" from \"{item.RefTableName}\" "
                        + $"where \"{item.RefFieldCode}\"=@fieldCode ";
                dict.Add("fieldCode", value);
                if (!string.IsNullOrEmpty(orgCode))
                {
                    sql = sql + " and \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCode);
                }
                if (year != null && item.AttachYear == true)
                {
                    sql = sql + " and \"Year\"=@year";
                    dict.Add("year", year);
                }

                var dataTable = await _accountingDb.GetDataTableAsync(sql, dict);
                if (dataTable.Rows.Count > 0)
                    return $"Dữ liệu {item.RefFieldCode} = {value} ở bảng {item.RefTableName} đã có phát sinh";
            }
            return "";
        }

        public async Task<bool> IsCodeUsing(string fieldCode, string value, string orgCode,
                                int? year)
        {
            var linkCodes = await _accountingCacheManager.GetLinkCodeAsync(fieldCode);
            if (linkCodes.Count == 0) return false;

            var dict = new Dictionary<string, object>();
            foreach (var item in linkCodes)
            {
                if (dict.Count > 0) dict.Clear();

                string sql = $"select \"{item.RefFieldCode}\" from \"{item.RefTableName}\" "
                        + $"where \"{item.RefFieldCode}\"=@fieldCode ";
                dict.Add("fieldCode", value);
                if (!string.IsNullOrEmpty(orgCode))
                {
                    sql = sql + " and \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCode);
                }
                if (year != null && item.AttachYear == true)
                {
                    sql = sql + " and \"Year\"=@year";
                    dict.Add("year", year);
                }
                var dataTable = await _accountingDb.GetDataTableAsync(sql, dict);
                if (dataTable.Rows.Count > 0) return true;
            }
            return false;
        }
        public async Task UpdateCode(string fieldCode, string newValue, string oldValue, 
                                            string orgCode)
        {
            var linkCodes = await _accountingCacheManager.GetLinkCodeAsync(fieldCode);
            if (linkCodes.Count == 0) return;
            var dict = new Dictionary<string, object>();
            foreach (var item in linkCodes)
            {
                if (dict.Count > 0) dict.Clear();

                string sql = $"update \"{item.RefTableName}\" " +
                            $"set \"{item.RefFieldCode}\"=@newValue "
                        + $"where \"{item.RefFieldCode}\"=@oldValue ";
                dict.Add("newValue", newValue);
                dict.Add("oldValue", oldValue);
                if (!string.IsNullOrEmpty(orgCode))
                {
                    sql = sql + " and \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCode);
                }

                await _accountingDb.ExecuteSQLAsync(sql, dict);
            }
        }
        public async Task UpdateCode(string fieldCode, string newValue, string oldValue,
                                            string orgCode,int? year)
        {
            var linkCodes = await _accountingCacheManager.GetLinkCodeAsync(fieldCode);
            if (linkCodes.Count == 0) return;
            var dict = new Dictionary<string, object>();
            foreach (var item in linkCodes)
            {
                if (dict.Count > 0) dict.Clear();

                string sql = $"update \"{item.RefTableName}\" " +
                            $"set \"{item.RefFieldCode}\"=@newValue "
                        + $"where \"{item.RefFieldCode}\"=@oldValue ";
                dict.Add("newValue", newValue);
                dict.Add("oldValue", oldValue);
                if (!string.IsNullOrEmpty(orgCode))
                {
                    sql = sql + " and \"OrgCode\"=@orgCode";
                    dict.Add("orgCode", orgCode);
                }
                if (year != null && item.AttachYear == true)
                {
                    sql = sql + " and \"Year\"=@year";
                    dict.Add("year", year);
                }
                await _accountingDb.ExecuteSQLAsync(sql, dict);
            }
        }
        #endregion
    }
}
