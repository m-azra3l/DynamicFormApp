using DynamicFormAPI.Dtos;

namespace DynamicFormAPI.Data
{
    public interface IFormService
    {
        // Post, Put, and Delete functions
        Task<bool> CreateForm(CreateUpdateFormReq req);
        Task<bool> UpdateForm(CreateUpdateFormReq req, string formId);
        Task<bool> DeleteQuestion(string questionId);

        // Get functions
        Task<IEnumerable<QuestionTypeRes>> GetQuestionTypesAsync();
        Task<IEnumerable<QuestionRes>> GetInactiveQuestions();
        Task<IEnumerable<ApplicationFormRes>> GetFormsAsync();
        Task<ApplicationFormRes?> GetFormAsync(string formId);

    }
}
