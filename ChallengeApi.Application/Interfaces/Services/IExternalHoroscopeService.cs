using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Application.Interfaces.Services
{
    public interface IExternalHoroscopeService
    {
        Task<string> GetHoroscopoTextoAsync(string signo, DateOnly fecha);
    }
}
