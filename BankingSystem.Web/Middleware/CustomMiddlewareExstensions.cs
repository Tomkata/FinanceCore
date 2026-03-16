using System.Runtime.CompilerServices;

namespace BankingSystem.Web.Middleware
{
    public static class  CustomMiddlewareExstensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
