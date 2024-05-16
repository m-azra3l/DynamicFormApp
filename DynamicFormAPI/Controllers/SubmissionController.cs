using DynamicFormAPI.Data;
using DynamicFormAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DynamicFormAPI.Controllers
{
    // This controller handles submission related services
    public class SubmissionController : BaseController
    {

        private readonly ISubmissionService subService;
        private readonly ILogger<SubmissionController> logger;

        public SubmissionController(ISubmissionService subService, ILogger<SubmissionController> logger)
        {
            this.subService = subService;
            this.logger = logger;
        }

        /// <summary>
        /// Endpoin to submit form response
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPost("submissions")]
        public async Task<IActionResult> SubmitForm(SubmissionReq req)
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
                var result = await subService.SubmitForm(req);
                if (!result)
                {

                    var errorResponse = new ErrorResponseDTO(
                        HttpStatusCode.BadRequest,
                        "Error submitting form"
                    );
                    return BadRequest(errorResponse);
                }
                var response = new MessageResponse
                {
                    Successful = true,
                    Message = "Form submitted"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handling exceptions and logging errors
                logger.LogError($"An error occurred while submitting the form: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get all submissions
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResponseArrayDTO<SubmissionRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("submissions")]
        public async Task<IActionResult> GetSubmissions()
        {
            try
            {
                var result = await subService.GetSubmissions();

                var response = new ResponseArrayDTO<SubmissionRes>
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
                logger.LogError($"An error occurred while retrieving submissions: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get submissions by a form Id
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResponseArrayDTO<SubmissionRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("submissions/form/{formId}")]
        public async Task<IActionResult> GetFormSubmissions(string formId)
        {
            try
            {
                var result = await subService.GetFormSubmissions(formId);

                var response = new ResponseArrayDTO<SubmissionRes>
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
                logger.LogError($"An error occurred while retrieving form submissions: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get a submission by id
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Response<SubmissionRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("submissions/{submissionId}")]
        public async Task<IActionResult> GetSubmissionById(string submissionId)
        {
            try
            {
                var result = await subService.GetSubmission(submissionId);
                if (result == null)
                {
                    var errorResponse = new ErrorResponseDTO(
                        HttpStatusCode.NotFound,
                        "Submission not found"
                    );
                    return NotFound(errorResponse);
                }
                var response = new Response<SubmissionRes>
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
                logger.LogError($"Failed to retrieve submission: {ex.Message}");

                // Returning a 500 Internal Server Error with the exception message
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
