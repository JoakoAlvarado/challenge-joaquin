using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace ChallengeApi.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Challenge API - Horóscopo",
                    Version = "v1",
                    Description = "API REST para consulta de horóscopo diario con autenticación JWT."
                });

                // Definición del esquema de seguridad JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingresá el token JWT. Ejemplo: Bearer {tu_token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            return services;
        }
    }
}
