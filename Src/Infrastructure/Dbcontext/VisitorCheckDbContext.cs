using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Visitor_Check_Application.Models;

public partial class VisitorCheckDbContext : DbContext
{
    public VisitorCheckDbContext()
    {
    }

    public VisitorCheckDbContext(DbContextOptions<VisitorCheckDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<Hospital> Hospitals { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.DivisionId).HasName("PK__Division__B8A52DAF6015BAC9");
        });

        modelBuilder.Entity<Hospital>(entity =>
        {
            entity.HasKey(e => e.FacilityCode).HasName("PK__Hospital__5DA02D084AB2B824");

            entity.Property(e => e.FacilityCode).ValueGeneratedNever();
            entity.Property(e => e.MaxLimit).HasDefaultValue(0);

            entity.HasOne(d => d.Division).WithMany(p => p.Hospitals).HasConstraintName("FK__Hospital__Divisi__59FA5E80");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__C1A16F51D1C2CAB2");

            entity.Property(e => e.VisitorsInRoom).HasDefaultValue(0);
            entity.HasOne(p => p.Hospital)
                  .WithMany(h => h.Patients)
                  .HasForeignKey(p => p.FacilityCode)
                  .OnDelete(DeleteBehavior.Restrict)
                  .HasConstraintName("FK_Patients_Hospital");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
