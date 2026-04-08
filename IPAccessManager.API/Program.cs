
using IPAccessManager.Application.Interfaces;
using IPAccessManager.Infrastructure.Services;
using IPAccessManager.Infrastructure.Repositories;
namespace IPAccessManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
			
            builder.Services.AddHttpClient<IIpGeoLocationService, IpGeoLocationService>();

			builder.Services.AddSingleton<IBlockedCountryRepository, BlockedCountryRepository>();
			builder.Services.AddSingleton<IAttemptLogRepository, AttemptLogRepository>();

			builder.Services.AddHostedService<ExpiredCountryCleanupService>();
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
        }
    }
}
