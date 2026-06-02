using ChallengeApi.Application.Interfaces.Repositories;
using ChallengeApi.Domain.Entities;
using ChallengeApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Infrastructure.Persistence.Repositories
{
    public class ConsultaHistorialRepository : IConsultaHistorialRepository
    {
        private readonly AppDbContext _context;

        public ConsultaHistorialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ConsultaHistorial consulta)
        {
            await _context.ConsultasHistorial.AddAsync(consulta);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ConsultaHistorial>> GetByUserIdAsync(int userId)
            => await _context.ConsultasHistorial
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.FechaConsulta)
                .ToListAsync();

        public async Task<IEnumerable<ConsultaHistorial>> GetAllAsync()
            => await _context.ConsultasHistorial.ToListAsync();
    }
}
