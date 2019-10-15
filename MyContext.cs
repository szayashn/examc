using Microsoft.EntityFrameworkCore;
 
namespace OneProject.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get;set;}
        public DbSet<Association> Associations {get;set;}
        public DbSet<Auction> Auctions {get;set;}
    }
}
