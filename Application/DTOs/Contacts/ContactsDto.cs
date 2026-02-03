using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Contacts
{
    public class ContactsDto
    {

    }
    public record AddContactDto
        (
            [Required]
            string username
        );

    public record ResponseContactDto
    {
        public string Username { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
    }

}
