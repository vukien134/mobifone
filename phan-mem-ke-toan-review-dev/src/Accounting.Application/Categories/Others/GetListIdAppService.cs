using Accounting.BaseDtos;
using Accounting.Catgories.Others.FeeTypes;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class GetListIdAppService : AccountingAppService
    {
        #region Fields
        private readonly FeeTypeService _feeTypeService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        #endregion
        #region Ctor
        public GetListIdAppService(FeeTypeService feeTypeService,
                            UserService userService,
                            WebHelper webHelper
                            )
        {
            _feeTypeService = feeTypeService;
            _userService = userService;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<string> CreateListIdAsync(int number)
        {
            var str = "";
            for (var i = 0; i < number; i++)
            {
                str += Environment.NewLine + this.GetNewObjectId();
            }
            return str;
        }
        #region Private
        #endregion
    }
}
