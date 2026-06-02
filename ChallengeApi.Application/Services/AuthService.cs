using ChallengeApi.Application.DTOs.Auth;
using ChallengeApi.Application.Interfaces.Repositories;
using ChallengeApi.Application.Interfaces.Services;
using ChallengeApi.Domain.Entities;
using ChallengeApi.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            _logger.LogInformation("Intento de registro para email: {Email}", dto.Email);

            var exists = await _userRepository.ExistsAsync(dto.Email, dto.Username);
            if (exists)
            {
                _logger.LogWarning("Registro fallido, ya existe usuario con email o username: {Email}", dto.Email);
                throw new UserAlreadyExistsException(dto.Email);
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                BirthDate = dto.BirthDate,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            _logger.LogInformation("Usuario registrado exitosamente: {Username}", user.Username);

            return GenerateToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            _logger.LogInformation("Intento de login para email: {Email}", dto.Email);

            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login fallido para email: {Email}", dto.Email);
                throw new InvalidCredentialsException();
            }

            _logger.LogInformation("Login exitoso para usuario: {Username}", user.Username);

            return GenerateToken(user);
        }

        private AuthResponseDto GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddHours(
                double.Parse(jwtSettings["ExpirationHours"]!));

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                ExpiresAt = expiresAt
            };
        }
    }
}
