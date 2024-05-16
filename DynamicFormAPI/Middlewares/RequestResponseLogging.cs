namespace DynamicFormAPI.Middlewares
{
    public class RequestResponseLogging
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestResponseLogging> logger;

        public RequestResponseLogging(RequestDelegate next,
            ILogger<RequestResponseLogging> logger
            )
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log the request
            logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

            // Copy the original response body stream
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream to capture the response
            using (var responseBody = new MemoryStream())
            {
                // Set the response body stream to the memory stream
                context.Response.Body = responseBody;

                // Continue processing the request
                await next(context);

                // Log the response
                var response = await FormatResponse(context.Response);
                logger.LogInformation($"Response: {response}");

                // Copy the captured response to the original response body stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"{response.StatusCode}: {text}";
        }
    }
}
