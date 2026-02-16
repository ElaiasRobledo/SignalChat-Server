using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions.Users
{
    public sealed class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException()
            : base("User has already been registered") { }
    }
}
