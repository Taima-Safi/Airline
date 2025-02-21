using Airline.Database.Context;
using Airline.Repository;
using Airline.Service.Airport;
using Airline.Service.Country;
using Airline.Service.User;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Repository
builder.Services.AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
#endregion

#region Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IAirportService, AirportService>();

#endregion

#region Database
var connectionString = builder.Configuration.GetConnectionString(builder.Environment.IsProduction() ? "Server" : "Server");
builder.Services.AddDbContext<AirlineDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(cors => cors
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
