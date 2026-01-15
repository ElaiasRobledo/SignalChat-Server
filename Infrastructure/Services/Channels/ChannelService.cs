using Application.Common.Interfaces.Channels;
using Application.DTOs.Channels;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Infrastructure.Services.Channels
{
    internal class ChannelService : IChannelService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ChannelService> _logger;
        public ChannelService(AppDbContext db, ILogger<ChannelService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ChannelDto> CreateChannelAsync(ChannelCreateDto dto)
        {

            var entity = new Channel(dto.Name, dto.OwnerId);

            _db.Channels.Add(entity);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Channel created {ChannelId} by {OwnerId}", entity.Id, entity.OwnerId);

            return new ChannelDto
            {
                Id = entity.Id,
                Name = entity.Name,
                OwnerId = entity.OwnerId,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<ChannelDto> GetChannelAsync(Guid id)
        {
            var entity = await _db.Channels
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                _logger.LogWarning("Channel not found {ChannelId}", id);
                return null;
            }

            return new ChannelDto
            {
                Id = entity.Id,
                Name = entity.Name,
                OwnerId = entity.OwnerId,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<IEnumerable<ChannelDto>> GetAllChannelsAsync()
        {
            var list = await _db.Channels
                .Select(c => new ChannelDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    OwnerId = c.OwnerId,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Channels retrieved: {Count}", list.Count);

            return list;
        }

        public async Task<bool> UpdateChannelAsync(Guid id, ChannelUpdateDto dto)
        {
            var entity = await _db.Channels.FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                _logger.LogWarning("Update failed. Channel not found {ChannelId}", id);
                return false;
            }

            var nameField = typeof(Channel)
                .GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            nameField.SetValue(entity, dto.Name);

            await _db.SaveChangesAsync();

            _logger.LogInformation("Channel updated {ChannelId}", id);
            return true;
        }
        public async Task<string> GetChannelByName(string name)
        {
            var channel = await _db.Channels.FirstOrDefaultAsync
                (c => c.Name == name);

            return channel is null ? "Channel not found" : channel.Id.ToString();


        }
        public async Task<bool> DeleteChannelAsync(Guid id)
        {
            var entity = await _db.Channels.FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                _logger.LogWarning("Delete failed. Channel not found {ChannelId}", id);
                return false;
            }

            _db.Channels.Remove(entity);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Channel deleted {ChannelId}", id);

            return true;
        }
    }

}
