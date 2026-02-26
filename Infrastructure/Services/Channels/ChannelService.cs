using Application.Common.Interfaces.Channels;
using Application.Common.Interfaces.Users;
using Application.DTOs.Channels;
using Application.Exceptions.Channels;
using Domain.Entities;
using Infrastructure.Channels.Tags;
using Mapster;
using MapsterMapper;
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

        public async Task<ChannelDto> CreateChannelAsync(ChannelCreateDto dto, Guid ownerId)
        {
            var publicId = await GeneratePublicIdAsync();
            var entity = new Channel(dto.Name, dto.Description,
                ownerId, publicId,
                dto.IsPublic,
                dto.IsPublicName,
                dto.IsVisible);

            entity.AddMember(ownerId, ChannelMember.ChannelRole.Owner);
            if (dto.Tags?.Any() == true)
            {
                var tags = await ResolveTagsAsync(dto.Tags);

                foreach (var tag in tags)
                {
                    entity.AddTag(tag);
                }
            }
            _db.Channels.Add(entity);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Channel created {ChannelId} by {OwnerId}", entity.Id, entity.OwnerId);

            return entity.Adapt<ChannelDto>();
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

            return entity.Adapt<ChannelDto>();
        }

        public async Task<IEnumerable<ChannelDto>> GetAllChannelsAsync()
        {
            var list = await _db.Channels
                .Where(c => c.IsVisible)
                .Select(c => new ChannelDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    IsPublic = c.IsPublic,
                    PublicId = c.PublicId.ToString()
                })
                .ToListAsync();

            _logger.LogInformation("Channels retrieved: {Count}", list.Count);

            return list;
        }
      
       
        public async Task<bool> UpdateChannelAsync(Guid id, ChannelUpdateDto dto,
            Guid ownerId)
        {
            var entity = await _db.Channels
                .Include(c => c.Tags)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
                return false;

            if (entity.OwnerId != ownerId) throw new MemberNotAuthorizedException();

            entity.Update(dto.Name, dto.Description,
                dto.IsPublic,
                dto.IsPublicName, dto.IsVisible);

            if (dto.Tags != null)
            {
                var tags = await ResolveTagsAsync(dto.Tags);
                entity.ReplaceTags(tags);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ChannelDto>> GetChannelsForUserAsync(Guid userId)
        {
            return await _db.ChannelMembers
                .Where(m => m.UserId == userId)
                .Join(_db.Channels,
                    m => m.ChannelId,
                    c => c.Id,
                    (m, c) => c)
                .Select(c => c.Adapt<ChannelDto>())
                .ToListAsync();
        }

        public async Task<IEnumerable<ChannelDto>> SearchChannelsByTags(IEnumerable<string> tags)
        {
            var normalized = tags
                .Select(t => t.ToLowerInvariant())
                .ToList();

            return await _db.Channels
                .Where(c =>
                    c.Tags.Any(ct =>
                        normalized.Contains(ct.Tag.NormalizedName)))
                .Select(c => c.Adapt<ChannelDto>())
                .ToListAsync();
        }

        public async Task<IEnumerable<ChannelDto?>> SearchByNameAsync(string channelName)
        {
            var channel = await _db.Channels
                .Where(c => c.Name.StartsWith(channelName)
                && c.IsVisible && c.IsPublicName)
                .ToListAsync();
            return channel.Adapt<IEnumerable<ChannelDto>>();
        }

        public async Task<ChannelDto?> SearchByPublicIdAsync(int publicId)
        {
            var entity = await _db.Channels
                .Where(c => c.PublicId.ToString()
                .StartsWith(publicId.ToString())
                && c.IsVisible && !c.IsPublicName)
                .ToListAsync();

            return entity == null ? null : entity.Adapt<ChannelDto>();
        }

        public async Task<bool> DeleteChannelAsync(Guid id, Guid ownerId)
        {
            var entity = await _db.Channels.FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                _logger.LogWarning("Delete failed. Channel not found {ChannelId}", id);
                return false;
            }

            if (entity.OwnerId != ownerId) throw new MemberNotAuthorizedException();

            _db.Channels.Remove(entity);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Channel deleted {ChannelId}", id);

            return true;
        }

        private async Task<List<Tag>> ResolveTagsAsync(IEnumerable<string> tagNames)
        {
            var normalized = tagNames
                .Select(t => t.ToLowerInvariant())
                .Distinct()
                .ToList();

            var existing = await _db.Tags
                .Where(t => normalized.Contains(t.NormalizedName))
                .ToListAsync();

            var existingNames = existing
                .Select(t => t.NormalizedName)
                .ToHashSet();

            var newTags = normalized
                .Where(n => !existingNames.Contains(n))
                .Select(n => new Tag(n))
                .ToList();

            if (newTags.Any())
                _db.Tags.AddRange(newTags);

            return existing.Concat(newTags).ToList();
        }

        //TODO
        //Create a middleware for handling public Id generation.
        private async Task<int> GeneratePublicIdAsync()
        {
            var random = new Random();
            int number;

            do
            {
                number = random.Next(100000, 999999);
            }
            while (await _db.Channels.AnyAsync(c => c.PublicId == number));

            return number;
        }
    }
}