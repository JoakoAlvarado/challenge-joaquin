using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Domain.Exceptions
{
    public class UserNotFoundException : DomainException
    {
        public UserNotFoundException(int id)
            : base($"No se encontró el usuario con id: {id}") { }
    }
}
