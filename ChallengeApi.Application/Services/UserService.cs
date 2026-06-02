using ChallengeApi.Application.DTOs.User;
using ChallengeApi.Application.Helpers;
using ChallengeApi.Application.Interfaces.Repositories;
using ChallengeApi.Application.Interfaces.Services;
using ChallengeApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            _logger.LogInformation("Obteniendo perfil para userId: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new UserNotFoundException(userId);

            return new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                BirthDate = user.BirthDate,
                Signo = ZodiacHelper.GetSigno(user.BirthDate),
                DiasHastaCumpleanos = ZodiacHelper.GetDiasHastaCumpleanos(user.BirthDate)
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto)
        {
            _logger.LogInformation("Actualizando perfil para userId: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new UserNotFoundException(userId);

            user.Email = dto.Email;
            user.BirthDate = dto.BirthDate;
            user.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Perfil actualizado exitosamente para userId: {UserId}", userId);

            return await GetProfileAsync(userId);
        }
    }
}
