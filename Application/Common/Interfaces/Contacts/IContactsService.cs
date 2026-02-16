using Application.DTOs.Contacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Contacts
{
    public interface IContactsService
    {
        Task<AddContactResult> AddAsync(Guid ownerId, string userName);
        Task<IEnumerable<ResponseContactDto>> GetApprovedAsync(Guid approverId);
     //   Task<IEnumerable<ResponseContactDto>> GetPendingAsync(Guid approverId);
        Task ApproveAsync(Guid requesterId, Guid approverId);
        Task RejectAsync(Guid requesterId, Guid approverId);
        Task DeleteAsync(Guid ownerId,Guid contactId);
        Task<IEnumerable<ResponseContactDto>> OutgoingPendingAsync(Guid userId);
        Task<IEnumerable<ResponseContactDto>> IncomingPendingAsync(Guid userId);
    }
}
