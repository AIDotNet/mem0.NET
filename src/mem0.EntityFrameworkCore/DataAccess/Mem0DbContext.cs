using mem0.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace mem0.NET.Service.DataAccess;

/// <summary>
///
/// </summary>
/// <param name="options"></param>
public class Mem0DbContext<TDbContext>(DbContextOptions<TDbContext> options)
    : DbContext(options) where TDbContext : DbContext
{
    public DbSet<History> Histories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigEntities(modelBuilder);
    }

    /// <summary>
    /// Mem0 数据库实体配置
    /// </summary>
    /// <param name="builder"></param>
    public static void ConfigEntities(ModelBuilder builder)
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