using App.API.Filters;
using App.Application.Contracts.Caching;
using App.Application.Extensions;
using App.Caching;
using App.Persistence.Extensions;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Filter for FluentValidation
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true; // Suppress implicit required attribute for non-nullable reference types
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379")
);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

// Application Services
builder.Services.AddRepositories(builder.Configuration).AddServices(builder.Configuration);


// Filters
builder.Services.AddScoped(typeof(IdCheckFilter<>));
builder.Services.AddScoped(typeof(NameCheckFilter<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
