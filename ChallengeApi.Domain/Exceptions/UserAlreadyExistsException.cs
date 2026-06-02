using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Domain.Exceptions
{
    public class UserAlreadyExistsException : DomainException
    {
        public UserAlreadyExistsException(string identifier)
            : base($"Ya existe un usuario con el identificador: {identifier}") { }
    }
}
