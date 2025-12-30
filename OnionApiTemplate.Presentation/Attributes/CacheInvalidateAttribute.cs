using Khazen.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Khazen.Presentation.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheInvalidateAttribute(string pattern) : Attribute, IAsyncActionFilter
    {
        private readonly string _pattern = pattern;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (resultContext.HttpContext.Response.StatusCode is >= 200 and < 300)
            {
                var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

                var wildcardPattern = _pattern.EndsWith('*') ? _pattern : $"{_pattern}*";

                var finalPattern = $"Khazen_{wildcardPattern}";

                await cacheService.RemoveByPatternAsync(finalPattern);
            }
        }
    }
}