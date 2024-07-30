using mem0.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace mem0.NET.Service.DataAccess;

public class Mem0DbContext(DbContextOptions<Mem0DbContext> options) : DbContext(options)
{
    public DbSet<History> Histories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigEntities(modelBuilder);
    }

    private void ConfigEntities(ModelBuilder builder)
    {
        builder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.MemoryId).HasMaxLength(50);

            entity.Property(e => e.PrevValue).HasMaxLength(200);

            entity.Property(e => e.NewValue).HasMaxLength(200);

            entity.Property(e => e.Event).HasMaxLength(50);

            entity.HasIndex(e => e.MemoryId);

            entity.HasIndex(e => e.Event);

            entity.HasIndex(e => e.DateTime);

            entity.HasIndex(e => e.IsDeleted);
        });
    }
}