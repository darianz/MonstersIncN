using Login.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Login
{
    public class AppDbContext : DbContext
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Loginm> Logins { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>()
        //        .HasOne(u => u.Role)
        //        .WithMany(r => r.Users)
        //        .HasForeignKey(u => u.RoleId);
        //}
    }
}
