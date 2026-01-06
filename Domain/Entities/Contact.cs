using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Contact
    {
        public Guid OwnerUserId { get; private set;  }
        public Guid ContactUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Contact() { }

        public Contact(Guid ownerUserId, Guid contactUserId)
        {
            OwnerUserId = ownerUserId;
            ContactUserId = contactUserId;
            CreatedAt = DateTime.UtcNow;   
        }
    }
}
