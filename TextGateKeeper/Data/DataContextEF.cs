using Microsoft.EntityFrameworkCore;
using TextGateKeeper.Models;

namespace TextGateKeeper.Data {
    public class DataContextEF : DbContext  {
        
        private IConfiguration _config;
        
        public DataContextEF(IConfiguration config) {
            _config = config;
        }

        public DbSet<TextMessage> textMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TextMessage>()
                .ToTable("TextMessage", "dbo")
                .HasKey(t => t.Id);
        }
        
    }
}