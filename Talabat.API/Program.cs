using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis; 
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talabat.API.Extensions;
using Talabat.API.MiddleWares;
using Talabat.core.Entities.Identity;
using Talabat.repository.Data;
using Talabat.repository.Data.Identity;
using Talabat.repository.Identity;

namespace Talabat.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region configure services
            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));


            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(s =>
            {
                var connection = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
                return ConnectionMultiplexer.Connect(connection);
            });
            builder.Services.AddApplicationServices();

            builder.Services.AddSwaggerServices();
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
            #endregion

            var app = builder.Build();

            #region Apply migration and data seeding
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = services.GetRequiredService<StoreContext>();
                await context.Database.MigrateAsync();
                var identityContext = services.GetRequiredService<AppIdentityDbContext>();
                await identityContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(context, loggerFactory);
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
            }
            catch (Exception ex)
            {
                var Logger = loggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, ex.Message); //or: "an error occured during apply Migration"
            }
            #endregion

            #region configure
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleWare>();
            if (app.Environment.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwaggerDocumentation();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #endregion

            app.Run();


        }


    }
}
