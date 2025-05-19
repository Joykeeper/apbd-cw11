using Microsoft.EntityFrameworkCore;
using Task11.Models;

namespace Task11.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> MedicamentPrescriptions { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the Course entity
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");
            entity.HasKey(e => e.IdPatient);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Birthdate)
                .IsRequired();
            entity.HasMany(e => e.Prescriptions)
                .WithOne(e => e.Patient)
                .HasForeignKey(e => e.IdPatient);
        });
        
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");
            entity.HasKey(e => e.IdDoctor);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Email)
                .IsRequired();
            entity.HasMany(e => e.Prescriptions)
                .WithOne(e => e.Doctor)
                .HasForeignKey(e => e.IdDoctor);
        });
        
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescription");
            entity.HasKey(e => e.IdPrescription);
            entity.Property(e => e.Date)
                .IsRequired();
            entity.Property(e => e.DueDate)
                .IsRequired();
            entity.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(e => e.Prescription)
                .HasForeignKey(e => e.IdPrescription);
        });
        
        modelBuilder.Entity<Medicament>(entity =>
        {
            entity.ToTable("Medicament");
            entity.HasKey(e => e.IdMedicament);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Type)
                .IsRequired();
            entity.HasMany(e => e.PrescriptionMedicaments)
                .WithOne(e => e.Medicament)
                .HasForeignKey(e => e.IdMedicament);
        });
        
        modelBuilder.Entity<PrescriptionMedicament>(entity =>
        {
            entity.ToTable("Prescription_Medicament");
            entity.HasKey(e => new { e.IdMedicament, e.IdPrescription });
            entity.Property(e => e.Dose)
                .IsRequired();
            entity.Property(e => e.Details)
                .IsRequired()
                .HasMaxLength(100);
        });

    }
}