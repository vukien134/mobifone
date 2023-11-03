using Accounting.BaseDtos;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Windows;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Windows
{
    public class TabAppService : AccountingAppService, ITabAppService
    {
        #region Fields
        private readonly TabService _tabService;
        private readonly UserService _userService;
        #endregion
        #region Ctor
        public TabAppService(TabService tabService,
                        UserService userService)
        {
            _tabService = tabService;
            _userService = userService;
        }
        #endregion
        public async Task<TabDto> CreateAsync(CrudTabDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            var entity = ObjectMapper.Map<CrudTabDto, Tab>(dto);
            var result = await _tabService.CreateAsync(entity);
            return ObjectMapper.Map<Tab, TabDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _tabService.DeleteAsync(id);
        }

        public async Task<PageResultDto<TabDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<TabDto>();
            var query = await Filter(dto);
            var count = dto.Count == 0 ? 100 : dto.Count;
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Tab, TabDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task UpdateAsync(string id, CrudTabDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            var entity = await _tabService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _tabService.UpdateAsync(entity);
        }
        #region Private
        private async Task<IQueryable<Tab>> Filter(PageRequestDto dto)
        {
            var queryable = await _tabService.GetQueryableAsync();
            return queryable;
        }
        #endregion
    }
}
