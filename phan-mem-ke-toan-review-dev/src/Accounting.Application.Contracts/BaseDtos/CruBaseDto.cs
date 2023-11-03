using System.Text.Json.Serialization;

namespace Accounting.BaseDtos
{
    public abstract class CruBaseDto
    {
        public string Id { get; set; }

        [JsonIgnore]
        public string CreatorName { get; set; }

        [JsonIgnore]
        public string LastModifierName { get; set; }
    }
}
