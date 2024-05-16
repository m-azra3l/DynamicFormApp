using System.ComponentModel.DataAnnotations;

namespace DynamicFormAPI.Dtos
{
    public class CreateUpdateFormReq
    {
        public string ProgramTitle { get; set; }
        public string ProgramDescription { get; set; }
        public bool PhoneInput { get; set; }
        public bool NationalityInput { get; set; }
        public bool CurrentResidenceInput { get; set; }
        public bool IdNumberInput { get; set; }
        public bool DateOfBirthInput { get; set; }
        public bool GenderInput { get; set; }
        public List<QuestionReq> Questions { get; set; }
    }

    public class QuestionReq
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public List<string> Choices { get; set; }
        public bool AllowMultiple { get; set; }
        public bool IncludeOtherOption { get; set; }
    }

    public class SubmissionReq
    {
        public string FormId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Nationality { get; set; }
        public string? CurrentResidence { get; set; }
        public string? IdNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Dictionary<string, List<string>> Responses { get; set; }
    }
}
