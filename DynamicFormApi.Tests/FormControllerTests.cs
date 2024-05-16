using DynamicFormAPI.Controllers;
using DynamicFormAPI.Data;
using DynamicFormAPI.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace DynamicFormApi.Tests
{
    public class FormControllerTests
    {
        private readonly Mock<IFormService> _mockFormService;
        private readonly Mock<ILogger<FormController>> _mockLogger;
        private readonly FormController _controller;
        private readonly CreateUpdateFormReq _validRequest;

        public FormControllerTests()
        {
            _mockFormService = new Mock<IFormService>();
            _mockLogger = new Mock<ILogger<FormController>>();
            _controller = new FormController(_mockFormService.Object, _mockLogger.Object);

            _validRequest = new CreateUpdateFormReq
            {
                ProgramTitle = "New Form",
                ProgramDescription = "Description",
                PhoneInput = true,
                NationalityInput = false,
                CurrentResidenceInput = true,
                IdNumberInput = true,
                DateOfBirthInput = true,
                GenderInput = true,
                Questions = new List<QuestionReq>
            {
                new QuestionReq { Type = "Choice", Content = "Select one", Choices = new List<string> { "Option1", "Option2" }, AllowMultiple = false }
            }
            };

            _controller.ModelState.Clear();
        }

        // Test that the endpoint to create form should return ok upon successful form creation
        [Fact(DisplayName = "Create Form should return OK when the form is successfully created")]
        public async Task CreateForm_ReturnsOk_WhenFormIsSuccessfullyCreated()
        {
            // Arrange
            _mockFormService.Setup(x => x.CreateForm(It.IsAny<CreateUpdateFormReq>())).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateForm(_validRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.True(response.Successful);
            Assert.Equal("Form created", response.Message);
        }

        // Test that the create form endpoint should return bad request if model state is invalid
        [Fact(DisplayName = "Create Form should return Bad Request when model state is invalid")]
        public async Task CreateForm_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("ProgramTitle", "Required");

            // Act
            var result = await _controller.CreateForm(_validRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ModelStateErrorResponseDTO>(badRequestResult.Value);
        }

        // Test that the update form should return ok upon successful update
        [Fact(DisplayName = "Update Form should return OK when the form is successfully updated")]
        public async Task UpdateForm_ReturnsOk_WhenFormIsSuccessfullyUpdated()
        {
            // Arrange
            _mockFormService.Setup(x => x.GetFormAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationFormRes());
            _mockFormService.Setup(x => x.UpdateForm(It.IsAny<CreateUpdateFormReq>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateForm(_validRequest, "formId");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<MessageResponse>(okResult.Value);
            Assert.True(response.Successful);
            Assert.Equal("Form updated", response.Message);
        }

        // Test that update form endpoint should return not found if the form doesn't exist
        [Fact(DisplayName = "Update Form should return Not Found when the form does not exist")]
        public async Task UpdateForm_ReturnsNotFound_WhenFormDoesNotExist()
        {
            // Arrange
            _mockFormService.Setup(x => x.GetFormAsync(It.IsAny<string>())).ReturnsAsync((ApplicationFormRes)null);

            // Act
            var result = await _controller.UpdateForm(_validRequest, "invalidFormId");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<ErrorResponseDTO>(notFoundResult.Value);
        }

        // Test that the get form by ID endpoint should return ok when the form exist
        [Fact(DisplayName = "Get Form By ID should return OK when the form exists")]
        public async Task GetFormById_ReturnsOk_WhenFormExists()
        {
            // Arrange
            string formId = "existing-form-id";
            var mockForm = new ApplicationFormRes
            {
                Id = formId,
                ProgramTitle = "Test Title",
                ProgramDescription = "Test Description"
            };

            _mockFormService.Setup(x => x.GetFormAsync(formId)).ReturnsAsync(mockForm);

            // Act
            var result = await _controller.GetFormById(formId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<ApplicationFormRes>>(okResult.Value);
            Assert.True(response.Successful);
            Assert.Equal("Success", response.Message);
            Assert.Equal(mockForm, response.Data);
        }

        // Test that the get form by ID endpoint should return not found if it doesn't exist
        [Fact(DisplayName = "Get Form By ID should return Not Found when the form does not exist")]
        public async Task GetFormById_ReturnsNotFound_WhenFormDoesNotExist()
        {
            // Arrange
            string formId = "nonexistent-form-id";
            _mockFormService.Setup(x => x.GetFormAsync(formId)).ReturnsAsync((ApplicationFormRes)null);

            // Act
            var result = await _controller.GetFormById(formId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var errorResponse = Assert.IsType<ErrorResponseDTO>(notFoundResult.Value);
            Assert.Equal("Form not found", errorResponse.ErrorMessages);
            Assert.Equal(HttpStatusCode.NotFound, errorResponse.Status);
        }

        // Test that the get form by ID endpoint should return internal server error
        [Fact(DisplayName = "Get Form By ID should return Internal Server Error when an exception occurs")]
        public async Task GetFormById_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            string formId = "form-id";
            _mockFormService.Setup(x => x.GetFormAsync(formId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetFormById(formId);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Test exception", internalServerErrorResult.Value);
        }

    }
}
