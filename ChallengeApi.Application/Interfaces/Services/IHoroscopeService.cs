using ChallengeApi.Application.DTOs.Horoscope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Interfaces.Services
{
    public interface IHoroscopeService
    {
        Task<HoroscopeResponseDto> GetHoroscopeAsync(int userId);
        Task<IEnumerable<ConsultaHistorialDto>> GetHistorialAsync(int userId);
        Task<IEnumerable<SignoEstadisticaDto>> GetEstadisticasAsync();
    }
}
