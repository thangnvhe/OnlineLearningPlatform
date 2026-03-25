using OnlineLearningPlatform.API.Helpers;
using OnlineLearningPlatform.API.Middlewares;
using OnlineLearningPlatform.DataAccess;
using OnlineLearningPlatform.Infrastruture;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});
// Cấu hình Swagger để hỗ trợ DateOnly
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<DateOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
});

// Cấu hình Global Exception Handler và ProblemDetails
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Cấu hình Identity 
builder.Services.ConfigureIdentity(builder.Configuration);
// Cấu hình AutoMapper
builder.Services.AddAutoMappers();
// Cấu hình DI
builder.Services.AddDependencyInjection();

var app = builder.Build();

app.AutoMigration().GetAwaiter().GetResult();
app.SeedData(builder.Configuration).GetAwaiter().GetResult();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
