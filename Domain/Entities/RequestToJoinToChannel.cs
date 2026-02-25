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
        public string Reason { get; private set; }

        public User Requester { get; private set; }

        private RequestToJoinToChannel() { }

        public void Accept() => status = Status.Accepted;
        public void Reject() => status = Status.Rejected;
        public RequestToJoinToChannel(Guid requesterId, Guid channelId, string reason)
        {
            RequesterId = requesterId;
            ChannelId = channelId;
            SentAt = DateTime.UtcNow;
            status = Status.Pending;
            Reason = reason;
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
