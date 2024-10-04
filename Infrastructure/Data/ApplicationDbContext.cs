using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Image> Images { get; set; }
        public DbSet<ImageVariation> ImageVariations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships, constraints, etc.
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FilePath).IsRequired();
                entity.Property(e => e.UploadDate).IsRequired();
            });
        }

        public class BloggingContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer("Server=tcp:imagedbserver.database.windows.net,1433;Initial Catalog=imageDB;Persist Security Info=False;User ID=adminantonio;Password=testsqlserverA1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                // optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));
                // optionsBuilder.UseSqlServer("Server=ANTONIO\\SQLEXPRESS; Initial Catalog=ImageDb; Encrypt=False;Integrated Security=true; pooling=false;");

                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
    }
}
