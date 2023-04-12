using AutoMapper;
using FirstApi.Dtos.CategoryDto;
using FirstApi.Dtos.ProductDto;
using FirstApi.Extention;
using FirstApi.Models;

namespace FirstApi.Profiles
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<Category, CategoryReturnDto>()
                .ForMember(d => d.ImageUrl, map => map.MapFrom(src => "https://localhost:7076/img/" + src.ImageUrl))
                .ForMember(d => d.Time, map => map.MapFrom(src => src.CreateDate.CalculateTime()));

            CreateMap<Category, CategoryInProductReturnDto>();


            CreateMap<Product, ProductReturnDto>()
                .ForMember(d => d.Profit, map => map.MapFrom(src => src.SalePrice - src.CostPrice));
        }
    }
}
