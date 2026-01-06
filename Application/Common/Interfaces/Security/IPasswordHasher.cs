using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string hash, string password);
    }
}
