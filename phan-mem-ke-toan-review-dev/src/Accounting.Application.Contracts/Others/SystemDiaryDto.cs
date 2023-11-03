using Accounting.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Others
{
    public class SystemDiaryDto
    {
        public string UserName { get; set; }

        [JsonDateTimeFormat("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ExecutionTime { get; set; }
        public int? ExcutionDuration { get; set; }
        public string ClientIpAddress { get; set; }
        public string BrowserInfo { get; set; }
        public string HttpMethod { get; set; }
        public string Url { get; set; }
        public string Exceptions { get; set; }
        public int? HttpStatusCode { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public string Paramters { get; set; }
    }
}
