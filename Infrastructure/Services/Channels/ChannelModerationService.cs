using Application.Common.Interfaces.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Channels
{
    internal class ChannelModerationService : IChannelModeration
    {
        private readonly AppDbContext _dbContext;
        private readonly IChannelService _channelService;

        public ChannelModerationService(AppDbContext dbContext, IChannelService channelService)
        {
            _dbContext = dbContext;
            _channelService = channelService;
        }


    }
}
