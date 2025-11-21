using NordClan.BookingApp.Api.Exceptions;
using System.Net;

namespace NordClan.BookingApp.Api.Middleware
{
    public class BookingExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BookingExceptionMiddleware> _logger;

        public BookingExceptionMiddleware(RequestDelegate next, ILogger<BookingExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BookingValidationException ex)
            {
                await HandleDomainExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (BookingOverlapException ex)
            {
                await HandleDomainExceptionAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (BookingNotFoundException ex)
            {
                await HandleDomainExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (LoginValidationException ex)
            {
                await HandleDomainExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidCredentialsException ex)
            {
                await HandleDomainExceptionAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (BookingAccessDeniedException ex)
            {
                await HandleDomainExceptionAsync(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("Внутренняя ошибка сервера");
                }
            }
        }

        private static async Task HandleDomainExceptionAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "text/plain; charset=utf-8";

            await context.Response.WriteAsync(message);
        }
    }
}
