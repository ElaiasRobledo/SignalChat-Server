using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.ChannelMembers
{
    public interface IChannelMember
    {
        Task<string> JoinToChannel(Guid userId, Guid channelId);
    }
}
