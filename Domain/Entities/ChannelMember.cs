using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class ChannelMember
    {
        public Guid ChannelId { get; private set; }
        public Guid UserId { get; private set; }
        public ChannelRole Role { get; private set; }
        public DateTime JoinedAt { get; private set; }

        private ChannelMember() { }

        public ChannelMember(Guid channelId, Guid userId, ChannelRole role)
        {
            ChannelId = channelId;
            UserId = userId;
            Role = role;
            JoinedAt = DateTime.UtcNow;
        }
        public enum ChannelRole
        {
            Owner,
            Admin,
            Member
        }
    
    }

}
