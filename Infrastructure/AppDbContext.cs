using Domain.Entities;
using Infrastructure.Channels.Tags;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ChannelTag> ChannelTags { get; set; }
        public DbSet<RequestToJoinToChannel> RequestToJoinToChannels { get; set; }
        public DbSet<ChannelMember> ChannelMembers { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChannelMember>()
            .HasKey(cm => new { cm.ChannelId, cm.UserId });

            modelBuilder.Entity<ChannelMember>()
                .HasOne(cm => cm.Member)
                .WithMany(u => u.Channels)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChannelMember>()
                .HasOne(cm => cm.Channel)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ChannelId);



            modelBuilder.Entity<RequestToJoinToChannel>()
                .HasKey(req => new { req.RequesterId, req.ChannelId });

            modelBuilder.Entity<RequestToJoinToChannel>()
                .HasOne(r => r.Requester)
                .WithMany()
                .HasForeignKey(r => r.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasKey(cm => new { cm.RequesterId, cm.AddresseeId });
            modelBuilder.Entity<ChannelTag>()
                .HasKey(ct => new { ct.ChannelId, ct.TagId });

            modelBuilder.Entity<ChannelTag>()
                .HasOne(ct => ct.Channel)
                .WithMany(c => c.Tags)
                .HasForeignKey(ct => ct.ChannelId);

            modelBuilder.Entity<ChannelTag>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.Channels)
                .HasForeignKey(ct => ct.TagId);

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.NormalizedName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}