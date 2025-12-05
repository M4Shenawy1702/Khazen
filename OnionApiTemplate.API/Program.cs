using Khazen.API.Extensions;
using Khazen.Application;
using Khazen.Domain.IRepositoty;
using Khazen.Infrastructure;
using Khazen.Infrastructure.Common.Setting;
using Serilog;

namespace Khazen.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddWebApplicationServices(builder.Configuration, builder.Host);
            builder.Services.RegisterApplicationServices();
            builder.Services.AddHttpContextAccessor();

            builder.Services.Configure<JWTConfigurations>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await dbInitializer.InitializeDatabaseAsync();
                await dbInitializer.InitializeIdentityAsync();
            }

            app.UseCustomExceptionHandling();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                });
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}
