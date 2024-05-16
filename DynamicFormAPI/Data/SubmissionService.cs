using DynamicFormAPI.Dtos;
using DynamicFormAPI.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace DynamicFormAPI.Data
{
    public class SubmissionService : ISubmissionService
    {
        private readonly Container submissionContainer;
        private readonly IConfiguration _config;
        public SubmissionService(
            CosmosClient cosmosClient,
            IConfiguration config
            )
        {
            _config = config;
            var databaseName = _config["ConnectionStrings:DatabaseName"];
            var submissionContainerName = _config["ConnectionStrings:SubmissionContainer"];
            this.submissionContainer = cosmosClient.GetContainer(databaseName, submissionContainerName);
        }

        public async Task<bool> SubmitForm(SubmissionReq req)
        {
            try
            {
                // Create a new submission object from the request
                var submission = new Submission
                {
                    Id = Guid.NewGuid().ToString(), // Generate a unique ID for each submission
                    FormId = req.FormId, // This should be passed to the function or included in the SubmissionReq
                    FirstName = req.FirstName,
                    LastName = req.LastName,
                    Email = req.Email,
                    Phone = req.Phone,
                    Nationality = req.Nationality,
                    CurrentResidence = req.CurrentResidence,
                    IdNumber = req.IdNumber,
                    DateOfBirth = req.DateOfBirth?.ToString("yyyy-MM-dd"), // Formatting Date of Birth if it is provided
                    Gender = req.Gender,
                    Responses = req.Responses,
                    SubmittedAt = DateTime.UtcNow.ToString("o") // ISO 8601 format
                };

                // Upsert the submission document in Cosmos DB
                await submissionContainer.UpsertItemAsync(submission, new PartitionKey(req.FormId));
                return true; // Successfully created the submission
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<SubmissionRes>> GetSubmissions()
        {
            List<SubmissionRes> submissions = new List<SubmissionRes>();
            try
            {
                var query = submissionContainer.GetItemLinqQueryable<Submission>(true);
                var iterator = query.ToFeedIterator();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    submissions.AddRange(response.Select(sub => new SubmissionRes
                    {
                        Id = sub.Id,
                        FormId = sub.FormId,
                        FirstName = sub.FirstName,
                        LastName = sub.LastName,
                        Email = sub.Email,
                        Phone = sub.Phone,
                        Nationality = sub.Nationality,
                        CurrentResidence = sub.CurrentResidence,
                        IdNumber = sub.IdNumber,
                        DateOfBirth = sub.DateOfBirth,
                        Gender = sub.Gender,
                        Responses = sub.Responses,
                        SubmittedAt = sub.SubmittedAt
                    }));
                }
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }

            return submissions;
        }

        public async Task<IEnumerable<SubmissionRes>> GetFormSubmissions(string formId)
        {
            List<SubmissionRes> submissions = new List<SubmissionRes>();
            try
            {
                var query = submissionContainer.GetItemLinqQueryable<Submission>(true)
                            .Where(sub => sub.FormId == formId);
                var iterator = query.ToFeedIterator();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    submissions.AddRange(response.Select(sub => new SubmissionRes
                    {
                        Id = sub.Id,
                        FormId = formId,
                        FirstName = sub.FirstName,
                        LastName = sub.LastName,
                        Email = sub.Email,
                        Phone = sub.Phone,
                        Nationality = sub.Nationality,
                        CurrentResidence = sub.CurrentResidence,
                        IdNumber = sub.IdNumber,
                        DateOfBirth = sub.DateOfBirth,
                        Gender = sub.Gender,
                        Responses = sub.Responses,
                        SubmittedAt = sub.SubmittedAt
                    }));
                }
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }

            return submissions;
        }

        public async Task<SubmissionRes?> GetSubmission(string id)
        {
            try
            {
                var response = await submissionContainer.ReadItemAsync<Submission>(id, new PartitionKey(id));
                if (response.Resource == null)
                {
                    return null; // Submission not found
                }
                var sub = response.Resource;
                return new SubmissionRes
                {
                    Id = sub.Id,
                    FormId = sub.FormId,
                    FirstName = sub.FirstName,
                    LastName = sub.LastName,
                    Email = sub.Email,
                    Phone = sub.Phone,
                    Nationality = sub.Nationality,
                    CurrentResidence = sub.CurrentResidence,
                    IdNumber = sub.IdNumber,
                    DateOfBirth = sub.DateOfBirth,
                    Gender = sub.Gender,
                    Responses = sub.Responses,
                    SubmittedAt = sub.SubmittedAt
                };
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
