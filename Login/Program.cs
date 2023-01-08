using Login;
using Login.Interfaces;
using Login.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//  .AddJwtBearer(options =>
//  {
//      options.TokenValidationParameters = new TokenValidationParameters
//      {
//          ValidateActor = false,
//          ValidateIssuer = true,
//          ValidateAudience = true,
//          ValidateLifetime = true,
//          ValidateIssuerSigningKey = true,
//          ValidIssuer = builder.Configuration["Jwt:Issuer"],
//          ValidAudience = builder.Configuration["Jwt:Audience"],
//          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//      };
//  });

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("http://example.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
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
//app.UseAuthentication();

app.UseCors("MyPolicy");
app.UseHttpsRedirection();


app.MapControllers();


app.Run();
