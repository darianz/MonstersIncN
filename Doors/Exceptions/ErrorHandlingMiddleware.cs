using Doors.Exceptions;
using Doors.Exceptions.Doors.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Doors.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidTokenException ex)
            {
                _logger.LogWarning(ex, "Invalid token provided");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                var errorResponse = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message
                };
                await context.Response.WriteAsync(errorResponse.ToString());
            }
            catch (DoorNotFoundException ex)
            {
                _logger.LogWarning(ex, "Door not found");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                var errorResponse = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message
                };
                await context.Response.WriteAsync(errorResponse.ToString());
            }
            catch(DoorAlreadyUsedException ex)
            {
                _logger.LogError(ex, "An error occurred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorResponse = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message
                };
                await context.Response.WriteAsync(errorResponse.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorResponse = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message
                };
                await context.Response.WriteAsync(errorResponse.ToString());
            }
        }
    }
}
