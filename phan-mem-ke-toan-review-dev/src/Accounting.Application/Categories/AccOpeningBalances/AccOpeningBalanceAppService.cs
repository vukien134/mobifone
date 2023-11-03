using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Accounts.AccOpeningBalances;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.AccOpeningBalances
{
    public class AccOpeningBalanceAppService : AccountingAppService, IAccOpeningBalanceAppService
    {
        #region Fields
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly AccountSystemService _accountSystemService;
        private readonly ExcelService _excelService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        #endregion
        #region Ctor
        public AccOpeningBalanceAppService(AccOpeningBalanceService accOpeningBalanceService,
                            AccountSystemService accountSystemService,
                            ExcelService excelService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper
                            )
        {
            _accOpeningBalanceService = accOpeningBalanceService;
            _accountSystemService = accountSystemService;
            _excelService = excelService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerCreate)]
        public async Task<AccOpeningBalanceDto> CreateAsync(CrudAccOpeningBalanceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            var entity = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(dto);
            var result = await _accOpeningBalanceService.CreateAsync(entity);
            return ObjectMapper.Map<AccOpeningBalance, AccOpeningBalanceDto>(result);
        }
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerCreate)]
        public async Task<List<AccOpeningBalanceDto>> CreateListAsync(AccOpeningBalanceParaDto listDto)
        {
            await _licenseBusiness.CheckExpired();
            var accSystem = (await _accountSystemService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                                    && p.Year == _webHelper.GetCurrentYear());
            List<AccOpeningBalanceDto> listResult = new List<AccOpeningBalanceDto>();
            listDto.Data = (from a in listDto.Data
                           join b in accSystem on a.AccCode equals b.AccCode
                           where b.AttachAccSection != "C" && b.AttachContract != "C" && b.AttachCurrency != "C"
                                 && b.AttachPartner != "C" && b.AttachProductCost != "C" && b.AttachWorkPlace != "C"
                           select a).ToList();
            var listAccCode = from a in listDto.Data
                              group a.AccCode by a.AccCode into g
                              select new { AccCode = g.Key };
            foreach(var item in listAccCode)
            {
                var accOpeningBalances = await _accOpeningBalanceService.GetByAccCodeAsync(item.AccCode);
                if (accOpeningBalances != null)
                {
                    await _accOpeningBalanceService.DeleteManyAsync(accOpeningBalances);
                }
            }
            foreach (var dto in listDto.Data)
            {
                dto.CreatorName = await _userService.GetCurrentUserNameAsync();
                dto.Id = GetNewObjectId();
                dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                dto.Year = _webHelper.GetCurrentYear();
                var entity = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(dto);
                var result = await _accOpeningBalanceService.CreateAsync(entity);
                listResult.Add(ObjectMapper.Map<AccOpeningBalance, AccOpeningBalanceDto>(result));
            }
            return listResult;
        }

        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerCreate)]
        public async Task<List<AccOpeningBalanceDto>> CreateListDetailAsync(AccOpeningBalanceDetailParaDto listDto)
        {
            await _licenseBusiness.CheckExpired();
            List<AccOpeningBalanceDto> listResult = new List<AccOpeningBalanceDto>();
            var accOpeningBalances = await _accOpeningBalanceService.GetByAccCodeAsync(listDto.AccCode);
            if (accOpeningBalances != null)
            {
                await _accOpeningBalanceService.DeleteManyAsync(accOpeningBalances);
            }
            foreach (var dto in listDto.Data)
            {
                dto.CreatorName = await _userService.GetCurrentUserNameAsync();
                dto.Id = GetNewObjectId();
                dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                dto.Year = _webHelper.GetCurrentYear();
                var entity = ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(dto);
                var result = await _accOpeningBalanceService.CreateAsync(entity);
                listResult.Add(ObjectMapper.Map<AccOpeningBalance, AccOpeningBalanceDto>(result));
            }
            return listResult;
        }

        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _accOpeningBalanceService.DeleteAsync(id,true);
        }
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerView)]
        public async Task<PageResultDto<AccOpeningBalanceDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerView)]
        public async Task<PageResultDto<AccOpeningBalanceDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AccOpeningBalanceDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.AccCode).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AccOpeningBalance, AccOpeningBalanceDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<AccOpeningBalanceDto>> GetDetailByAccCodeAsync(string accCode)
        {
            var result = new List<AccOpeningBalanceDto>();
            var query = await _accOpeningBalanceService.GetQueryableAsync();
            query = query.Where(p => p.AccCode == accCode 
                                     && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                     && p.Year == _webHelper.GetCurrentYear());
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<AccOpeningBalance, AccOpeningBalanceDto>(p)).ToList();
            return result;
        }
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerView)]
        public async Task<List<AccOpeningBalanceCustomineDto>> GetDataAsync()
        {
            
            var result = new List<AccOpeningBalanceCustomineDto>();
            var accOpeningBalances = await _accOpeningBalanceService.GetQueryableAsync();
            var accountSystems = await _accountSystemService.GetQueryableAsync();
            accOpeningBalances = accOpeningBalances.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                            && p.Year == _webHelper.GetCurrentYear());
            accountSystems = accountSystems.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                            && p.Year == _webHelper.GetCurrentYear());
            var datas = from aS in accountSystems
                        join aOB in accOpeningBalances on aS.AccCode equals aOB.AccCode into aOBDt
                        from aOB in aOBDt.DefaultIfEmpty()
                        where !(from aS0 in accountSystems
                                group new { aS0 } by new
                                {
                                    aS0.ParentCode
                                } into aS0Gr
                                select aS0Gr.Key.ParentCode)
                                .Contains(aS.AccCode)
                        group new { aS, aOB } by new
                        {
                            aS.AccCode,
                            aS.AccName,
                            aS.Year,
                            aS.OrgCode,
                            aS.AttachCurrency,
                            aS.AttachPartner,
                            aS.AttachContract,
                            aS.AttachProductCost,
                            aS.AttachAccSection,
                            aS.AttachWorkPlace
                        } into aSGr
                        select new AccOpeningBalanceCustomineDto
                        {
                            Year = aSGr.Key.Year,
                            AccCode = aSGr.Key.AccCode,
                            CurrencyCode = "",
                            AccName = aSGr.Key.AccName,
                            OrgCode = aSGr.Key.OrgCode,
                            Debit = aSGr.Sum(d => d.aOB.Debit),
                            DebitCur = aSGr.Sum(d => d.aOB.DebitCur),
                            Credit = aSGr.Sum(d => d.aOB.Credit),
                            CreditCur = aSGr.Sum(d => d.aOB.CreditCur),
                            DebitCum = 0,
                            DebitCumCur = 0,
                            CreditCum = 0,
                            CreditCumCur = 0,
                            AttachCurrency = aSGr.Key.AttachCurrency,
                            AttachPartner = aSGr.Key.AttachPartner,
                            AttachContract = aSGr.Key.AttachContract,
                            AttachProductCost = aSGr.Key.AttachProductCost,
                            AttachAccSection = aSGr.Key.AttachAccSection,
                            AttachWorkPlace = aSGr.Key.AttachWorkPlace
                        };
            var sections = await AsyncExecuter.ToListAsync(datas);
            result = sections.Select(p => p).ToList();
            return result;
        }
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerUpdate)]
        public async Task UpdateAsync(string id, CrudAccOpeningBalanceDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            var entity = await _accOpeningBalanceService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _accOpeningBalanceService.UpdateAsync(entity);
        }
        [Authorize(AccountingPermissions.AccountOpeningBalanceManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudAccOpeningBalanceDto>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {
                var accOpeningBalances = await _accOpeningBalanceService.GetByAccCodeAsync(item.AccCode);
                if (accOpeningBalances != null)
                {
                    await _accOpeningBalanceService.DeleteManyAsync(accOpeningBalances);
                }
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
                item.Year = _webHelper.GetCurrentYear();
            }

            var lstAccOpeningBalance = lstImport.Select(p => ObjectMapper.Map<CrudAccOpeningBalanceDto, AccOpeningBalance>(p)).ToList();
            await _accOpeningBalanceService.CreateManyAsync(lstAccOpeningBalance);
            return new UploadFileResponseDto() { Ok = true };
        }

        public async Task<AccOpeningBalanceDto> GetByIdAsync(string caseId)
        {
            var accOpeningBalance = await _accOpeningBalanceService.GetAsync(caseId);
            return ObjectMapper.Map<AccOpeningBalance, AccOpeningBalanceDto>(accOpeningBalance);
        }
        #region Private
        private async Task<IQueryable<AccOpeningBalance>> Filter(PageRequestDto dto)
        {
            var queryable = await _accOpeningBalanceService.GetQueryableAsync();
            if (dto.FilterRows != null) 
            {
                foreach (var item in dto.FilterRows)
                {
                    queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
                }
            }

            if (dto.FilterAdvanced != null)
            {
                foreach (var item in dto.FilterAdvanced)
                {
                    queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
                }
            }
            return queryable;
        }

        #endregion
    }
}
