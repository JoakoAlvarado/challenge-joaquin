using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.DTOs.User
{
    public class UpdateProfileRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public DateOnly BirthDate { get; set; }
    }
}
