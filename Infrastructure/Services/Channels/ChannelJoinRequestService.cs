using Application.Common.Interfaces.Channels;
using Application.DTOs.Channels;
using Application.Exceptions.ChannelMembers;
using Application.Exceptions.Channels;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Channels
{
    internal class ChannelJoinRequestService : IChannelJoinRequest
    {
        private readonly AppDbContext _appDbContext;
        private readonly IChannelService _channelService;
        private readonly ILogger<ChannelJoinRequestService> _logger;
        public ChannelJoinRequestService(AppDbContext appDbContext, IChannelService channelService,
            ILogger<ChannelJoinRequestService> logger)
        {
            _appDbContext = appDbContext;
            _channelService = channelService;
            _logger = logger;

        }

        public async Task<IEnumerable<IncomingRequestsDto>> IncomingRequestsAsync(Guid channelId, Guid ownerId)
        {
            var channel = await _appDbContext.Channels.FirstOrDefaultAsync
                (c => c.Id == channelId);
            if (channel is null) throw new ChannelNotFoundException();

            if (channel.OwnerId != ownerId) throw new MemberNotAuthorizedException();

            var result = await _appDbContext.RequestToJoinToChannels
                .Where(r => r.ChannelId == channelId)
                .Select(r => new IncomingRequestsDto
                {
                    Username = r.Requester.Username,
                    Reason = r.Reason,
                    Id = r.RequesterId.ToString(),
                    Date = r.SentAt
                }).ToListAsync();

            return result;
        }
        //Change the name to CreateJoinRequest
        public async Task CreateJoinRequestAsync(Guid userId, Guid channelId, string reason)
        {
            var channel = await _appDbContext.Channels.FirstOrDefaultAsync
                (c => c.Id == channelId);
            if (channel is null) throw new ChannelNotFoundException();

            var requestSent = await _appDbContext.RequestToJoinToChannels.AnyAsync
              (u => u.RequesterId == userId && u.ChannelId == channelId);

            if (channel.OwnerId == userId) throw new OwnerSendJoinRequestException();

            if (requestSent) throw new SentRequestToJoinToChannelException();

            var newRequest = new RequestToJoinToChannel(userId, channelId, reason);
            _logger.LogInformation($"Creating request for: {userId.ToString()} to channel: {channelId.ToString()}");

            await _appDbContext.RequestToJoinToChannels.AddAsync(newRequest);
            await _appDbContext.SaveChangesAsync();

        }
        public async Task ApproveAsync(Guid channelId,Guid requesterId, Guid ownerId)
        {
            var request = await _appDbContext.RequestToJoinToChannels
                .FirstOrDefaultAsync(c => c.RequesterId == requesterId &&
                c.ChannelId == channelId &&
                c.status == RequestToJoinToChannel.Status.Pending);

            if(request is null) throw new KeyNotFoundException();

            request.Accept();
            await _appDbContext.SaveChangesAsync ();


        }
    }
}
