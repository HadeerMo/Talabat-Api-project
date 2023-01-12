using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.core.IServices
{
    public interface IResponseCacheService
    {
        Task CachResponseAsync(string cachekey, object response, TimeSpan timeToLive);
        Task<string> GetCachedResponseAsync(string cachekey);
    }
}
