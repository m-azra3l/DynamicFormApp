using DynamicFormAPI.Data;
using DynamicFormAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DynamicFormAPI.Controllers
{
    // This controller handles form related services
    public class FormController : BaseController
    {
        private readonly IFormService formService;
        private readonly ILogger<FormController> logger;
        public FormController(IFormService formService, ILogger<FormController> logger)
        {
            this.formService = formService;
            this.logger = logger;
        }

        /// <summary>
        /// Endpoint to create a form
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPost("forms")]
        public async Task<IActionResult> CreateForm(CreateUpdateFormReq req)
        {
            try
            {
                // Checking if the incoming model is valid
                if (!ModelState.IsValid)
                {
                    // Returning a BadRequest response with details of invalid ModelState
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }
                var result = await formService.CreateForm(req);
                if (!result)
                {

                    var errorResponse = new ErrorResponseDTO(
                        HttpStatusCode.BadRequest,
                        "Error creating form"
                    );
                    return BadRequest(errorResponse);
                }
                var response = new MessageResponse
                {
                    Successful = true,
                    Message = "Form created"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"An error occurred: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to update a form
        /// </summary>
        /// <param name="req"></param>
        /// <param name="formId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPut("forms/{formId}")]
        public async Task<IActionResult> UpdateForm(CreateUpdateFormReq req, string formId)
        {
            try
            {
                // Checking if the incoming model is valid
                if (!ModelState.IsValid)
                {
                    // Returning a BadRequest response with details of invalid ModelState
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }
                var form = await formService.GetFormAsync(formId);
                if (form == null)
                {
                    var errorResponse = new ErrorResponseDTO(
                        HttpStatusCode.NotFound,
                        "Form not found"
                    );
                    return NotFound(errorResponse);
                }
                var result = await formService.UpdateForm(req, formId);

                if (!result)
                {

                    var errorResponse = new ErrorResponseDTO(
                        HttpStatusCode.BadRequest,
                        "Error updatinging form"
                    );
                    return BadRequest(errorResponse);
                }
                var response = new MessageResponse
                {
                    Successful = true,
                    Message = "Form updated"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"An error occurred: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to delete question by Id
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpDelete("questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(string questionId)
        {
            try
            {
                var result = await formService.DeleteQuestion(questionId);
                if (!result)
                {

                    var errorResponse = new ErrorResponseDTO(
                        HttpStatusCode.BadRequest,
                        "Error deleting question"
                    );
                    return BadRequest(errorResponse);
                }
                var response = new MessageResponse
                {
                    Successful = true,
                    Message = "Question deleted"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"Failed to delete question: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get question types
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResponseArrayDTO<QuestionTypeRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("question-types")]
        public async Task<IActionResult> GetQuestionTypes()
        {
            try
            {
                var result = await formService.GetQuestionTypesAsync();

                var response = new ResponseArrayDTO<QuestionTypeRes>
                {
                    Successful = true,
                    Message = "Success",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"An error occurred: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get inactive questions
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResponseArrayDTO<QuestionRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("inactive-questions")]
        public async Task<IActionResult> GetInactiveQuestions()
        {
            try
            {
                var result = await formService.GetInactiveQuestions();

                var response = new ResponseArrayDTO<QuestionRes>
                {
                    Successful = true,
                    Message = "Success",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"An error occurred while retrieving inactive questions: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get all forms
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResponseArrayDTO<ApplicationFormRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("forms")]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var result = await formService.GetFormsAsync();

                var response = new ResponseArrayDTO<ApplicationFormRes>
                {
                    Successful = true,
                    Message = "Success",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"An error occurred while retrieving forms: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get form by id
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Response<ApplicationFormRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("forms/{formId}")]
        public async Task<IActionResult> GetFormById(string formId)
        {
            try
            {
                var result = await formService.GetFormAsync(formId);
                if (result == null)
                {
                    var errorResponse = new ErrorResponseDTO(
                        HttpStatusCode.NotFound,
                        "Form not found"
                    );
                    return NotFound(errorResponse);
                }
                var response = new Response<ApplicationFormRes>
                {
                    Successful = true,
                    Message = "Success",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"An error occurred while retrieving the form: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
