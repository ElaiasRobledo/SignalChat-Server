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
}
