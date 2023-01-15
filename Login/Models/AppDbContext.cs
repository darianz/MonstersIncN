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
      

      
    }
}
