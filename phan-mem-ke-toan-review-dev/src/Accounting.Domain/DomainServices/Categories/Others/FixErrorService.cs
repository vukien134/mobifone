using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Accounting.DomainServices.Categories.Others
{
    public class FixErrorService : BaseDomainService<FixError, string>
    {
        private readonly IObjectMapper _objectMapper;
        private readonly WebHelper _webHelper;
        private readonly DefaultFixErrorService _defaultFixErrorService;

        public FixErrorService(IRepository<FixError, string> repository,
                               IObjectMapper objectMapper,
                               DefaultFixErrorService defaultFixErrorService,
                               WebHelper webHelper
                               ) : base(repository)
        {
            _objectMapper = objectMapper;
            _webHelper = webHelper;
            _defaultFixErrorService = defaultFixErrorService;
        }

        public async Task<List<FixError>> GetData(string orgCode)
        {
            var fixError = await this.GetQueryableAsync();
            var lstFixError = fixError.Where(p => p.OrgCode == orgCode).ToList();
            if (lstFixError.Count() == 0)
            {
                var defaultAllotmentForwardCategory = await _defaultFixErrorService.GetQueryableAsync();
                lstFixError = defaultAllotmentForwardCategory
                                           .Select(p => _objectMapper.Map<DefaultFixError, FixError>(p)).ToList();
                foreach (var item in lstFixError)
                {
                    item.OrgCode = orgCode;
                }
            }
            return lstFixError;
        }
    }
}
