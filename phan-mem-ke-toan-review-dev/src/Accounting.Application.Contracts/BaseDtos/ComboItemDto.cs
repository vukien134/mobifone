using System.Text.Json.Serialization;

namespace Accounting.BaseDtos
{
    public class ComboItemDto
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("display")]
        public string Display { get; set; }
    }
}
