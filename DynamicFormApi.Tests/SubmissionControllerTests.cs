using DynamicFormAPI.Controllers;
using DynamicFormAPI.Data;
using DynamicFormAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace DynamicFormApi.Tests
{
    public class SubmissionControllerTests
    {
        private readonly Mock<ISubmissionService> _mockSubmissionService;
        private readonly Mock<ILogger<SubmissionController>> _mockLogger;
        private readonly SubmissionController _controller;

        public SubmissionControllerTests()
        {
            _mockSubmissionService = new Mock<ISubmissionService>();
            _mockLogger = new Mock<ILogger<SubmissionController>>();
            _controller = new SubmissionController(_mockSubmissionService.Object, _mockLogger.Object);
        }

        // Tests that the SubmitForm endpoint returns an Ok result when the submission is successful
        [Fact(DisplayName = "Submit Form should return OK when submission is successful")]
        public async Task SubmitForm_ReturnsOk_WhenSubmissionIsSuccessful()
        {
            // Arrange
            var request = new SubmissionReq
            {
                FormId = "form123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Nationality = "Nigerian",
                CurrentResidence = "Nigerian",
                IdNumber = "AB1234567",
                DateOfBirth = new DateTime(1960, 10, 1), 
                Gender = "Male",
                Responses = new Dictionary<string, List<string>>
                {
                    { "question1", new List<string> { "Answer1" } },
                    { "question2", new List<string> { "Answer2a", "Answer2b" } }
                }
            };
            _mockSubmissionService.Setup(x => x.SubmitForm(It.IsAny<SubmissionReq>())).ReturnsAsync(true);

            // Act
            var result = await _controller.SubmitForm(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.True(response.Successful);
            Assert.Equal("Form submitted", response.Message);
        }

        // Test to ensure that SubmitForm returns BadRequest when the submission fails
        [Fact(DisplayName = "Submit Form should return BadRequest when submission fails")]
        public async Task SubmitForm_ReturnsBadRequest_WhenSubmissionFails()
        {
            // Arrange
            var request = new SubmissionReq
            {
                FormId = "form123", 
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Nationality = "Nigerian",
                CurrentResidence = "Nigerian",
                IdNumber = "AB1234567",
                DateOfBirth = new DateTime(1960, 10, 1), 
                Gender = "Male",
                Responses = new Dictionary<string, List<string>>
                {
                    { "question1", new List<string> { "Answer1" } },
                    { "question2", new List<string> { "Answer2a", "Answer2b" } }
                }
            };
            _mockSubmissionService.Setup(x => x.SubmitForm(It.IsAny<SubmissionReq>())).ReturnsAsync(false);

            // Act
            var result = await _controller.SubmitForm(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponseDTO>(badRequestResult.Value);
            Assert.Equal("Error submitting form", errorResponse.ErrorMessages);
        }

        // Test to ensure that the endpoint returns list of submissions
        [Fact(DisplayName = "Get Submissions should return OK with a list of submissions")]
        public async Task GetSubmissions_ReturnsOk_WithSubmissions()
        {
            // Arrange
            var submissions = new List<SubmissionRes>() 
            {
                new SubmissionRes
                {
                    Id = "submission123",
                    FormId = "form123",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Phone = "123-456-7890",
                    Nationality = "Nigerian",
                    CurrentResidence = "Nigerian",
                    IdNumber = "AB1234567",
                    DateOfBirth = "1990-10-01",
                    Gender = "Male",
                    Responses = new Dictionary<string, List<string>>
                    {
                        { "question1", new List<string> { "Answer1" } },
                        { "question2", new List<string> { "Answer2a", "Answer2b" } }
                    }
                }
            }; 
            _mockSubmissionService.Setup(x => x.GetSubmissions()).ReturnsAsync(submissions);

            // Act
            var result = await _controller.GetSubmissions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseArrayDTO<SubmissionRes>>(okResult.Value);
            Assert.True(response.Successful);
            Assert.Equal("Success", response.Message);
            Assert.Equal(submissions, response.Data);
        }

        // Test to ensure that the endpoint returns a submission provided the specified id
        [Fact(DisplayName = "Get Submission By ID should return OK with the specified submission")]
        public async Task GetSubmissionById_ReturnsOk_WithSubmission()
        {
            // Arrange
            string submissionId = "submission123";
            var submission = new SubmissionRes 
            {
                Id = "submission123",
                FormId = "form123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Nationality = "Nigerian",
                CurrentResidence = "Nigerian",
                IdNumber = "AB1234567",
                DateOfBirth = "1990-10-01",
                Gender = "Male",
                Responses = new Dictionary<string, List<string>>
                {
                    { "question1", new List<string> { "Answer1" } },
                    { "question2", new List<string> { "Answer2a", "Answer2b" } }
                }
            }; 
            _mockSubmissionService.Setup(x => x.GetSubmission(submissionId)).ReturnsAsync(submission);

            // Act
            var result = await _controller.GetSubmissionById(submissionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<SubmissionRes>>(okResult.Value);
            Assert.True(response.Successful);
            Assert.Equal("Success", response.Message);
            Assert.Equal(submission, response.Data);
        }
    }
}
