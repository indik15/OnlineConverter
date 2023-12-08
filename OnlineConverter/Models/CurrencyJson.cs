using Newtonsoft.Json;

namespace OnlineConverter.Models
{
    public class CurrencyJson
    {
        [JsonProperty("txt")]
        public string Name {  get; init; }

        [JsonProperty("cc")]
        public string Code { get; init; }

        [JsonProperty("rate")]
        public double Price { get; init; }
    }
}
