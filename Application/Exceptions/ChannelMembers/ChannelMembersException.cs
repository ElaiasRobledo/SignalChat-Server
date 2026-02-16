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
}
