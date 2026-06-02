using ChallengeApi.Application.DTOs.User;
using ChallengeApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChallengeApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado.
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.GetProfileAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Actualiza los datos del perfil del usuario autenticado.
        /// El nombre de usuario no puede modificarse.
        /// </summary>
        [HttpPut("me")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            var result = await _userService.UpdateProfileAsync(userId, dto);
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
