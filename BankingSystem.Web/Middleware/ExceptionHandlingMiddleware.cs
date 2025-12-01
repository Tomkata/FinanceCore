using BankingSystem.Domain.Exceptions;

namespace BankingSystem.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                await HandleExceptionAsync(context, 400, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, 500, ex.ToString());
            }
        }

        private static Task HandleExceptionAsync(HttpContext context,int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(new
            {
                error = message
            });
        }
    }
}
