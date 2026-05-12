using Microsoft.EntityFrameworkCore;
using UnadeskTest.Shared.Models;

namespace UnadeskTest.Shared.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<PdfDocument> PdfDocuments => Set<PdfDocument>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PdfDocument>(entity =>
            {
                entity.ToTable("pdf_documents");
                entity.HasKey(d => d.Id);

                entity.Property(d => d.OriginalFileName).HasMaxLength(260).IsRequired();
                entity.Property(d => d.StoredFileName).HasMaxLength(260).IsRequired();
                entity.Property(d => d.FilePath).HasMaxLength(1000).IsRequired();
                entity.Property(d => d.ContentType).HasMaxLength(100).IsRequired();
                entity.Property(d => d.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(d => d.ExtractedText).HasColumnType("text");
                entity.Property(d => d.ErrorMessage).HasMaxLength(2000);

                entity.HasIndex(d => d.Status);
                entity.HasIndex(d => d.CreatedAtUtc);
            });

            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.ToTable("outbox_messages");
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(m => m.ErrorMessage).HasMaxLength(2000);

                entity.HasIndex(m => m.Status);
                entity.HasIndex(m => m.CreatedAtUtc);
            });
        }
    }
}
