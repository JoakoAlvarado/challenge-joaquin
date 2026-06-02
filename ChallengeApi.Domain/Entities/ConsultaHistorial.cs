using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Domain.Entities
{
    public class ConsultaHistorial
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Signo { get; set; } = string.Empty;
        public DateOnly FechaConsulta { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
