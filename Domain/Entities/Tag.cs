using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Channels.Tags
{
    public class Tag
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string NormalizedName { get; private set; }

        private readonly List<ChannelTag> _channels = new();
        public IReadOnlyCollection<ChannelTag> Channels => _channels.AsReadOnly();
        private Tag() { }
        public Tag(string  name)
        {
            Id = Guid.NewGuid();
            Name = name; 
            NormalizedName = name.ToLowerInvariant();
        }
    }
}
