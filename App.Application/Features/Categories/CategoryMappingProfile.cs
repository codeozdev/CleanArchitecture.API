using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Response;
using App.Application.Features.Categories.Update;
using App.Domain.Entities;
using AutoMapper;

namespace App.Application.Features.Categories;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryResponse>().ReverseMap();
        CreateMap<Category, CategoryWithProduct>().ReverseMap();
        CreateMap<CreateCategoryRequest, Category>().ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));

        CreateMap<UpdateCategoryRequest, Category>().ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name.ToLowerInvariant()));
    }
}