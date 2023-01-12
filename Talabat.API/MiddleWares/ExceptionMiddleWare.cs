using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.API.Errors;

namespace Talabat.API.MiddleWares
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleWare> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex,ex.Message);
                //Log in DataBase
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //500
                var errorResponse = env.IsDevelopment() ?
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.InnerException.Message, ex.InnerException.StackTrace.ToString())
                    : 
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.InnerException.Message, ex.InnerException.StackTrace.ToString());


                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(errorResponse, options);
                await context.Response.WriteAsync(json);

            }
        }
    }
}
