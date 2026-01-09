using Application.Common.Interfaces.ChannelMembers;
using Application.DTOs.ChannelMembers;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.ChannelMembers
{
    public class ChannelMemberService : IChannelMember
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


            if (userInGroup)
            {
                throw new InvalidOperationException(
                    $"User '{userId}' is already a member of channel '{channelId}'."
                );
            }

            var newUser = new ChannelMember(channelId,userId,ChannelMember.ChannelRole.Member);
            
            _logger.LogInformation($"Adding user: {userId.ToString()} to channel: {channelId.ToString()}");

            await _appDbContext.ChannelMembers.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();

        }

        public async Task<IEnumerable<GetChannelsMembersDto>> GetChannelsForUserAsync(Guid userId)
        {

            var entities = await _appDbContext.ChannelMembers.Where(c => c.UserId == userId).ToListAsync();

            if (!entities.Any())
                throw new InvalidOperationException("The user is not registered in any group");

            return entities.Adapt<IEnumerable<GetChannelsMembersDto>>();
        }
    }
}
