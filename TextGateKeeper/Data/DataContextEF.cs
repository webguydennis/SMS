using Microsoft.EntityFrameworkCore;
using TextGateKeeper.Models;

namespace TextGateKeeper.Data {
    public class DataContextEF : DbContext  {
        
        private IConfiguration _config;
        
        public DataContextEF(IConfiguration config) {
            _config = config;
        }

        public DbSet<TextMessage>? textMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionStrings = _config.GetSection("ConnectionStrings:DefaultConnection").Value;
            Console.WriteLine("Dennis Test");
            Console.WriteLine(connectionStrings?.ToLower().IndexOf("testdatabase") > 0);
            if (!optionsBuilder.IsConfigured) {
                if (connectionStrings?.ToLower().IndexOf("testdatabase") > 0) {
                    optionsBuilder
                        .UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                            optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                        )
                        .UseInMemoryDatabase("TestDatabase");
                } else {
                    optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"),
                        optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                    );
                }
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