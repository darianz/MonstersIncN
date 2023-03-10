using Doors.Exceptions;
using Doors.Middleware;
using Doors.Models;
using Doors.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Doors API", Version = "v1" });

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

builder.Services.AddScoped<DoorService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();


// Add the database connection
try
{
    builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




}
catch (Exception e)
{
    Console.WriteLine(e);
}
var app = builder.Build();
// Connect to or create the database
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
    try
    {
        context.Database.EnsureCreated();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }

}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();


app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedDoors(context);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();



