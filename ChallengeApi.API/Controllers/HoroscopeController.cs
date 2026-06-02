using ChallengeApi.Application.DTOs.Horoscope;
using ChallengeApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChallengeApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HoroscopeController : ControllerBase
    {
        private readonly IHoroscopeService _horoscopeService;

        public HoroscopeController(IHoroscopeService horoscopeService)
        {
            _horoscopeService = horoscopeService;
        }

        /// <summary>
        /// Obtiene el horóscopo del día para el usuario autenticado,
        /// según su signo zodiacal e informa los días hasta su próximo cumpleaños.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(HoroscopeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHoroscope()
        {
            var userId = GetUserIdFromToken();
            var result = await _horoscopeService.GetHoroscopeAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el historial de consultas de horóscopo del usuario autenticado.
        /// </summary>
        [HttpGet("historial")]
        [ProducesResponseType(typeof(IEnumerable<ConsultaHistorialDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHistorial()
        {
            var userId = GetUserIdFromToken();
            var result = await _horoscopeService.GetHistorialAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene las estadísticas de los signos más consultados.
        /// </summary>
        [HttpGet("estadisticas")]
        [ProducesResponseType(typeof(IEnumerable<SignoEstadisticaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEstadisticas()
        {
            var result = await _horoscopeService.GetEstadisticasAsync();
            return Ok(result);
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Token inválido.");

            return userId;
        }
    }
}
