using Application.Common.Interfaces.ChannelMembers;
using Domain.Entities;
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

        public async Task<string> JoinToChannel(Guid userId, Guid channelId)
        {
            var newUser = new ChannelMember(channelId,userId,ChannelMember.ChannelRole.Member);


            _logger.LogInformation($"Adding user: {userId.ToString()} to channel: {channelId.ToString()}");

            await _appDbContext.ChannelMembers.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();
            return "You have been added correctly!";

        }
    }
}
