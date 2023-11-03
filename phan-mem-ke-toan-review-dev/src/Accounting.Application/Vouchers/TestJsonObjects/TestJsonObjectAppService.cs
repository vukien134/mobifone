using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Others.Careers;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.EntityFrameworkCore;
using Accounting.Helpers;
using Accounting.Reports;
using Accounting.Vouchers.ResetVoucherNumbers;
using Accounting.Vouchers.TransMigrations;
using Accounting.Vouchers.VoucherNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.Vouchers.VoucherNumbers
{
    public class TestJsonObjectAppService : AccountingAppService, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly VoucherNumberService _voucherNumberService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly AccountingDb _accountingDb;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        #endregion
        #region Ctor
        public TestJsonObjectAppService(VoucherNumberService voucherNumberService,
                            AccountingDb accountingDb,
                            UserService userService,
                            VoucherCategoryService voucherCategoryService,
                            ProductVoucherService productVoucherService,
                            AccVoucherService accVoucherService,
                            IUnitOfWorkManager unitOfWorkManager,
                            WebHelper webHelper
                            )
        {
            _voucherNumberService = voucherNumberService;
            _accountingDb = accountingDb;
            _userService = userService;
            _voucherCategoryService = voucherCategoryService;
            _productVoucherService = productVoucherService;
            _accVoucherService = accVoucherService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<ResultDto> TestAsync(TransMigrationDto dto)
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            string sqlTable = $"select table_name from information_schema.Columns WHERE Column_Name='OrgCode'";
            var dataTable = await _accountingDb.GetDataTableAsync(sqlTable);
            var dataRow = dataTable.Rows;
            for (int i = 0; i < dataRow.Count; i++)
            {
                var dict = new Dictionary<string, object>();
                var table_name = dataRow[i][0];
                string sqlCheckData = $"select 1 from \"{table_name}\" WHERE \"OrgCode\"=@orgCode";
                dict.Add("orgCode", orgCode);
                var dataCheckData = await _accountingDb.GetDataTableAsync(sqlCheckData, dict);
                if (dataCheckData.Rows.Count > 0) throw new Exception($"Đơn vị cơ sở {orgCode} đã có phát sinh");
            }
            var voucherCategory = await _voucherCategoryService.GetQueryableAsync();
            voucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstdata = voucherCategory.ToList();
            var lstDataNew = new List<VoucherCategory>();
            string str = "Name";
            string strCode = "Code";
            var lstJob = new List<JsonObject>();
            foreach (var item in lstJob)
            {
                item[str] = "";
            }
            foreach (var item in lstdata)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = (JsonObject)JsonObject.Parse(json);
                job[str] = "Phiếu xuất kho đã sửa";
                job.Add(str, 1);
                job.Add("sum1", 1);
                job.Add("sum2", 2);
                job.Add("sum3", 3);
                lstJob.Add((JsonObject)job);
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<VoucherCategory>(job.ToString());
                lstDataNew.Add(obj);
            }
            var lstField = GetFormular("sum1 - sum2 + sum3");
            var lstFill = lstJob.Where(p => p["VoucherType"].ToString() == "T").ToList();
            var zxcvgr = lstJob.GroupBy(g => new { VoucherType = g["VoucherType"].ToString() }).Select(p =>
                                        {
                                            decimal amount = 0;
                                            foreach (var itemField in lstField)
                                            {
                                                amount += p.Sum(a => decimal.Parse(a[itemField.Code].ToString())) * ((itemField.Math == "+") ? 1 : -1);
                                            }
                                            return new
                                            {
                                                VoucherType = p.Key.VoucherType,
                                                sum = p.Sum(a => decimal.Parse(a["sum"].ToString())),
                                                sum2 = amount
                                            };
                                        }).ToList();

            var isBookClosing = voucherCategory.Where(p => p.BookClosingDate.Value.Year >= _webHelper.GetCurrentYear())
                                               .OrderByDescending(p => p.BookClosingDate);
            var tabName = "VoucherCategory";
            var data = (from a in lstJob
                        where a[strCode].ToString() == "PXT"
                        select a).ToList();
            var strJson = "{\"data\":[{\"FieldName\":\"TaxCategoryCode\",\"Type\":\"Contains\",\"Value\":\"[V00D],[V00C],[V00K]\"},{\"FieldName\":\"CreditAcc\",\"Type\":\"!=\",\"Value\":\"33312\"},{\"FieldName\":\"TaxCategoryCode\",\"Type\":\"!Contains\",\"Value\":\"[V00D],[V00C],[V00K]\"}]}";
            var jsonConvert = (JsonObject)JsonObject.Parse(strJson);
            var lstJson = (JsonArray)jsonConvert["data"];
            foreach (var item in lstJson)
            {
                switch (item["Type"].ToString())
                {
                    case "Contains":
                        break;
                    case "!Contains":
                        break;
                    case "!=":
                        break;
                    case "==":
                        break;
                    case ">":
                        break;
                    case "<":
                        break;
                    default:
                        break;
                }
            }
            var accVoucher = await _accVoucherService.GetQueryableAsync();
            accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucer = await _productVoucherService.GetQueryableAsync();
            productVoucer = productVoucer.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstKK = "KK0";

            var res = new ResultDto();
            res.Ok = true;
            return res;
        }

        #region Private
        private List<FormularDto> GetFormular(string formular)
        {
            var lst = new List<FormularDto>();
            formular = formular.Replace(" ", "");
            formular = formular.Replace("+", ",+,");
            formular = formular.Replace("-", ",-,");
            formular = "+," + formular;
            var lstData = formular.Split(',').ToList();
            for (var i = 0; i < lstData.Count; i += 2)
            {
                lst.Add(new FormularDto
                {
                    Code = lstData[i + 1],
                    AccCode = lstData[i + 1],
                    Math = lstData[i],
                });
            }
            return lst;
        }
        #endregion
    }
}
