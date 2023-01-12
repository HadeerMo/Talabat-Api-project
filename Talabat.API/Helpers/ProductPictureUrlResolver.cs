using AutoMapper;
using Microsoft.Extensions.Configuration;
using Talabat.API.Dtos;
using Talabat.core.Entities;

namespace Talabat.API.Helpers
{
    public class ProductPictureUrlResolver : IValueResolver<Product, ProductToReturnDTO, string>
    {
        public IConfiguration Configuration { get; }

        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public string Resolve(Product source, ProductToReturnDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
                return $"{Configuration["ApiBaseUrl"]}{source.PictureUrl}";
            return null;
        }
    }
}
