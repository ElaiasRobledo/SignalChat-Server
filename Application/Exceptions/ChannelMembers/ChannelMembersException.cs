using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions.ChannelMembers
{
   public sealed class UserIsAlreadyInTheChannelException : Exception
    {

        public UserIsAlreadyInTheChannelException() : base("The user is already a member of the channel") { }    

    }

    public sealed class SentRequestToJoinToChannelException : Exception
    {
        public SentRequestToJoinToChannelException() : base("The request has already been sent") { }
    }
    public sealed class OwnerSendJoinRequestException : Exception
    {
        public OwnerSendJoinRequestException() : base("You cannot send a request because you're thw owner") { }
    }
    public sealed class SendJoinRequestToPublicChannelException : Exception
    {
        public SendJoinRequestToPublicChannelException() : base("You cannot send a request because the channel is public") {}
    }
}
