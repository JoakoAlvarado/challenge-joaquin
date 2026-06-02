using ChallengeApi.Application.DTOs.Horoscope;
using ChallengeApi.Application.Helpers;
using ChallengeApi.Application.Interfaces.Repositories;
using ChallengeApi.Application.Interfaces.Services;
using ChallengeApi.Domain.Entities;
using ChallengeApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Services
{
    public class HoroscopeService : IHoroscopeService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConsultaHistorialRepository _historialRepository;
        private readonly IExternalHoroscopeService _externalHoroscopeService;
        private readonly ILogger<HoroscopeService> _logger;

        public HoroscopeService(
            IUserRepository userRepository,
            IConsultaHistorialRepository historialRepository,
            IExternalHoroscopeService externalHoroscopeService,
            ILogger<HoroscopeService> logger)
        {
            _userRepository = userRepository;
            _historialRepository = historialRepository;
            _externalHoroscopeService = externalHoroscopeService;
            _logger = logger;
        }

        public async Task<HoroscopeResponseDto> GetHoroscopeAsync(int userId)
        {
            _logger.LogInformation("Consultando horóscopo para userId: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new UserNotFoundException(userId);

            var signo = ZodiacHelper.GetSigno(user.BirthDate);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var diasHastaCumple = ZodiacHelper.GetDiasHastaCumpleanos(user.BirthDate);

            var horoscopoTexto = await _externalHoroscopeService
                .GetHoroscopoTextoAsync(signo, today);

            var consulta = new ConsultaHistorial
            {
                UserId = user.Id,
                Signo = signo,
                FechaConsulta = today,
                CreatedAt = DateTime.UtcNow
            };
            //VER DE METER EN GUARDADO TAMBIEN LA CONSULTA (TEXTO) QUE RESPONDE EL API EXTERNA, PARA PODER MOSTRARLO EN EL HISTORIAL
            //ESTO PUEDE REQUERIR AJUSTAR TEST CASES

            await _historialRepository.AddAsync(consulta);

            _logger.LogInformation(
                "Horóscopo obtenido para usuario: {Username}, signo: {Signo}",
                user.Username, signo);

            return new HoroscopeResponseDto
            {
                Signo = signo,
                Horoscopo = horoscopoTexto,
                DiasHastaCumpleanos = diasHastaCumple,
                FechaConsulta = today
            };
        }

        public async Task<IEnumerable<ConsultaHistorialDto>> GetHistorialAsync(int userId)
        {
            _logger.LogInformation("Obteniendo historial para userId: {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new UserNotFoundException(userId);

            var historial = await _historialRepository.GetByUserIdAsync(userId);

            return historial.Select(h => new ConsultaHistorialDto
            {
                Signo = h.Signo,
                FechaConsulta = h.FechaConsulta,
                Username = user.Username
            });
        }

        public async Task<IEnumerable<SignoEstadisticaDto>> GetEstadisticasAsync()
        {
            _logger.LogInformation("Obteniendo estadísticas de signos");

            var todas = await _historialRepository.GetAllAsync();

            return todas
                .GroupBy(h => h.Signo)
                .Select(g => new SignoEstadisticaDto
                {
                    Signo = g.Key,
                    CantidadConsultas = g.Count()
                })
                .OrderByDescending(s => s.CantidadConsultas);
        }
    }
}
