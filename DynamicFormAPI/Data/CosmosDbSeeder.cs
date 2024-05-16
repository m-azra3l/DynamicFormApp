using DynamicFormAPI.Models;
using Microsoft.Azure.Cosmos;

namespace DynamicFormAPI.Data
{
    public class CosmosDbSeeder
    {
        private readonly Container questionTypeContainer;
        private readonly IConfiguration _config;

        public CosmosDbSeeder(
            CosmosClient cosmosClient,
            IConfiguration config
            )
        {
            _config = config;

            var databaseName = _config["ConnectionStrings:DatabaseName"];
            var questionTypeContainerName = _config["ConnectionStrings:QuestionTypeContainer"];
            this.questionTypeContainer = cosmosClient.GetContainer(databaseName, questionTypeContainerName);
        }

        // Function to seed question types if non exists
        public async Task SeedDataAsync()
        {
            var existingData = questionTypeContainer.GetItemLinqQueryable<QuestionType>(true).ToList();
            if (!existingData.Any())
            {
                await CreateQuestionTypesAsync();
            }
        }

        // Private function to create question types
        private async Task CreateQuestionTypesAsync()
        {
            var questionTypes = new List<QuestionType>
        {
            new QuestionType { Id = "1", TypeName = "Paragraph" },
            new QuestionType { Id = "2", TypeName = "YesNo" },
            new QuestionType { Id = "3", TypeName = "Dropdown" },
            new QuestionType { Id = "4", TypeName = "MultipleChoice" },
            new QuestionType { Id = "5", TypeName = "Date" },
            new QuestionType { Id = "6", TypeName = "Number" }
        };

            foreach (var type in questionTypes)
            {
                // Exception handling for each insert operation
                try
                {
                    await questionTypeContainer.CreateItemAsync(type, new PartitionKey(type.Id));
                }
                catch (CosmosException ex)
                {
                    // Log the exception or handle it according to your logging strategy
                    Console.WriteLine($"Error inserting item: {ex.Message}");
                }
            }
        }
    }
}
