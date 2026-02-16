using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Channels.Tags
{
    public class ChannelTag
    {
        public Guid ChannelId { get; private set; }
        public Channel Channel { get; private set; }

        public Guid TagId { get; private set; }
        public Tag Tag { get; private set; }

        private ChannelTag() { }

        public ChannelTag(Channel channel, Tag tag)
        {
            Channel = channel;
            ChannelId = channel.Id;

            Tag = tag;
            TagId = tag.Id;
        }
    }
}
