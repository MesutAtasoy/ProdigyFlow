using Microsoft.AspNetCore.Mvc;
using ProdigyFlow.Application;
using ProdigyFlow.Application.Services;
using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/products", async (ProductService service) => await service.GetAllProductsAsync()).WithOpenApi();

app.MapGet("/products/{id}", async (Guid id, ProductService service) => await service.GetProductAsync(id))
    .WithOpenApi();

app.MapPost("/products", async ([FromBody] ProductCreateRequest request, [FromServices] ProductService service) =>
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Sku = request.Sku,
            Barcode = request.Barcode
        };

        await service.AddProductAsync(product);
    })
    .WithOpenApi();

app.MapPut("/products/{id}", async (Guid id, [FromBody]Product product, ProductService service) =>
{
    product.Id = id;
    await service.UpdateProductAsync(product);
}).WithOpenApi();

app.MapDelete("/products/{id}", async (Guid id, ProductService service) => await service.DeleteProductAsync(id));

app.MapGet("/categories", async (CategoryService service) => await service.GetAllCategoriesAsync())
    .WithOpenApi();

app.MapGet("/categories/{id}", async (Guid id, CategoryService service) => await service.GetCategoryAsync(id))
    .WithOpenApi();

app.MapPost("/categories", async ([FromBody] CategoryCreateRequest request, CategoryService service) =>
    {
        var category = new Category
        {
            Name = request.Name
        };

        await service.AddCategoryAsync(category);
    })
    .WithOpenApi();

app.MapPut("/categories/{id}", async (Guid id, [FromBody] CategoryUpdateRequest request, CategoryService service) =>
    {
        var category = new Category
        {
            Id = id,
            Name = request.Name
        };

        await service.UpdateCategoryAsync(category);
    })
    .WithOpenApi();

app.MapDelete("/categories/{id}", async (Guid id, CategoryService service) =>
    {
        await service.DeleteCategoryAsync(id);
    })
    .WithOpenApi();

app.Run();
