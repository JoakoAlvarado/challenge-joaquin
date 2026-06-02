using ChallengeApi.Application.Interfaces.Services;
using ChallengeApi.Application.Services;

namespace ChallengeApi.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHoroscopeService, HoroscopeService>();

            return services;
        }
    }
}
