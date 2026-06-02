using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.DTOs.Horoscope
{
    public class ConsultaHistorialDto
    {
        public string Signo { get; set; } = string.Empty;
        public DateOnly FechaConsulta { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
