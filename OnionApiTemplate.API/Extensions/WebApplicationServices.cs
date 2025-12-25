using Khazen.API.Factories;
using Khazen.API.Middlewares;
using Khazen.Application.Common.Configurations;
using Khazen.Infrastructure.Common.Setting;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

namespace Khazen.API.Extensions
{
    public static class WebApplicationServices
    {
        public static IServiceCollection AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(configuration)
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .CreateLogger();

            host.UseSerilog();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                rateLimiterOptions.AddPolicy("GlobalControllerPolicy", context =>
                {
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        return RateLimitPartition.GetSlidingWindowLimiter(
                            context.User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                            key => new SlidingWindowRateLimiterOptions
                            {
                                Window = TimeSpan.FromSeconds(10),
                                SegmentsPerWindow = 5,
                                PermitLimit = 10,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0
                            });
                    }
                    else
                    {
                        return RateLimitPartition.GetSlidingWindowLimiter(
                            context.Connection.RemoteIpAddress?.ToString()!,
                            key => new SlidingWindowRateLimiterOptions
                            {
                                Window = TimeSpan.FromSeconds(60),
                                SegmentsPerWindow = 6,
                                PermitLimit = 15,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                QueueLimit = 0
                            });
                    }
                });
            });

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.CustomValidationErrorResponse;
            });
            ConfigureJWT(services, configuration);
            services.AddAuthorization();
            services.AddSwaggerServices();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }

        private static void ConfigureJWT(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailConfigurations>(configuration.GetSection("EmailConfigurations"));
            services.Configure<TwilioSmsConfigurations>(configuration.GetSection("Twilio"));

            var jwt = configuration.GetSection("JWT").Get<JWTConfigurations>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt!.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}
