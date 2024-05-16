using Newtonsoft.Json;

namespace DynamicFormAPI.Models
{
    public class Submission
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("formId")]
        public string FormId { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone")]
        public string? Phone { get; set; }
        [JsonProperty("nationality")]
        public string? Nationality { get; set; }
        [JsonProperty("currentResidence")]
        public string? CurrentResidence { get; set; }
        [JsonProperty("idNumber")]
        public string? IdNumber { get; set; }
        [JsonProperty("dateOfBirth")]
        public string? DateOfBirth { get; set; }
        [JsonProperty("gender")]
        public string? Gender { get; set; }
        [JsonProperty("responses")]
        public Dictionary<string, List<string>> Responses { get; set; }
        [JsonProperty("submittedAt")]
        public string SubmittedAt { get; set; }
    }
}
