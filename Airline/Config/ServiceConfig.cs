using Airline.Database.Context;
using Airline.Repository;
using Airline.Repository.Token;
using Airline.Service.Airport;
using Airline.Service.Book;
using Airline.Service.Country;
using Airline.Service.Flight;
using Airline.Service.Seat;
using Airline.Service.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Airline.Config;

public static class ServiceConfig
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
     => services.AddAuthentication(options =>
     {
         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
     })
     .AddJwtBearer(options =>
     {
         options.SaveToken = true;
         options.RequireHttpsMetadata = false;
         options.Events = new JwtBearerEvents
         {
             OnChallenge = context =>
             {
                 context.Response.StatusCode = 401;
                 return Task.CompletedTask;
             }
         };
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidAudience = configuration.GetSection("JwtConfig:validAudience").Value,
             ValidIssuer = configuration.GetSection("JwtConfig:validIssuer").Value,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:secret"])),
             ClockSkew = TimeSpan.Zero
         };
     });
    public static void ConfigureSwagger(this IServiceCollection services)
     => services.AddSwaggerGen(c =>
     {
         c.SwaggerDoc("v1", new OpenApiInfo
         {
             Title = Environment.GetEnvironmentVariable("ASPNETCORE_SWAGGER_TITLE"),
             Version = "v1",
             Description = "AirLine API Services.",
             Contact = new OpenApiContact
             {
                 Name = "AirLine"
             },
         });
         c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
         c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
         {
             Name = "Authorization",
             Type = SecuritySchemeType.ApiKey,
             Scheme = "Bearer",
             BearerFormat = "JWT",
             In = ParameterLocation.Header,
             Description = "JWT Authorization header using the Bearer scheme."
         });
         c.AddSecurityRequirement(new OpenApiSecurityRequirement
         {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
         });
     });

    public static void ConfigureRepos(this IServiceCollection services)
        => services
                   //.AddScoped<IDbRepo, DbRepo>()
                   .AddScoped<ITokenRepe, TokenRepo>(sp =>
                   {
                       var configuration = sp.GetRequiredService<IConfiguration>();
                       var jwtSettings = configuration.GetSection("JwtSettings");

                       return new TokenRepo(
                           sp.GetRequiredService<AirlineDbContext>(),
                           jwtSettings["DurationInHours"],
                           jwtSettings["Audience"],
                           jwtSettings["Issuer"],
                           jwtSettings["Key"],
                           // sp.GetRequiredService<IDbRepo>(),
                           sp.GetRequiredService<IHttpContextAccessor>(),
                           configuration
                       );
                   })
                    .AddScoped<IBookService, BookService>()
                    .AddScoped<IUserService, UserService>()
                    .AddScoped<IFlightService, FlightService>()
                    .AddScoped<ICountryService, CountryService>()
                    .AddScoped<IAirportService, AirportService>()
                    .AddScoped<IAirplaneService, AirplaneService>()
                    .AddScoped(typeof(IBaseRepo<>), typeof(BaseRepo<>));
}