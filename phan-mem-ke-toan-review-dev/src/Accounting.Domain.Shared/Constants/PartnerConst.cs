using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Constants
{
    public class PartnerConst
    {
        public static Dictionary<int, string> TypePartner = new Dictionary<int, string>()
        {
            { 1 , "Cá nhân" },
            { 2 , "Doanh nghiệp" },
            { 3 , "Nhân viên" },
            { 4 , "Khác" }
        };
    }
}
