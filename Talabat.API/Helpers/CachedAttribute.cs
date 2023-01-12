using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.IServices;

namespace Talabat.API.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSecound;

        public CachedAttribute(int timeToLiveInSecound)
        {
            _timeToLiveInSecound = timeToLiveInSecound;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var cacheResponse = await cacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cacheResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            var executedEndPointContext = await next();
            if(executedEndPointContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.CachResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSecound));

            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path);
            foreach(var (key, value) in request.Query.OrderBy(x=>x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
