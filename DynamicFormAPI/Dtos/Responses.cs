namespace DynamicFormAPI.Dtos
{
    public class ApplicationFormRes
    {
        public string Id { get; set; }
        public string ProgramTitle { get; set; }
        public string ProgramDescription { get; set; }
        public bool PhoneInput { get; set; }
        public bool NationalityInput { get; set; }
        public bool CurrentResidenceInput { get; set; }
        public bool IdNumberInput { get; set; }
        public bool DateOfBirthInput { get; set; }
        public bool GenderInput { get; set; }
        public List<QuestionRes> Questions { get; set; }
    }

    public class QuestionRes
    {
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public string Content { get; set; }
        public List<string> Choices { get; set; }
    }

    public class SubmissionRes
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Nationality { get; set; }
        public string CurrentResidence { get; set; }
        public string IdNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Dictionary<string, List<string>> Responses { get; set; }
        public string SubmittedAt { get; set; }
    }

    public class QuestionTypeRes
    {
        public string Id { get; set; }
        public string TypeName { get; set; }
    }
}
