using Application.Common.Interfaces.Security;
using Application.Common.Interfaces.Users;
using Domain.Entities;
using Infrastructure.Cryptography;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Application.DTOs.Users.AuthUserDto;

namespace Infrastructure.Services.Users
{
    internal class AuthUserService : IAuthUser
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJWTService _jwtService;
        private readonly AppDbContext _appDbContext;
        public AuthUserService(IPasswordHasher passwordHasher, AppDbContext appDbContext, IJWTService jwtService)
        {
            _passwordHasher = passwordHasher;
            _appDbContext = appDbContext;
            _jwtService = jwtService;
        }

        public async Task AddUserAsync(RegisterUserDto payload)
        {

            var email = await _appDbContext.
                Users.
                FirstOrDefaultAsync
                (c => c.Username == payload.username);

            if (email is not null) { throw new Exception("Usuario ya registrado"); }

            var passwordHash = _passwordHasher.Hash(payload.password);
            var user = new User(payload.username, passwordHash);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();

        }
        public async Task<string> VerifyUserAsync(LoginUserDto payload)
        {
            var user = await _appDbContext.Users.SingleOrDefaultAsync
                (u => u.Username == payload.username);
            if (user is null) 
                throw new UnauthorizedAccessException("Invalid credentials");


            var password = _passwordHasher.Verify(user.PasswordHash, payload.password);
            if (!password)
                throw new UnauthorizedAccessException("Invalid credentials");

            return _jwtService.GenerateToken(user);


        }
    }
}
