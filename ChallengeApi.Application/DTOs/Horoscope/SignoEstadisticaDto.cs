using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.DTOs.Horoscope
{
    public class SignoEstadisticaDto
    {
        public string Signo { get; set; } = string.Empty;
        public int CantidadConsultas { get; set; }
    }
}
