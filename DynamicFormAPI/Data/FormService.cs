using DynamicFormAPI.Dtos;
using DynamicFormAPI.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace DynamicFormAPI.Data
{
    public class FormService : IFormService
    {
        private readonly Container formContainer;
        private readonly Container questionContainer;
        private readonly Container questionTypeContainer;
        private readonly IConfiguration _config;

        public FormService(
            CosmosClient cosmosClient,
            IConfiguration config
            )
        {
            _config = config;

            var databaseName = _config["ConnectionStrings:DatabaseName"];
            var formContainerName = _config["ConnectionStrings:FormContainer"];
            var questionContainerName = _config["ConnectionStrings:QuestionContainer"];
            var questionTypeContainerName = _config["ConnectionStrings:QuestionTypeContainer"];

            this.formContainer = cosmosClient.GetContainer(databaseName, formContainerName);
            this.questionContainer = cosmosClient.GetContainer(databaseName, questionContainerName);
            this.questionTypeContainer = cosmosClient.GetContainer(databaseName, questionTypeContainerName);
        }

        // Post, Put, and Delete functions

        public async Task<bool> CreateForm(CreateUpdateFormReq req)
        {
            var form = new ApplicationForm
            {
                Id = Guid.NewGuid().ToString(),  // Generate a new unique identifier
                ProgramTitle = req.ProgramTitle,
                ProgramDescription = req.ProgramDescription,
                PhoneInput = req.PhoneInput,
                NationalityInput = req.NationalityInput,
                CurrentResidenceInput = req.CurrentResidenceInput,
                IdNumberInput = req.IdNumberInput,
                DateOfBirthInput = req.DateOfBirthInput,
                GenderInput = req.GenderInput,
                Questions = req.Questions.Select(q => new Question
                {
                    Id = Guid.NewGuid().ToString(),  // Generate new ID for each question
                    Type = q.Type,
                    Content = q.Content,
                    AllowMultiple = q.AllowMultiple,
                    IncludeOtherOption = q.IncludeOtherOption,
                    Choices = q.Choices,
                }).ToList()
            };

            try
            {
                await formContainer.CreateItemAsync(form, new PartitionKey(form.Id));
                return true; // Success
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateForm(CreateUpdateFormReq req, string formId)
        {
            try
            {
                // Retrieve the existing form using the form ID
                var formResponse = await formContainer.ReadItemAsync<ApplicationForm>(formId, new PartitionKey(formId));
                var existingForm = formResponse.Resource;

                if (existingForm == null)
                {
                    return false; // Form not found
                }

                // Create a transactional batch
                TransactionalBatch batch = formContainer.CreateTransactionalBatch(new PartitionKey(formId));

                // Set existing questions to inactive
                foreach (var question in existingForm.Questions)
                {
                    question.IsActive = false;
                    batch = batch.ReplaceItem(question.Id, question);
                }

                // Add new questions provided in the request
                foreach (var questionReq in req.Questions)
                {
                    var newQuestion = new Question
                    {
                        Id = Guid.NewGuid().ToString(), // Assign a new ID for each new question
                        FormId = formId,
                        Type = questionReq.Type,
                        Content = questionReq.Content,
                        AllowMultiple = questionReq.AllowMultiple,
                        IncludeOtherOption = questionReq.IncludeOtherOption,
                        Choices = questionReq.Choices,
                        IsActive = true
                    };
                    batch = batch.CreateItem(newQuestion);
                }

                // Execute the batch operation
                TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

                if (batchResponse.IsSuccessStatusCode)
                {
                    return true; // Success
                }
                else
                {
                    Console.WriteLine($"Batch operation failed with status code: {batchResponse.StatusCode}");
                    return false; // Failure
                }
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteQuestion(string questionId)
        {
            try
            {
                var response = await questionContainer.DeleteItemAsync<Question>(questionId, new PartitionKey(questionId));
                return true;
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Get functions

        public async Task<IEnumerable<QuestionTypeRes>> GetQuestionTypesAsync()
        {
            // Prepare a LINQ query to fetch all QuestionType items from the Cosmos DB container
            var query = this.questionTypeContainer.GetItemLinqQueryable<QuestionType>(true)
                .Select(qt => new QuestionTypeRes
                {
                    Id = qt.Id,
                    TypeName = qt.TypeName
                });

            // Create an iterator to execute the query
            var iterator = query.ToFeedIterator();

            // List to hold the results
            List<QuestionTypeRes> questionTypes = new List<QuestionTypeRes>();

            // Iterate through the results
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                questionTypes.AddRange(response.ToList());
            }

            return questionTypes;
        }

        public async Task<IEnumerable<QuestionRes>> GetInactiveQuestions()
        {
            List<QuestionRes> inactiveQuestions = new List<QuestionRes>();
            try
            {
                // Create a LINQ query to select inactive questions
                var query = questionContainer.GetItemLinqQueryable<Question>()
                    .Where(q => !q.IsActive)
                    .Select(q => new QuestionRes
                    {
                        Type = q.Type,
                        IsActive = q.IsActive,
                        Content = q.Content,
                        AllowMultiple = q.AllowMultiple,
                        IncludeOtherOption = q.IncludeOtherOption,
                        Choices = q.Choices
                    });

                // Fetch the results
                var iterator = query.ToFeedIterator();
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    inactiveQuestions.AddRange(response);
                }

                return inactiveQuestions;
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ApplicationFormRes>> GetFormsAsync()
        {
            List<ApplicationFormRes> forms = new List<ApplicationFormRes>();
            try
            {
                var query = formContainer.GetItemLinqQueryable<ApplicationForm>(true);
                var iterator = query.ToFeedIterator();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    foreach (var form in response)
                    {
                        var formRes = new ApplicationFormRes
                        {
                            Id = form.Id,
                            ProgramTitle = form.ProgramTitle,
                            ProgramDescription = form.ProgramDescription,
                            PhoneInput = form.PhoneInput,
                            NationalityInput = form.NationalityInput,
                            CurrentResidenceInput = form.CurrentResidenceInput,
                            IdNumberInput = form.IdNumberInput,
                            DateOfBirthInput = form.DateOfBirthInput,
                            GenderInput = form.GenderInput,
                            Questions = form.Questions.Where(q => q.IsActive).Select(q => new QuestionRes
                            {
                                Type = q.Type,
                                IsActive = q.IsActive,
                                Content = q.Content,
                                AllowMultiple = q.AllowMultiple,
                                IncludeOtherOption = q.IncludeOtherOption,
                                Choices = q.Choices
                            }).ToList()
                        };
                        forms.Add(formRes);
                    }
                }
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }

            return forms;
        }
        public async Task<ApplicationFormRes?> GetFormAsync(string formId)
        {
            try
            {
                var form = await formContainer.ReadItemAsync<ApplicationForm>(formId, new PartitionKey(formId));
                if (form.Resource == null)
                {
                    return null; // Form not found
                }

                return new ApplicationFormRes
                {
                    Id = form.Resource.Id,
                    ProgramTitle = form.Resource.ProgramTitle,
                    ProgramDescription = form.Resource.ProgramDescription,
                    PhoneInput = form.Resource.PhoneInput,
                    NationalityInput = form.Resource.NationalityInput,
                    CurrentResidenceInput = form.Resource.CurrentResidenceInput,
                    IdNumberInput = form.Resource.IdNumberInput,
                    DateOfBirthInput = form.Resource.DateOfBirthInput,
                    GenderInput = form.Resource.GenderInput,
                    Questions = form.Resource.Questions.Where(q => q.IsActive).Select(q => new QuestionRes
                    {
                        Type = q.Type,
                        IsActive = q.IsActive,
                        Content = q.Content,
                        AllowMultiple = q.AllowMultiple,
                        IncludeOtherOption = q.IncludeOtherOption,
                        Choices = q.Choices
                    }).ToList()
                };
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
