using System;
using System.Collections.Generic;
using System.Text;
using static Application.DTOs.Users.AuthUserDto;

namespace Application.Common.Interfaces.Users
{
    public interface IAuthUser
    {
        Task AddUserAsync(RegisterUserDto payload);
        Task<string> VerifyUserAsync(LoginUserDto payload);
    }
}
