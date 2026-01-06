using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Users
{
    public class AuthUserDto
    {
        public record RegisterUserDto
            (
                [Required] string username,
                [Required, MinLength(6)] string password
            );

        public record LoginUserDto
         (
             [Required] string username,
             [Required, MinLength(6)] string password
         );

    }
}
