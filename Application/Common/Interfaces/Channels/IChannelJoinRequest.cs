using Application.DTOs.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Channels
{
    public interface IChannelJoinRequest
    {
        Task CreateJoinRequestAsync(Guid userId, Guid channelId, string reason);
        Task<IEnumerable<IncomingRequestsDto>> IncomingRequestsAsync(Guid channelId, Guid ownerId);
    }
}
