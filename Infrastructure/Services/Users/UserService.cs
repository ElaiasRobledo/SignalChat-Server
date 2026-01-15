using Application.Common.Interfaces.Users;
using Application.DTOs.Contacts;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Users
{
    internal class UserService : IUserService
    {
        private readonly AppDbContext _appDbContext;

        public UserService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<string> GetUsername(string id)
        {
            var result = await _appDbContext.Users.FirstOrDefaultAsync
                (u => u.Id.ToString() == id);
            if(result is null) { throw new KeyNotFoundException("User not found"); }
            return result.Username;
        }
        public async Task<User> GetByIdAsync(string id)
        {
            var result = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
            if (result is null) { throw new KeyNotFoundException("User not found"); }

            return result;
        }
        public async Task<ResponseContactDto> GetByUsernameAsync(string userName)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == userName);
            if (user is null) { throw new KeyNotFoundException("User not found"); }
            var result = new ResponseContactDto { Username = userName, UserId = user.Id };
            return result;
        
        }
    }
}
