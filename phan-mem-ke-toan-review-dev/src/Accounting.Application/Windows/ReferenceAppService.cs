using Accounting.BaseDtos;
using Accounting.Constants;
using Accounting.DomainServices.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Windows
{
    public class ReferenceAppService : AccountingAppService, IReferenceAppService
    {
        #region Fields
        private readonly ReferenceService _referenceService;
        private readonly ReferenceDetailService _referenceDetailService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        #endregion
        #region Ctor
        public ReferenceAppService(ReferenceService referenceService,
                                    IUnitOfWorkManager unitOfWorkManager,
                                    ReferenceDetailService referenceDetailService)
        {
            _referenceService = referenceService;
            _unitOfWorkManager = unitOfWorkManager;
            _referenceDetailService = referenceDetailService;
        }
        #endregion
        public async Task<ReferenceDto> CreateAsync(CrudReferenceDto dto)
        {
            dto.Id = this.GetNewObjectId();            
            var entity = ObjectMapper.Map<CrudReferenceDto, Reference>(dto);
            var result = await _referenceService.CreateAsync(entity);
            return ObjectMapper.Map<Reference, ReferenceDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _referenceService.DeleteAsync(id);
        }

        public async Task<PageResultDto<ReferenceDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ReferenceDto>();
            var query = await Filter(dto);
            var count = dto.Count == 0 ? 100 : dto.Count;
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Reference, ReferenceDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudReferenceDto dto)
        {
            var entity = await _referenceService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            try
            {
                var details = await _referenceDetailService.GetByReferenceIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (details != null)
                {
                    await _referenceDetailService.DeleteManyAsync(details);
                }
                await _referenceService.UpdateAsync(entity);                
                await unitOfWork.CompleteAsync();
            }
            catch(Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw ;
            }
        }
        public async Task<List<ComboItemDto>> GetValuesAsync(string referenceId)
        {
            var reference = await _referenceService.GetAsync(referenceId);
            if (reference.RefType != ReferenceType.List) return null;

            var result = reference.ListType switch
            {
                ReferenceListType.RString => GetValuesFromStringArray(reference.ListValue),
                ReferenceListType.RJson => GetValuesFromStringJson(reference.ListValue),
                _ => null
            };
            
            return result;
        }
        public async Task<ReferenceDto> GetByIdAsync(string referenceId)
        {
            var reference = await _referenceService.GetAsync(referenceId);
            return ObjectMapper.Map<Reference, ReferenceDto>(reference);
        }
        #region Private
        private async Task<IQueryable<Reference>> Filter(PageRequestDto dto)
        {
            var queryable = await _referenceService.GetQueryableAsync();
            return queryable;
        }
        private List<ComboItemDto> GetValuesFromStringArray(string listValue)
        {
            var result = new List<ComboItemDto>();
            string[] parts = listValue.Split(',');
            foreach (string part in parts)
            {
                var item = new ComboItemDto()
                {
                    Id = part,
                    Value = part
                };
                result.Add(item);
            }
            return result;
        }
        private List<ComboItemDto> GetValuesFromStringJson(string listValue)
        {
            var result = JsonSerializer.Deserialize<List<ComboItemDto>>(listValue);            
            return result;
        }
        #endregion
    }
}
