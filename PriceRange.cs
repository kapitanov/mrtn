using Newtonsoft.Json;

namespace Mrtn
{
    public sealed class PriceRange
    {
        [JsonProperty("min")]
        public decimal Min { get; set; }

        [JsonProperty("avg")]
        public decimal Avg { get; set; }

        [JsonProperty("max")]
        public decimal Max { get; set; }
    }
}
