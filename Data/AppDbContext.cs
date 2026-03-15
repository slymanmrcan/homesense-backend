using Microsoft.EntityFrameworkCore;
using Models;

namespace HomeSense.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<ReadingBatch> ReadingBatches => Set<ReadingBatch>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
    public DbSet<DeviceThreshold> DeviceThresholds => Set<DeviceThreshold>();
    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Device>(e =>
        {
            e.ToTable("Devices");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.MacAddress).HasMaxLength(17).IsRequired();
            e.Property(x => x.ApiKeyHash).HasMaxLength(256).IsRequired();

            e.HasIndex(x => x.MacAddress).IsUnique();

            e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<ReadingBatch>(e =>
        {
            e.ToTable("ReadingBatches");
            e.HasKey(x => x.Id);

            // İdempotency — aynı BatchKey iki kez insert edilemez
            e.HasIndex(x => x.BatchKey).IsUnique();

            e.Property(x => x.TriggerSensorType).HasMaxLength(50);
            e.Property(x => x.TriggerRule).HasMaxLength(100);
            e.Property(x => x.TriggerValue).HasPrecision(8, 3);

            e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

            e.HasOne(x => x.Device)
             .WithMany(x => x.ReadingBatches)
             .HasForeignKey(x => x.DeviceId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SensorReading>(e =>
        {
            e.ToTable("SensorReadings");
            e.HasKey(x => x.Id);

            e.Property(x => x.SensorType).HasMaxLength(50).IsRequired();
            e.Property(x => x.Unit).HasMaxLength(20).IsRequired();
            e.Property(x => x.Value).HasPrecision(10, 4);
            e.Property(x => x.RawPayload).HasMaxLength(500);

            // Sorgu paterni: cihazın belirli sensörünün zaman serisi
            e.HasIndex(x => new { x.BatchId, x.SensorType });

            e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

            e.HasOne(x => x.Batch)
             .WithMany(x => x.Readings)
             .HasForeignKey(x => x.BatchId)
             .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<DeviceThreshold>(e =>
        {
            e.ToTable("DeviceThresholds");
            e.HasKey(x => x.Id);

            e.Property(x => x.SensorType).HasMaxLength(50).IsRequired();
            e.Property(x => x.MinValue).HasPrecision(10, 4);
            e.Property(x => x.MaxValue).HasPrecision(10, 4);

            // Bir cihazda aynı sensör tipi için tek aktif eşik olsun
            e.HasIndex(x => new { x.DeviceId, x.SensorType }).IsUnique();

            e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

            e.HasOne(x => x.Device)
            .WithMany(x => x.Thresholds)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Alert>(e =>
        {
            e.ToTable("Alerts");
            e.HasKey(x => x.Id);

            e.Property(x => x.SensorType).HasMaxLength(50).IsRequired();
            e.Property(x => x.Rule).HasMaxLength(100).IsRequired();
            e.Property(x => x.TriggerValue).HasPrecision(10, 4);
            e.Property(x => x.ThresholdValue).HasPrecision(10, 4);

            e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

            e.HasOne(x => x.Device)
            .WithMany(x => x.Alerts)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.NoAction);  // Cascade çakışmasını önlemek için

            e.HasOne(x => x.Batch)
            .WithMany()
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.NoAction);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAtUtc = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}