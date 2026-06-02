using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Domain.Exceptions
{
    public class InvalidCredentialsException : DomainException
    {
        public InvalidCredentialsException()
            : base("Las credenciales ingresadas son incorrectas.") { }
    }
}
