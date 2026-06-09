using AutoMapper;
using ToyStoreManagement.Application.DTOs;
using ToyStoreManagement.Application.DTOs.Admin;
using ToyStoreManagement.Domain.Entities;

namespace ToyStoreManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : "N/A"));

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoryId))
                .ReverseMap();
            // 2. Map tên danh mục (ví dụ DB là Title, DTO là Name)
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))

            CreateMap<ProductUpdateDto, Product>()
                // Ánh xạ trường Id của DTO vào trường ProductId của Entity
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                // Bỏ qua không tự động map trường ImageUrl từ file nhị phân IFormFile
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<ProductCreateDto, Product>()
                .ReverseMap();

        }
    }
}
