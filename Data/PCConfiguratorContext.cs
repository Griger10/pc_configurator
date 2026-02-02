using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Курсовой_Конфигуратор_ПК.Models;

namespace Курсовой_Конфигуратор_ПК.Data;

public partial class PCConfiguratorContext : DbContext
{
    public PCConfiguratorContext()
    {
    }

    public PCConfiguratorContext(DbContextOptions<PCConfiguratorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Configuration> Configurations { get; set; }

    public virtual DbSet<ConfigurationRam> ConfigurationRams { get; set; }

    public virtual DbSet<ConfigurationStorage> ConfigurationStorages { get; set; }

    public virtual DbSet<Gpu> Gpus { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Motherboard> Motherboards { get; set; }

    public virtual DbSet<Processor> Processors { get; set; }

    public virtual DbSet<Ram> Rams { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-62AA05E\\SQLEXPRESS;Database=PCConfigurator;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.ConfigurationId).HasName("PK__Configur__95AA53BBC1DF77FB");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Gpuid).HasColumnName("GPUId");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Gpu).WithMany(p => p.Configurations)
                .HasForeignKey(d => d.Gpuid)
                .HasConstraintName("FK_Configurations_GPUs");

            entity.HasOne(d => d.Motherboard).WithMany(p => p.Configurations)
                .HasForeignKey(d => d.MotherboardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Configurations_Motherboards");

            entity.HasOne(d => d.Processor).WithMany(p => p.Configurations)
                .HasForeignKey(d => d.ProcessorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Configurations_Processors");
        });

        modelBuilder.Entity<ConfigurationRam>(entity =>
        {
            entity.HasKey(e => e.ConfigurationRamid).HasName("PK__Configur__C1AA8E06E24ECE24");

            entity.ToTable("ConfigurationRAM");

            entity.Property(e => e.ConfigurationRamid).HasColumnName("ConfigurationRAMId");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.Ramid).HasColumnName("RAMId");

            entity.HasOne(d => d.Configuration).WithMany(p => p.ConfigurationRams)
                .HasForeignKey(d => d.ConfigurationId)
                .HasConstraintName("FK_ConfigurationRAM_Configurations");

            entity.HasOne(d => d.Ram).WithMany(p => p.ConfigurationRams)
                .HasForeignKey(d => d.Ramid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfigurationRAM_RAM");
        });

        modelBuilder.Entity<ConfigurationStorage>(entity =>
        {
            entity.HasKey(e => e.ConfigurationStorageId).HasName("PK__Configur__E297CF33BA554159");

            entity.ToTable("ConfigurationStorage");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Configuration).WithMany(p => p.ConfigurationStorages)
                .HasForeignKey(d => d.ConfigurationId)
                .HasConstraintName("FK_ConfigurationStorage_Configurations");

            entity.HasOne(d => d.Storage).WithMany(p => p.ConfigurationStorages)
                .HasForeignKey(d => d.StorageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfigurationStorage_Storage");
        });

        modelBuilder.Entity<Gpu>(entity =>
        {
            entity.HasKey(e => e.Gpuid).HasName("PK__GPUs__8A41A969AF4E098C");

            entity.ToTable("GPUs");

            entity.Property(e => e.Gpuid).HasColumnName("GPUId");
            entity.Property(e => e.MemoryType).HasMaxLength(20);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Gpus)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_GPUs_Manufacturers");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PK__Manufact__357E5CC1D289F1CF");

            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Website).HasMaxLength(200);
        });

        modelBuilder.Entity<Motherboard>(entity =>
        {
            entity.HasKey(e => e.MotherboardId).HasName("PK__Motherbo__08FE22C61AE1EFFC");

            entity.Property(e => e.Chipset).HasMaxLength(50);
            entity.Property(e => e.MaxRam).HasColumnName("MaxRAM");
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Ramtype)
                .HasMaxLength(20)
                .HasColumnName("RAMType");
            entity.Property(e => e.Socket).HasMaxLength(50);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Motherboards)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_Motherboards_Manufacturers");
        });

        modelBuilder.Entity<Processor>(entity =>
        {
            entity.HasKey(e => e.ProcessorId).HasName("PK__Processo__CE8FE1894447582C");

            entity.Property(e => e.Frequency).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Socket).HasMaxLength(50);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Processors)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_Processors_Manufacturers");
        });

        modelBuilder.Entity<Ram>(entity =>
        {
            entity.HasKey(e => e.Ramid).HasName("PK__RAM__0CB2050340871A54");

            entity.ToTable("RAM");

            entity.Property(e => e.Ramid).HasColumnName("RAMId");
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Rams)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_RAM_Manufacturers");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.StorageId).HasName("PK__Storage__8A247E576251192E");

            entity.ToTable("Storage");

            entity.Property(e => e.Interface).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Storages)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_Storage_Manufacturers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
