using Accounting.BaseDtos;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;

namespace Accounting.Others
{
    public class SystemDiaryAppService : AccountingAppService
    {
        #region Fields        
        private readonly IAuditLogRepository _auditLogRepository;
        #endregion
        #region Ctor
        public SystemDiaryAppService(IAuditLogRepository auditLogRepository)
        {            
            _auditLogRepository = auditLogRepository;            
        }
        #endregion
        #region Methods
        [Authorize(AccountingPermissions.SystemDiaryManagerView)]
        public async Task<PageResultDto<SystemDiaryDto>> PagesAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<SystemDiaryDto>();
            var queryable = await this.Filter(dto);
            var querySort = queryable.OrderByDescending(p => p.ExecutionTime)
                        .Skip(dto.Start).Take(dto.Count);
            var entities = querySort.ToList();
            var dtos = entities.Select(p =>
            {
                var dto = new SystemDiaryDto()
                {
                    BrowserInfo = p.BrowserInfo,
                    ClientIpAddress = p.ClientIpAddress,
                    Exceptions = p.Exceptions,
                    ExcutionDuration = p.ExecutionDuration,
                    ExecutionTime = p.ExecutionTime,
                    HttpMethod = p.HttpMethod,
                    HttpStatusCode = p.HttpStatusCode,
                    Url = p.Url,
                    UserName = p.UserName
                };
                if (p.Actions.Count > 0)
                {
                    var auditLogAction = p.Actions.First<AuditLogAction>();
                    dto.MethodName = auditLogAction.MethodName;
                    dto.Paramters = auditLogAction.Parameters;
                    dto.ServiceName = auditLogAction.ServiceName;
                }
                return dto;
            }).ToList();
            result.Data = dtos;
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(queryable);
            }
            return result;
        }
        #endregion
        #region Privates
        private async Task<IQueryable<AuditLog>> Filter(PageRequestDto dto)
        {
            var queryable = await _auditLogRepository.WithDetailsAsync();            
            return queryable;
        }
        #endregion
    }
}
