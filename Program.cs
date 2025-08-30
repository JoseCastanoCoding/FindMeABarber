using FindMeABarber.Models;
using FindMeABarber.Services;
using Microsoft.AspNetCore.Mvc;

namespace FindMeABarber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
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

            app.UseAuthorization();

            

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
