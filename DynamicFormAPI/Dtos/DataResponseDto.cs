using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Net;

namespace DynamicFormAPI.Dtos
{
    // Abstract class for data response
    public abstract class DataResponseAbstractDTO
    {
        public HttpStatusCode Status { get; set; }
        public bool IsSuccess { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ErrorResponseDTO : DataResponseAbstractDTO
    {
        public string ErrorMessages { get; set; }

        public ErrorResponseDTO(HttpStatusCode statusCode, string errors)
        {
            Status = statusCode;
            ErrorMessages = errors;
            IsSuccess = false;
        }
    }

    public class ModelStateErrorResponseDTO : DataResponseAbstractDTO
    {
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public ModelStateErrorResponseDTO(HttpStatusCode statusCode, ModelStateDictionary modelStateErrors)
        {
            Status = statusCode;
            ErrorMessages.AddRange(modelStateErrors.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            IsSuccess = false;
        }
    }

    public class Response<T>
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }

    public class MessageResponse
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
    }

    public class ResponseArrayDTO<T>
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public IEnumerable<T?> Data { get; set; }
    }
}
