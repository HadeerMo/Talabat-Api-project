using AutoMapper;
using Talabat.API.Dtos;
using Talabat.core.Entities;
using Talabat.core.Entities.Identity;
using Talabat.core.Entities.OrderAggregate;

namespace Talabat.API.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDTO>()
                     .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                     .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                     .ForMember(d=>d.PictureUrl,o=>o.MapFrom<ProductPictureUrlResolver>());
            CreateMap<AddressDto,core.Entities.Identity.Address>().ReverseMap();
            CreateMap<CustomerBasketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<AddressDto, core.Entities.OrderAggregate.Address>();
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PicturUrl))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());

        }
    }
}
