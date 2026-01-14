using Application.Common.Interfaces.Contacts;
using Application.Common.Interfaces.Users;
using Application.DTOs.Contacts;
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
    public class ContactsService : IContactsService
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

        public async Task AddAsync(Guid ownerId, string userName)
        {
            var targetUser = await _usersService.GetByUsernameAsync(userName);
            var ownUsername = await _usersService.GetUsername(ownerId.ToString());

            var existing = await _appDbContext.Contacts
            .FirstOrDefaultAsync(c =>
                 (c.RequesterId == ownerId && c.AddresseeId == targetUser.UserId) ||
                 (c.RequesterId == targetUser.UserId && c.AddresseeId == ownerId));

            if (existing != null)
            {
                switch (existing.status)
                {
                    case Status.Accepted:
                        throw new InvalidOperationException("Users are already contacts.");

                    case Status.Pending:
                        throw new InvalidOperationException("A request already exists.");

                    case Status.Rejected:
                        throw new InvalidOperationException("The request has already been rejected.");
                }

            }
                var newContact = new Contact(ownerId, ownUsername, targetUser.UserId, targetUser.Username);

                try
                {
                    await _appDbContext.Contacts.AddAsync(newContact);
                    await _appDbContext.SaveChangesAsync();
                    _logger.LogInformation($"The new contact '{userName}' was added correctly");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"'ContactsService' | 'AddAsync'| Error creating the new contact: {ex.Message} ");
                    throw new Exception($"Error creating the contact: {ex.Message}");
                }
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
            try
            {

                var request = await _appDbContext.Contacts
                   .FirstOrDefaultAsync(c =>
                       c.RequesterId == requesterId &&
                       c.AddresseeId == approverId &&
                       c.status == Status.Pending);

                if (request is null)
                    throw new KeyNotFoundException("Request not found");

                request.Approve();
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"'ApproveAsync' | Error updating the status of the request: {ex.Message}");
            }
        }
        

        public async Task RejectAsync(Guid requesterId, Guid approverId)
        {
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
    }
}