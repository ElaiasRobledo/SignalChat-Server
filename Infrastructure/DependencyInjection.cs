using Application.Common.Interfaces.ChannelMembers;
using Application.Common.Interfaces.Channels;
using Application.Common.Interfaces.Contacts;
using Application.Common.Interfaces.Security;
using Application.Common.Interfaces.Users;
using Infrastructure.Cryptography;
using Infrastructure.Services.ChannelMembers;
using Infrastructure.Services.Channels;
using Infrastructure.Services.Contacts;
using Infrastructure.Services.Security;
using Infrastructure.Services.Users;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthUser, AuthUserService>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<IChannelMember, ChannelMemberService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactsService, ContactsService>();
            services.AddScoped<IChannelJoinRequest, ChannelJoinRequestService>();
            services.AddScoped<IChannelMembership, ChannelMembershipService>();
            services.AddScoped<IChannelModeration, ChannelModerationService>();
            return services;
        }
    }
}
