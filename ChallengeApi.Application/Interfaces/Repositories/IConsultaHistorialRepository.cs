using ChallengeApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Interfaces.Repositories
{
    public interface IConsultaHistorialRepository
    {
        Task AddAsync(ConsultaHistorial consulta);
        Task<IEnumerable<ConsultaHistorial>> GetByUserIdAsync(int userId);
        Task<IEnumerable<ConsultaHistorial>> GetAllAsync();
    }
}
