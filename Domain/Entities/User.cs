using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }

        private readonly List<Contact> _contacts = new();
        public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();

        private readonly List<ChannelMember> _channels = new();
        public IReadOnlyCollection<ChannelMember> Channels => _channels.AsReadOnly();

        private User() { }

        public User(string username, string passwordHash)
        {
            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
        }
    }

}
