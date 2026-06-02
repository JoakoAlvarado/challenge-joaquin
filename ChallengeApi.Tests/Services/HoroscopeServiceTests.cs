using ChallengeApi.Application.Interfaces.Repositories;
using ChallengeApi.Application.Interfaces.Services;
using ChallengeApi.Application.Services;
using ChallengeApi.Domain.Entities;
using ChallengeApi.Domain.Exceptions;
using FluentAssertions;
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
    public class HoroscopeServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConsultaHistorialRepository> _historialRepositoryMock;
        private readonly Mock<IExternalHoroscopeService> _externalServiceMock;
        private readonly Mock<ILogger<HoroscopeService>> _loggerMock;
        private readonly HoroscopeService _horoscopeService;

        public HoroscopeServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _historialRepositoryMock = new Mock<IConsultaHistorialRepository>();
            _externalServiceMock = new Mock<IExternalHoroscopeService>();
            _loggerMock = new Mock<ILogger<HoroscopeService>>();

            _horoscopeService = new HoroscopeService(
                _userRepositoryMock.Object,
                _historialRepositoryMock.Object,
                _externalServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetHoroscopeAsync_UsuarioValido_DebeRetornarHoroscopo()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                BirthDate = new DateOnly(1990, 9, 25) // Libra
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            _externalServiceMock
                .Setup(s => s.GetHoroscopoTextoAsync("Libra", It.IsAny<DateOnly>()))
                .ReturnsAsync("Hoy es un gran día para Libra.");

            _historialRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ConsultaHistorial>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _horoscopeService.GetHoroscopeAsync(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Signo.Should().Be("Libra");
            resultado.Horoscopo.Should().Be("Hoy es un gran día para Libra.");
            resultado.DiasHastaCumpleanos.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task GetHoroscopeAsync_UsuarioNoExiste_DebeLanzarUserNotFoundException()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            // Act
            var action = async () => await _horoscopeService.GetHoroscopeAsync(999);

            // Assert
            await action.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task GetHoroscopeAsync_DebeGuardarConsultaEnHistorial()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@test.com",
                BirthDate = new DateOnly(1990, 9, 25)
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            _externalServiceMock
                .Setup(s => s.GetHoroscopoTextoAsync(It.IsAny<string>(), It.IsAny<DateOnly>()))
                .ReturnsAsync("Texto del horóscopo.");

            _historialRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ConsultaHistorial>()))
                .Returns(Task.CompletedTask);

            // Act
            await _horoscopeService.GetHoroscopeAsync(1);

            // Assert — verificamos que se llamó al repositorio de historial exactamente una vez
            _historialRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<ConsultaHistorial>()),
                Times.Once);
        }

        [Fact]
        public async Task GetEstadisticasAsync_DebeOrdenarPorCantidadDescendente()
        {
            // Arrange
            var historial = new List<ConsultaHistorial>
        {
            new() { Signo = "Libra", FechaConsulta = DateOnly.FromDateTime(DateTime.UtcNow) },
            new() { Signo = "Libra", FechaConsulta = DateOnly.FromDateTime(DateTime.UtcNow) },
            new() { Signo = "Aries", FechaConsulta = DateOnly.FromDateTime(DateTime.UtcNow) },
            new() { Signo = "Libra", FechaConsulta = DateOnly.FromDateTime(DateTime.UtcNow) },
            new() { Signo = "Aries", FechaConsulta = DateOnly.FromDateTime(DateTime.UtcNow) },
            new() { Signo = "Tauro", FechaConsulta = DateOnly.FromDateTime(DateTime.UtcNow) },
        };

            _historialRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(historial);

            // Act
            var resultado = (await _horoscopeService.GetEstadisticasAsync()).ToList();

            // Assert
            resultado.Should().HaveCount(3);
            resultado[0].Signo.Should().Be("Libra");
            resultado[0].CantidadConsultas.Should().Be(3);
            resultado[1].Signo.Should().Be("Aries");
            resultado[1].CantidadConsultas.Should().Be(2);
            resultado[2].Signo.Should().Be("Tauro");
            resultado[2].CantidadConsultas.Should().Be(1);
        }
    }
}
