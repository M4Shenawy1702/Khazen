using Khazen.API.Middlewares;

namespace Khazen.API.Extensions
{
    public static class CustomExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }
    }
}
