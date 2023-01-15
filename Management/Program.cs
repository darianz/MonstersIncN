using Management.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Management API", Version = "v1" });

    c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Authorization"
                }
            },
            new string[] {}
        }
    });


});
builder.Services.AddScoped<TokenService>();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
builder.Services.AddScoped<WorkdayCacheService>();
builder.Services.AddScoped<DoorService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<WorkdayService>();
builder.Services.AddHttpClient("LoginClient", c =>
{
    c.BaseAddress = new Uri("https://localhost:7216/Sign/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("DoorsClient", c =>
{
    c.BaseAddress = new Uri("https://localhost:7173/Doors/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
