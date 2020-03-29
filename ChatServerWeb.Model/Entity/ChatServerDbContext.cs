using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServerWeb.Model.Entity
{
   

    public class ChatServerDbContext : DbContext
    {
        public ChatServerDbContext(DbContextOptions<ChatServerDbContext> options) : base(options)
        {
        }

        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatApplication> ChatApplications { get; set; }

        public DbSet<ChatApplicationUser> ChatApplicationUsers { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatSettings> ChatSettings { get; set; }


    }
}
