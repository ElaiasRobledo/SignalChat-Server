using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Channel
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid OwnerId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<ChannelMember> _members = new();
        public IReadOnlyCollection<ChannelMember> Members => _members.AsReadOnly();

        private Channel() { }

        public Channel(string name, Guid ownerId)
        {
            Id = Guid.NewGuid();
            Name = name;
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
