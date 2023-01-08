


using Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;

namespace Management
{
    public class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Door> Door { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasOne(u => u.Role)
        //        .WithMany(r => r.Users)
        //        .HasForeignKey(u => u.RoleId);
        //}
    }
}
