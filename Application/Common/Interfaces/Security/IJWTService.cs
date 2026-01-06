using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Security
{
    public interface IJWTService
    {
        public string GenerateToken(User user);
    }
}
