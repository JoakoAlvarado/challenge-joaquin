using ChallengeApi.Application.Interfaces.Repositories;
using ChallengeApi.Application.Interfaces.Services;
using ChallengeApi.Infrastructure.ExternalServices;
using ChallengeApi.Infrastructure.Persistence.Context;
using ChallengeApi.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // SQL Server
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("ChallengeApi.Infrastructure")));

            // Caché en memoria
            services.AddMemoryCache();

            // Repositorios
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IConsultaHistorialRepository, ConsultaHistorialRepository>();

            // HttpClient para API externa
            services.AddHttpClient<IExternalHoroscopeService, ExternalHoroscopeService>(client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ExternalApis:HoroscopeApiUrl"]!);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            return services;
        }
    }
}
