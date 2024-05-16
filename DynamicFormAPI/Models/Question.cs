using Newtonsoft.Json;

namespace DynamicFormAPI.Models
{
    public class Question
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
        [JsonProperty("formId")]
        public string FormId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("allowMultiple")]
        public bool AllowMultiple { get; set; }
        [JsonProperty("includeOtherOption")]
        public bool IncludeOtherOption { get; set; }
        [JsonProperty("choices")]
        public List<string> Choices { get; set; }
    }
}
