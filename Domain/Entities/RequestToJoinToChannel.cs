using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class RequestToJoinToChannel
    {
        public Guid RequesterId { get; private set; }
        public Guid ChannelId { get; private set; }
        public DateTime SentAt { get; private set; }
        public Status status { get; private set; }

        private RequestToJoinToChannel() { }
        public RequestToJoinToChannel(Guid requesterId, Guid channelId)
        {
            RequesterId = requesterId;
            ChannelId = channelId;
            SentAt = DateTime.UtcNow;
            status = Status.Pending;
        }

        public enum Status
        {
            Accepted,
            Pending,
            Rejected,
            Banned
        }
    }
}
