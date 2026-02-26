using Application.Common.Interfaces.ChannelMembers;
using Application.DTOs.ChannelMembers;
using Application.DTOs.Channels;
using Application.Exceptions.ChannelMembers;
using Application.Exceptions.Channels;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.ChannelMembers
{
    internal class ChannelMemberService : IChannelMember
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ChannelMemberService> _logger;

        public ChannelMemberService(AppDbContext appDbContext, ILogger<ChannelMemberService> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task JoinToChannel(Guid userId, Guid channelId)
        {
            var userInGroup = await _appDbContext.ChannelMembers.AnyAsync
                (u => u.UserId == userId && u.ChannelId == channelId);

            var channel = await _appDbContext.Channels.FirstOrDefaultAsync
                (c => c.Id == channelId);
            if (channel is null) throw new ChannelNotFoundException();

            if(!channel.IsPublic) throw new ChannelIsPrivateException();

            if (userInGroup || channel.OwnerId == userId) throw new UserIsAlreadyInTheChannelException();

            var newUser = new ChannelMember(channelId,userId,ChannelMember.ChannelRole.Member);
            
            _logger.LogInformation($"Adding user: {userId.ToString()} to channel: {channelId.ToString()}");

            await _appDbContext.ChannelMembers.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();

        }
    
        public async Task<IEnumerable<GetChannelsMembersDto>> GetChannelsForUserAsync(Guid userId)
        {

            var entities = await _appDbContext.ChannelMembers.Where(c => c.UserId == userId).ToListAsync();
            return entities.Adapt<IEnumerable<GetChannelsMembersDto>>();
        }

        public async Task<IEnumerable<MembersOfAChannelDto>> GetMembersAsync(Guid channelId)
        {
            //Validar y considerar el estado de un canal, si es privado nadie puede ver los miembros.

            var list = await _appDbContext.ChannelMembers.Where(c => c.ChannelId == channelId)
                .Select
                (c => new MembersOfAChannelDto
                {
                    Id = c.UserId.ToString(),
                    Role = c.Role.ToString(),
                    Username = c.Member.Username

                }).ToListAsync();

            return list;

        }
    }
}
