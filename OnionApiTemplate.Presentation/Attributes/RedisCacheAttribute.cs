using Khazen.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;

namespace Khazen.Presentation.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSeconds;

        public RedisCacheAttribute(int durationInSeconds = 90)
        {
            _durationInSeconds = durationInSeconds;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = GetCacheKey(context.HttpContext.Request);

            var cachedValue = await cacheService.GetAsync<string>(cacheKey);

            if (!string.IsNullOrEmpty(cachedValue))
            {
                context.Result = new ContentResult
                {
                    Content = cachedValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }

            var executedContext = await next.Invoke();

            if (executedContext.Result is ObjectResult objectResult &&
                executedContext.HttpContext.Response.StatusCode == StatusCodes.Status200OK)
            {
                var json = JsonSerializer.Serialize(objectResult.Value);
                await cacheService.SetAsync(cacheKey, json, TimeSpan.FromSeconds(_durationInSeconds));
            }
        }

        private static string GetCacheKey(HttpRequest request)
        {
            var sb = new StringBuilder();
            sb.Append(request.Path);

            if (request.Query.Count != 0)
                sb.Append('?');

            foreach (var q in request.Query.OrderBy(x => x.Key))
            {
                sb.Append($"{q.Key}={q.Value}&");
            }

            return sb.ToString().TrimEnd('&');
        }

    }
}
