using Infrastructure.Channels.Tags;
using System;
using System.Collections.Generic;
using System.Text;
using static Domain.Entities.ChannelMember;

namespace Domain.Entities
{
    public class Channel
    {
        public Guid Id { get; private set; }
        public int PublicId { get; private set; }

        public string Name { get; private set; }
        public string NormalizedName { get; private set; }

        public bool IsPublicName { get; private set; }
        public bool IsPublic { get; private set; }
        public bool IsVisible { get; private set; }
        public string Description { get; private set; }
        public Guid OwnerId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<ChannelMember> _members = new();
        public IReadOnlyCollection<ChannelMember> Members => _members.AsReadOnly();
        private readonly List<ChannelTag> _tags = new();
        public IReadOnlyCollection<ChannelTag> Tags => _tags.AsReadOnly();

        private Channel()
        { }

        public Channel(string name, string description,
            Guid ownerId, int publicId,
            bool isPublic, bool isPublicName,
            bool isVisible)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Channel name is required");

            Id = Guid.NewGuid();
            PublicId = publicId;
            Name = name;
            Description = description;
            NormalizedName = Normalize(name);
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;

            IsPublic = isPublic;
            IsPublicName = isPublicName;
            IsVisible = isVisible;
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToLowerInvariant();
        }

        public void Rename(string newName)
        {
            Name = newName;
            NormalizedName = Normalize(newName);
        }

        public void Update(string name, string description,
            bool isPublic,bool isPublicName,
            bool isVisible)
        {
            Rename(name);
            Description = description;
            IsPublic = isPublic;
            IsPublicName = isPublicName;
            IsVisible = isVisible;
        }
        
        public void ReplaceTags(IEnumerable<Tag> tags)
        {
            _tags.Clear();

            foreach (var tag in tags)
                AddTag(tag);
        }

        public void AddMember(Guid userId, ChannelMember.ChannelRole role)
        {
            if (_members.Any(m => m.UserId == userId)) return;
            _members.Add(new ChannelMember(Id, userId, role));
        }

        public void AddTag(Tag tag)
        {
            if (_tags.Any(t => t.TagId == tag.Id)) return;
            _tags.Add(new ChannelTag(this, tag));
        }

        public void RemoveTag(Guid tagId)
        {
            var tag = _tags.FirstOrDefault(t => t.TagId == tagId);
            if (tag != null)
                _tags.Remove(tag);
        }

    }
}