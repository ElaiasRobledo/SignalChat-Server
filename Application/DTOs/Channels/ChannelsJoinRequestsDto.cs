using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Channels
{
    public record IncomingRequestsDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
    public record CreateJoinRequestDto
    {
        public string Reason { get; set; }

    }
}