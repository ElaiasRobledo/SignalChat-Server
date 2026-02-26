using Application.DTOs.ChannelMembers;
using Application.DTOs.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.ChannelMembers
{
    public interface IChannelMember
    {
        Task JoinToChannel(Guid userId, Guid channelId);
        Task<IEnumerable<GetChannelsMembersDto>> GetChannelsForUserAsync(Guid userId);
        Task<IEnumerable<MembersOfAChannelDto>> GetMembersAsync(Guid channelId);

    }
}
