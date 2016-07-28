using Newtonsoft.Json;

namespace Mrtn
{
    public sealed class FlatList
    {
        [JsonProperty("src")]
        public string Source { get; set; }

        [JsonProperty("flats")]
        public Flat[] Flats { get; set; }
    }
}