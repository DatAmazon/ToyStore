using Microsoft.Extensions.DependencyInjection;
using ToyStoreManagement.Application.Interfaces;
using ToyStoreManagement.Application.Interfaces.IAdminService;
using ToyStoreManagement.Application.Services;
using ToyStoreManagement.Application.Services.AdminService;
using ToyStoreManagement.Domain.Interfaces;

namespace ToyStoreManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoriesService, CategoriesService>();

        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
