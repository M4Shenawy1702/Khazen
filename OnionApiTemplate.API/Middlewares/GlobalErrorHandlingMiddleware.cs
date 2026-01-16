using Khazen.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Khazen.API.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;
        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                await HandleNotFoundPath(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                BadRequestException => (int)HttpStatusCode.BadRequest,
                ValidationException => (int)HttpStatusCode.BadRequest,
                ConflictException => (int)HttpStatusCode.Conflict,
                ConcurrencyException => (int)HttpStatusCode.Conflict,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var errors = new List<string>();

            if (ex is ValidationException validationEx)
            {
                errors.Add(validationEx.Message);

            }
            else
            {
                errors.Add(ex.Message ?? "An unexpected error occurred.");
            }

            var response = new
            {
                status = statusCode,
                title = GetTitleForStatusCode(statusCode),
                errors,
                instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleNotFoundPath(HttpContext context)
        {
            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";

                var response = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = $"The endpoint '{context.Request.Path}' was not found.",
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }

        private string GetTitleForStatusCode(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status500InternalServerError => "Internal Server Error",
            _ => "An error occurred"
        };

    }
}
