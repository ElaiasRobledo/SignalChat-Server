using Application.Common.Interfaces.Contacts;
using Application.Common.Interfaces.Users;
using Application.DTOs.Contacts;
using Application.Exceptions;
using Application.Exceptions.Users;
using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Text;
using static Domain.Entities.Contact;

namespace Infrastructure.Services.Contacts
{
    internal class ContactsService : IContactsService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<ContactsService> _logger;
        private readonly IUserService _usersService;

        public ContactsService(AppDbContext appDbContext, ILogger<ContactsService> logger, IUserService usersService)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _usersService = usersService;
        }

        public async Task<AddContactResult> AddAsync(Guid ownerId, string userName)
        {
            var targetUser = await _usersService.GetByUsernameAsync(userName);
            var ownUsername = await _usersService.GetUsername(ownerId.ToString());

            var existing = await _appDbContext.Contacts
            .FirstOrDefaultAsync(c =>
                 (c.RequesterId == ownerId && c.AddresseeId == targetUser.UserId) ||
                 (c.RequesterId == targetUser.UserId && c.AddresseeId == ownerId));

            if (existing != null)
            {
                if (existing.status == Status.Pending)
                    return AddContactResult.PendingRequestExists;

                if (existing.status == Status.Accepted)
                    return AddContactResult.AlreadyContacts;

                _appDbContext.Contacts.Remove(existing);
            }

            if (targetUser.UserId == ownerId) throw new InvalidOperationException("You cant add yourself");

            var newContact = new Contact(ownerId, ownUsername, targetUser.UserId, targetUser.Username);

            try
            {
                await _appDbContext.Contacts.AddAsync(newContact);
                await _appDbContext.SaveChangesAsync();
                _logger.LogInformation($"The new contact '{userName}' was added correctly");
                return AddContactResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"'ContactsService' | 'AddAsync'| Error creating the new contact: {ex.Message} ");
                throw new Exception($"Error creating the contact: {ex.Message}");
            }
        }

        public async Task DeleteAsync(Guid ownerId, Guid contactId)
        {
            var contact = await _appDbContext.Contacts
            .FirstOrDefaultAsync(c =>
                 (c.RequesterId == ownerId && c.AddresseeId == contactId) ||
                 (c.RequesterId == contactId && c.AddresseeId == ownerId));

            if (contact is null) throw new UserNotFoundException();

            if (contact.status != Status.Accepted)
                throw new InvalidOperationException("Cannot delete non-accepted contact");

            _appDbContext.Contacts.Remove(contact);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResponseContactDto>> GetApprovedAsync(Guid userId)
        {
            var approvedContacts = await _appDbContext.Contacts
                .Where(c =>
                    c.status == Status.Accepted &&
                    (c.RequesterId == userId || c.AddresseeId == userId))
                .ToListAsync();

            var result = approvedContacts.Select(c =>
            {
                var isRequester = c.RequesterId == userId;

                return new ResponseContactDto
                {
                    UserId = isRequester ? c.AddresseeId : c.RequesterId,
                    Username = isRequester ? c.AddresseeUsername : c.RequesterUsername,
                    Status = c.status.ToString(),
                };
            });
            return result;
        }

        public async Task<IEnumerable<ResponseContactDto>> GetPendingAsync(Guid userId)
        {
            var pendingContacts = await _appDbContext.Contacts.Where
                 (c =>
                    (c.RequesterId == userId || c.AddresseeId == userId)
                    && c.status == Status.Pending)
                 .ToListAsync();

            var result = pendingContacts.Select(c =>
            {
                var isRequester = c.RequesterId == userId;

                return new ResponseContactDto
                {
                    UserId = isRequester ? c.AddresseeId : c.RequesterId,
                    Username = isRequester ? c.AddresseeUsername : c.RequesterUsername,
                    Status = c.status.ToString(),
                };
            });

            return result;
        }

        public async Task ApproveAsync(Guid requesterId, Guid approverId)
        {
            var request = await _appDbContext.Contacts
                .FirstOrDefaultAsync(c =>
                    c.RequesterId == requesterId &&
                    c.AddresseeId == approverId &&
                    c.status == Status.Pending);

            if (request is null)
                throw new InvalidOperationException("Pending request not found or invalid approver");

            request.Approve();
            await _appDbContext.SaveChangesAsync();
        }

        public async Task RejectAsync(Guid requesterId, Guid approverId)
        {
            if (requesterId == approverId)
                throw new InvalidOperationException("You cannot reject your own request");

            var request = await _appDbContext.Contacts
               .FirstOrDefaultAsync(c =>
                   c.RequesterId == requesterId &&
                   c.AddresseeId == approverId &&
                   c.status == Status.Pending);

            if (request is null)
                throw new KeyNotFoundException("Request not found");

            request.Reject();
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<ResponseContactDto>> IncomingPendingAsync(Guid userId)
        {
            return await _appDbContext.Contacts
                .Where(c =>
                    c.AddresseeId == userId &&
                    c.status == Status.Pending)
                .Select(c => new ResponseContactDto
                {
                    UserId = c.RequesterId,
                    Username = c.RequesterUsername,
                    Status = c.status.ToString()
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<ResponseContactDto>> OutgoingPendingAsync(Guid userId)
        {
            return await _appDbContext.Contacts
                .Where(c =>
                    c.RequesterId == userId &&
                    c.status == Status.Pending)
                .Select(c => new ResponseContactDto
                {
                    UserId = c.AddresseeId,
                    Username = c.AddresseeUsername,
                    Status = c.status.ToString()
                })
                .ToListAsync();
        }


    }
}