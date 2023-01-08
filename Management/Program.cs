using Management.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();

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
