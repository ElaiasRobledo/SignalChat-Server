using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Contact
    {
        public Guid RequesterId { get; private set;  }
        public string RequesterUsername { get; private set; }
        public Guid AddresseeId { get; private set; }
        public string AddresseeUsername { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Status status { get; private set; }

        private Contact() { }

        public void Approve() => status = Status.Accepted;
        public void Reject() => status = Status.Rejected;
        public Contact(Guid requesterId, string reqUsername, 
            Guid addresseeId, string addrUsername)
        {
            RequesterId = requesterId;
            RequesterUsername = reqUsername;
            AddresseeId = addresseeId;
            AddresseeUsername = addrUsername;
            status = Status.Pending;
            CreatedAt = DateTime.UtcNow;   
        }
        public enum Status
        {
            Accepted,
            Pending,
            Rejected
        }
    }
}
