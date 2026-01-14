using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Utils
{
    public interface ICurrentUserService
    {
       Guid UserId { get; }
    }
}
