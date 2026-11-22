using Microsoft.EntityFrameworkCore;
using Library_Management_System.Models;
using Library_Management_System.Models.DbModels;

namespace Library_Management_System.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<BookRequest> BookRequests { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<RentLog>  RentLogs { get; set; }
        public DbSet<BookInventory> BookInventories { get; set; }
        public DbSet<SalesRecord>  SalesRecords { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<LostRecord> LostRecords { get; set; }
    }
}