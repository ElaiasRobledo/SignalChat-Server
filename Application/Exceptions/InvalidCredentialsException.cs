using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    public sealed class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid credentials") { }
    }
    public sealed class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            : base("User not found") { }
    }
}
