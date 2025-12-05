using Khazen.API.Factories;
using Khazen.API.Middlewares;
using Khazen.Application.Common.Configurations;
using Khazen.Infrastructure.Common.Setting;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

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
