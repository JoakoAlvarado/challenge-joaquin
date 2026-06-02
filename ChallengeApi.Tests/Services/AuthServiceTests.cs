using ChallengeApi.Application.DTOs.Auth;
using ChallengeApi.Application.Interfaces.Repositories;
using ChallengeApi.Application.Services;
using ChallengeApi.Domain.Entities;
using ChallengeApi.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChallengeApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            // Configuración JWT en memoria para los tests
            var inMemorySettings = new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", "ChallengeApiSuperSecretKey2026!MustBe32CharsMin" },
            { "JwtSettings:Issuer", "ChallengeApi" },
            { "JwtSettings:Audience", "ChallengeApiUsers" },
            { "JwtSettings:ExpirationHours", "8" }
        };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _configuration,
                _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_UsuarioNuevo_DebeRetornarToken()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "testuser",
                Email = "test@test.com",
                Password = "Password123!",
                BirthDate = new DateOnly(1990, 6, 15)
            };

            _userRepositoryMock
                .Setup(r => r.ExistsAsync(dto.Email, dto.Username))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _authService.RegisterAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Token.Should().NotBeNullOrEmpty();
            resultado.Username.Should().Be(dto.Username);
            resultado.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task RegisterAsync_UsuarioExistente_DebeLanzarUserAlreadyExistsException()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "testuser",
                Email = "test@test.com",
                Password = "Password123!",
                BirthDate = new DateOnly(1990, 6, 15)
            };

            _userRepositoryMock
                .Setup(r => r.ExistsAsync(dto.Email, dto.Username))
                .ReturnsAsync(true);

            // Act
            var action = async () => await _authService.RegisterAsync(dto);

            // Assert
            await action.Should().ThrowAsync<UserAlreadyExistsException>();
        }

        [Fact]
        public async Task LoginAsync_CredencialesValidas_DebeRetornarToken()
        {
            // Arrange
            var password = "Password123!";
            var dto = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = password
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                BirthDate = new DateOnly(1990, 6, 15)
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            // Act
            var resultado = await _authService.LoginAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Token.Should().NotBeNullOrEmpty();
            resultado.Username.Should().Be(user.Username);
        }

        [Fact]
        public async Task LoginAsync_UsuarioNoExiste_DebeLanzarInvalidCredentialsException()
        {
            // Arrange
            var dto = new LoginRequestDto
            {
                Email = "noexiste@test.com",
                Password = "Password123!"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            // Act
            var action = async () => await _authService.LoginAsync(dto);

            // Assert
            await action.Should().ThrowAsync<InvalidCredentialsException>();
        }

        [Fact]
        public async Task LoginAsync_PasswordIncorrecta_DebeLanzarInvalidCredentialsException()
        {
            // Arrange
            var dto = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "PasswordIncorrecta!"
            };

            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("PasswordCorrecta123!"),
                BirthDate = new DateOnly(1990, 6, 15)
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            // Act
            var action = async () => await _authService.LoginAsync(dto);

            // Assert
            await action.Should().ThrowAsync<InvalidCredentialsException>();
        }
    }
}
