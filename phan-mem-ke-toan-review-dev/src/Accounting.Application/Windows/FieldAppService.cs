using Accounting.BaseDtos;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Windows
{
    public class FieldAppService : AccountingAppService,IFieldAppService
    {
        #region Fields
        private readonly FieldService _fieldService;
        private readonly UserService _userService;
        #endregion
        #region Ctor
        public FieldAppService(FieldService fieldService,
                        UserService userService)
        {
            _fieldService = fieldService;
            _userService = userService;
        }
        #endregion
        public async Task<FieldDto> CreateAsync(CrudFieldDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            var entity = ObjectMapper.Map<CrudFieldDto, Field>(dto);
            var result = await _fieldService.CreateAsync(entity);
            return ObjectMapper.Map<Field, FieldDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _fieldService.DeleteAsync(id);
        }

        public async Task<PageResultDto<FieldDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<FieldDto>();
            var query = await Filter(dto);
            var count = dto.Count == 0 ? 100 : dto.Count;
            var querysort = query.OrderBy(p => p.TabId)
                                .ThenBy(p => p.Ord)
                                .Skip(dto.Start).Take(count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Field, FieldDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudFieldDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            var entity = await _fieldService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _fieldService.UpdateAsync(entity);
        }
        #region Private
        private async Task<IQueryable<Field>> Filter(PageRequestDto dto)
        {
            var queryable = await _fieldService.GetQueryableAsync();
            return queryable;
        }
        #endregion
    }
}
