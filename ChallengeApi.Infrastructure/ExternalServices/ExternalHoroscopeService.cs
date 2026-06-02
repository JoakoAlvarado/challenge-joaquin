using ChallengeApi.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChallengeApi.Infrastructure.ExternalServices
{
    public class ExternalHoroscopeService : IExternalHoroscopeService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ExternalHoroscopeService> _logger;

        public ExternalHoroscopeService(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<ExternalHoroscopeService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string> GetHoroscopoTextoAsync(string signo, DateOnly fecha)
        {
            var cacheKey = $"horoscopo_{signo}_{fecha:yyyy-MM-dd}";

            // Intentar obtener desde Redis / IMemoryCache
            if (_cache.TryGetValue(cacheKey, out string? cached) && cached is not null)
            {
                _logger.LogInformation(
                    "Horóscopo obtenido desde caché para signo: {Signo}, fecha: {Fecha}",
                    signo, fecha);
                return cached;
            }

            _logger.LogInformation(
                "Consultando API externa para signo: {Signo}, fecha: {Fecha}",
                signo, fecha);

            var requestBody = new
            {
                date = fecha.ToString("yyyy-MM-dd"),
                lang = "es",
                sign = signo
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var horoscopoTexto = ParseHoroscopoResponse(responseJson);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                //AbsoluteExpiration = DateTime.UtcNow.Date.AddDays(1) // Medianoche UTC
                AbsoluteExpiration = DateTime.Now.Date.AddDays(1)
            };
            _cache.Set(cacheKey, horoscopoTexto, cacheOptions);

            _logger.LogInformation(
                "Horóscopo guardado en caché hasta medianoche para signo: {Signo}",
                signo);

            return horoscopoTexto;
        }

        private static string ParseHoroscopoResponse(string responseJson)
        {
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            // Intentamos extraer el texto del horóscopo de la respuesta
            if (root.TryGetProperty("horoscope", out var horoscope))
                return horoscope.GetString() ?? responseJson;

            if (root.TryGetProperty("data", out var data))
            {
                if (data.TryGetProperty("horoscope", out var nestedHoroscope))
                    return nestedHoroscope.GetString() ?? responseJson;
            }

            // Si no reconocemos retorno json completo
            return responseJson;
        }
    }
}
