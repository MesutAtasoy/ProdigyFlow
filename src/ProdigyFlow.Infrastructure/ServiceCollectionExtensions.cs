using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdigyFlow.Domain.Repositories;
using ProdigyFlow.Infrastructure.Repositories;

namespace ProdigyFlow.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ProdigyFlow"));
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}