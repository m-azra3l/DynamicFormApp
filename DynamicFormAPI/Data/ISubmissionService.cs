using DynamicFormAPI.Dtos;

namespace DynamicFormAPI.Data
{
    public interface ISubmissionService
    {
        // Post function
        Task<bool> SubmitForm(SubmissionReq req);

        // Get Functions
        Task<IEnumerable<SubmissionRes>> GetSubmissions();
        Task<IEnumerable<SubmissionRes>> GetFormSubmissions(string formId);
        Task<SubmissionRes?> GetSubmission(string id);
    }
}
