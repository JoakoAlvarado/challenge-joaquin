using ChallengeApi.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(int userId);
        Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileRequestDto dto);
    }
}
