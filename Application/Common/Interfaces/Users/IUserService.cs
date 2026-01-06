using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Users
{
    public interface IUserService
    {
        Task<string> GetUsername(string id);
        Task<User> GetByIdAsync(string id);
    }
}
