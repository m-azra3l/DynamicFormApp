using Newtonsoft.Json;

namespace DynamicFormAPI.Models
{
    public class QuestionType
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("typeName")]
        public string TypeName { get; set; }
    }
}
