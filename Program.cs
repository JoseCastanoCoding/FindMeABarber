using FindMeABarber.Models;
using FindMeABarber.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FindMeABarber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // CORS setup
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFindMeABarberFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);

            // JWT Authentication setup
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });
            
            builder.Services.AddAuthorization();

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5); // Default is 2 minutes
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2); // Default is 1 minute
            });

            builder.Services.AddScoped<IDbService, DbService>();
            builder.Services.AddScoped<IBarberService, BarberService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowFindMeABarberFrontend");

            app.MapGet("/api/hello", () =>
                Results.Ok(new { message = "Hello from .NET!" }));

            app.MapPost("/api/auth/register", async (RegisterDto dto, UserManager<AppUser> userManager) =>
            {
                var user = new AppUser { UserName = dto.Email, Email = dto.Email };
                var result = await userManager.CreateAsync(user, dto.Password);
                return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
            });

            app.MapPost("/api/auth/login", async (LoginDto dto, UserManager<AppUser> userManager, ITokenService tokenService) =>
            {
                var user = await userManager.FindByEmailAsync(dto.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, dto.Password))
                {
                    var token = tokenService.CreateToken(user);
                    return Results.Ok(new { token });
                }
                return Results.Unauthorized();
            });

            app.MapGet("/api/secure", [Authorize] () => "This is protected!");


            app.MapGet("/barbers", async ([FromServices] IBarberService barberService) =>
                await barberService.GetBarberList());

            app.MapGet("/barbers/{barberId}", async (int barberId, [FromServices] IBarberService barberService) =>
            {
                var barber = await barberService.GetBarber(barberId);
                if (barber == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(barber);
            });

            app.MapPost("/barbers", async (Barber barber, [FromServices] IBarberService barberService) =>
            {
                await barberService.CreateBarber(barber);
                return Results.Created($"/barbers/{barber.BarberId}", barber);
            });

            app.MapDelete("/barbers/{barberId}", async (int barberId, [FromServices] IBarberService barberService) =>
            {
                var deleted = await barberService.DeleteBarber(barberId);
                if (!deleted)
                {
                    return Results.NotFound();
                }
                return Results.NoContent();
            });

            app.Run();
        }
    }
}
