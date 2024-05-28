using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backendEmployeModule.Models;

public partial class BackendEmployesModuleContext : DbContext
{
    public BackendEmployesModuleContext()
    {
    }

    public BackendEmployesModuleContext(DbContextOptions<BackendEmployesModuleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employe> Employes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("date")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employe>(entity =>
        {
            entity.ToTable("Employe");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContractDate)
                .HasColumnType("date")
                .HasColumnName("contractDate");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("date")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IdDept).HasColumnName("idDept");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Pay)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("pay");

            entity.HasOne(d => d.IdDeptNavigation).WithMany(p => p.Employes)
                .HasForeignKey(d => d.IdDept)
                .HasConstraintName("FK_Employe_Department");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
