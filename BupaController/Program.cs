using Bupa.Interface;
using Bupa.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register services in DI container
builder.Services.AddHttpClient<BookService>();
builder.Services.AddControllers();
// Register HttpClient for BookService
builder.Services.AddHttpClient<BookService>(client =>
{
    // Use the value from the appsettings.json or environment variables
    client.BaseAddress = new Uri(builder.Configuration["ExternalApiUrl"]);
});
// Register IBookService and BookService in the DI container
builder.Services.AddScoped<IBookService, BookService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
