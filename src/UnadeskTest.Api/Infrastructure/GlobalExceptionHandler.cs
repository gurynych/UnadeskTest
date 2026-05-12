using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UnadeskTest.Api.Infrastructure
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Необработанное исключение.");

            (int status, string title) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Некорректный запрос."),
                _ => (StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.")
            };

            ProblemDetails problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
