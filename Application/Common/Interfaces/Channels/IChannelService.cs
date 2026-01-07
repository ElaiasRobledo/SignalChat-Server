using Application.DTOs.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Interfaces.Channels
{
    public interface IChannelService
    {
        Task<ChannelDto> CreateChannelAsync(ChannelCreateDto dto);
        Task<ChannelDto> GetChannelAsync(Guid id);
        Task<bool> DeleteChannelAsync(Guid id);
        Task<IEnumerable<ChannelDto>> GetAllChannelsAsync();
        Task<string> GetChannelByName(string name);
        Task<bool> UpdateChannelAsync(Guid id, ChannelUpdateDto dto);
    }
}
