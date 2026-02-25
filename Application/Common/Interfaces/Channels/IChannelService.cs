using Application.DTOs.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Channels
{
    public interface IChannelService
    {
        Task<ChannelDto> CreateChannelAsync(ChannelCreateDto dto, Guid ownerId);

        Task<ChannelDto> GetChannelAsync(Guid id);

        Task<IEnumerable<ChannelDto>> GetAllChannelsAsync();

        Task<IEnumerable<ChannelDto>> GetChannelsForUserAsync(Guid userId);

        Task<bool> UpdateChannelAsync(Guid id, ChannelUpdateDto dto, Guid ownerId);

        Task<ChannelDto?> SearchByPublicIdAsync(int publicId);

        Task<IEnumerable<ChannelDto>> SearchChannelsByTags(IEnumerable<string> tags);

        Task<bool> DeleteChannelAsync(Guid id, Guid ownerId);

        Task<IEnumerable<MembersOfAChannelDto>> GetMembersAsync(Guid channelId);
    }
}