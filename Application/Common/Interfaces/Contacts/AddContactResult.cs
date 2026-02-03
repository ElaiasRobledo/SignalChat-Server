using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Contacts
{
    public enum AddContactResult
    {
        Success,
        AlreadyContacts,
        PendingRequestExists,
        RequestRejected,
        TargetUserNotFound
    }

}
