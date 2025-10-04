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
            Price = request.Price
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

app.Run();
