using Airline.Config;
using Airline.Database.Context;
using Airline.Middleware;
using Airline.Repository;
using Airline.Repository.Flight;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<AuthenticationMiddleware>();
builder.Services.AddHttpContextAccessor();

#region Repository
builder.Services.AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
builder.Services.AddScoped<IFlightRepo, FlightRepo>();
#endregion


#region Database
var connectionString = builder.Configuration.GetConnectionString(builder.Environment.IsProduction() ? "MonsterServer" : "MonsterServer");
builder.Services.AddDbContext<AirlineDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureRepos();

#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseCors(cors => cors
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();

app.UseSwaggerUI(s =>
{
    s.DocExpansion(DocExpansion.None);
    s.DisplayRequestDuration();
    s.EnableTryItOutByDefault();
});

app.UseAuthorization();
app.UseAuthentication();

app.UseMiddleware<AuthenticationMiddleware>();


app.MapControllers();

app.Run();
