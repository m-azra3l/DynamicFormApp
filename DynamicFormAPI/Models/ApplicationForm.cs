using Newtonsoft.Json;

namespace DynamicFormAPI.Models
{
    public class ApplicationForm
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string ProgramTitle { get; set; }
        [JsonProperty("description")]
        public string ProgramDescription { get; set; }
        [JsonProperty("phoneInput")]
        public bool PhoneInput { get; set; }
        [JsonProperty("nationalityInput")]
        public bool NationalityInput { get; set; }
        [JsonProperty("currentResidenceInput")]
        public bool CurrentResidenceInput { get; set; }
        [JsonProperty("idNumberInput")]
        public bool IdNumberInput { get; set; }
        [JsonProperty("dateOfBirthInput")]
        public bool DateOfBirthInput { get; set; }
        [JsonProperty("genderInput")]
        public bool GenderInput { get; set; }
        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }
}
