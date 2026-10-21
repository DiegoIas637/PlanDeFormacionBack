using Application.Orders.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Orders.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<User, OrderUserResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<Product, OrderProductResponse>();

            CreateMap<Order, OrderDetailResponse>()
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.AppliedPrice * src.Quantity));
        }
    }
}
