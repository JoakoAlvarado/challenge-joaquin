using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.DTOs.User
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string Signo { get; set; } = string.Empty;
        public int DiasHastaCumpleanos { get; set; }
    }
}
