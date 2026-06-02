using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.DTOs.Horoscope
{
    public class HoroscopeResponseDto
    {
        public string Signo { get; set; } = string.Empty;
        public string Horoscopo { get; set; } = string.Empty;
        public int DiasHastaCumpleanos { get; set; }
        public DateOnly FechaConsulta { get; set; }
    }
}
