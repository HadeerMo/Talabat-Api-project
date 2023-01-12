using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.core.IRepositories;
using Talabat.core.IServices;
using Talabat.repository;
using Talabat.service;

namespace Talabat.API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            services.Configure<ApiBehaviorOptions>(options =>
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = actionContext.ModelState.Where(M => M.Value.Errors.Count() > 0)
                                                     .SelectMany(M => M.Value.Errors)
                                                     .Select(E => E.ErrorMessage)
                                                     .ToArray();
                var errorResponse = new ApiValidationErrorResponse()
                {
                    Errors = errors
                };
                return new BadRequestObjectResult(errorResponse);
            });
            return services;
        }
    }
}
