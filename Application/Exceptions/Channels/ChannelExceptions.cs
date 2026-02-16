using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions.Channels
{
    public sealed class ChannelNotFoundException : Exception
    {
        public ChannelNotFoundException() :
            base("Channel not found")
        { }
    }

    public sealed class ChannelIsPrivateException : Exception
    {
        public ChannelIsPrivateException() : base("The channel is private, you need authorization") { }
    }
    public sealed class MemberNotAuthorizedException : Exception 
    {
        public MemberNotAuthorizedException() : base("You are not authorized to do that") { }
    }
}
