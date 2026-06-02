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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<User?> GetByUsernameAsync(string username)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

        public async Task<bool> ExistsAsync(string email, string username)
            => await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower()
                            || u.Username.ToLower() == username.ToLower());

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
