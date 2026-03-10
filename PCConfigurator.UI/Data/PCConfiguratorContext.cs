using Microsoft.EntityFrameworkCore;
using Курсовой_Конфигуратор_ПК.Models;

namespace Курсовой_Конфигуратор_ПК.Data;

public class PCConfiguratorContext : DbContext
{
    public PCConfiguratorContext()
    {
    }

    public PCConfiguratorContext(DbContextOptions<PCConfiguratorContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Configuration> Configurations { get; set; }
    public DbSet<ConfigurationRam> ConfigurationRams { get; set; }
    public DbSet<ConfigurationStorage> ConfigurationStorages { get; set; }
    public DbSet<Gpu> Gpus { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Motherboard> Motherboards { get; set; }
    public DbSet<Processor> Processors { get; set; }
    public DbSet<Ram> Rams { get; set; }
    public DbSet<Storage> Storages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Используется только если контекст создаётся напрямую (например, dotnet ef migrations)
        // В рабочем режиме строка подключения настраивается через DI в App.xaml.cs
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-62AA05E\\SQLEXPRESS;Database=PCConfigurator;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ───── Users ─────────────────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.Login).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Password).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("user").IsRequired();
        });

        // ───── Configurations ─────────────────────────────────────
        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.ConfigurationId);

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("datetime");
            entity.Property(e => e.Gpuid).HasColumnName("GPUId");

            entity.HasOne(d => d.Gpu).WithMany(p => p.Configurations)
                .HasForeignKey(d => d.Gpuid)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Motherboard).WithMany(p => p.Configurations)
                .HasForeignKey(d => d.MotherboardId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Processor).WithMany(p => p.Configurations)
                .HasForeignKey(d => d.ProcessorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.User).WithMany(p => p.Configurations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ───── ConfigurationRAM ───────────────────────────────────
        modelBuilder.Entity<ConfigurationRam>(entity =>
        {
            entity.HasKey(e => e.ConfigurationRamid);
            entity.ToTable("ConfigurationRAM");
            entity.Property(e => e.ConfigurationRamid).HasColumnName("ConfigurationRAMId");
            entity.Property(e => e.Ramid).HasColumnName("RAMId");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Configuration).WithMany(p => p.ConfigurationRams)
                .HasForeignKey(d => d.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Ram).WithMany(p => p.ConfigurationRams)
                .HasForeignKey(d => d.Ramid)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ───── ConfigurationStorage ───────────────────────────────
        modelBuilder.Entity<ConfigurationStorage>(entity =>
        {
            entity.HasKey(e => e.ConfigurationStorageId);
            entity.ToTable("ConfigurationStorage");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Configuration).WithMany(p => p.ConfigurationStorages)
                .HasForeignKey(d => d.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Storage).WithMany(p => p.ConfigurationStorages)
                .HasForeignKey(d => d.StorageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ───── GPUs ───────────────────────────────────────────────
        modelBuilder.Entity<Gpu>(entity =>
        {
            entity.HasKey(e => e.Gpuid);
            entity.ToTable("GPUs");
            entity.Property(e => e.Gpuid).HasColumnName("GPUId");
            entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MemoryType).HasMaxLength(20).IsRequired();

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Gpus)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ───── Manufacturers ──────────────────────────────────────
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Website).HasMaxLength(200);
        });

        // ───── Motherboards ───────────────────────────────────────
        modelBuilder.Entity<Motherboard>(entity =>
        {
            entity.HasKey(e => e.MotherboardId);
            entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Socket).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Chipset).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Ramtype).HasMaxLength(20).HasColumnName("RAMType").IsRequired();
            entity.Property(e => e.MaxRam).HasColumnName("MaxRAM");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Motherboards)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ───── Processors ─────────────────────────────────────────
        modelBuilder.Entity<Processor>(entity =>
        {
            entity.HasKey(e => e.ProcessorId);
            entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Socket).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Frequency).HasColumnType("decimal(4,2)");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Processors)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ───── RAM ────────────────────────────────────────────────
        modelBuilder.Entity<Ram>(entity =>
        {
            entity.HasKey(e => e.Ramid);
            entity.ToTable("RAM");
            entity.Property(e => e.Ramid).HasColumnName("RAMId");
            entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(20).IsRequired();

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Rams)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ───── Storage ────────────────────────────────────────────
        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.StorageId);
            entity.ToTable("Storage");
            entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Interface).HasMaxLength(50).IsRequired();

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Storages)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
