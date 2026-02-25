using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Channels
{
    public record ChannelCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPublicName { get; set; }
        public bool IsVisible { get; set; }
    }
    public record JoinToChannelDto
    {
        public string ChannelName { get; set; }
    }
    public record ChannelUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPublicName { get; set; }
        public bool IsVisible { get; set; }
    }

    public record ChannelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PublicId { get; set; }
        public bool IsPublic { get; set; }

        public DateTime CreatedAt { get; set; }
    }
    public record MembersOfAChannelDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Role {  get; set; }
    }
}
