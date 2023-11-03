using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting
{
    public static class ErrorCode
    {
        public const string NotFoundEntity = "404";
        public const string Duplicate = "409"; //Trùng mã
        public const string NotStartWithParentCode = "432";
        public const string Other = "498";
        public const string IsApproval = "433";
        public const string IsUsing = "434";
        public const string NotRegisterLic = "436";
        public const string FileNotFound = "504";
        public const string IsParentGroup = "435";
        public static string Get(GroupErrorCodes groupErrorCodes, string code)
        {
            return $"{(int)groupErrorCodes}:{code}";
        }
    }
}
