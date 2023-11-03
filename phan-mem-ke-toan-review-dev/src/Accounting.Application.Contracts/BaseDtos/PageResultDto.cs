using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Accounting.BaseDtos
{
    public class PageResultDto<T>
    {
        public List<T> Data { get; set; }
        public int Pos { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
    }
}
