using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelMember> ChannelMembers { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChannelMember>()
                .HasKey(cm => new { cm.ChannelId, cm.UserId });
            modelBuilder.Entity<Contact>()
                .HasKey(cm => new { cm.RequesterId, cm.AddresseeId });

            base.OnModelCreating(modelBuilder);
        }
    }
  
}
