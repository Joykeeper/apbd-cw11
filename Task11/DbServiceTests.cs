using Microsoft.EntityFrameworkCore;
using Task11.Data;
using Task11.DTOs;
using Task11.Exceptions;
using Task11.Models;
using Task11.Services;
using Xunit;

public class DbServiceTests
{
    private async Task<DatabaseContext> GetDbContextAsync()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        var context = new DatabaseContext(options);

        // Seed test data
        var doctor = new Doctor { IdDoctor = 1, FirstName = "Gregory", LastName = "House", Email = "house@hospital.com" };
        context.Doctors.Add(doctor);
        context.Medicaments.AddRange(
            new Medicament { IdMedicament = 1, Name = "Paracetamol", Description = "Painkiller", Type = "Tablet" },
            new Medicament { IdMedicament = 2, Name = "Ibuprofen", Description = "Anti-inflammatory", Type = "Capsule" }
        );
        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task AddPrescription_Success()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        var dto = new PrescriptionDto
        {
            DoctorId = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            Patient = new PatientDto
            {
                IdPatient = 12,
                FirstName = "John",
                LastName = "Doe",
                Birthdate = new DateTime(1990, 1, 1)
            },
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto { IdMedicament = 1, Dose = 100, Description = "Take 1 daily", Name = "Ibuprofen" }
            }
        };

        await service.AddPrescription(dto);
    }

    [Fact]
    public async Task AddPrescription_TooManyMedicaments_Throws()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        var dto = new PrescriptionDto
        {
            DoctorId = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            Patient = new PatientDto { IdPatient = 12, FirstName = "Jane", LastName = "Smith", Birthdate = new DateTime(1985, 5, 5) },
            Medicaments = new List<MedicamentDto>()
        };

        for (int i = 0; i < 11; i++)
            dto.Medicaments.Add(new MedicamentDto { IdMedicament = 1, Dose = 100, Description = "Test", Name = "Ibuprofen" });

        await Assert.ThrowsAsync<MedicamentsOverflowException>(() => service.AddPrescription(dto));
    }

    [Fact]
    public async Task AddPrescription_DueDateBeforeDate_Throws()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        var dto = new PrescriptionDto
        {
            DoctorId = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(-1),
            Patient = new PatientDto { IdPatient = 12, FirstName = "John", LastName = "Doe", Birthdate = new DateTime(1990, 1, 1) },
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto { IdMedicament = 1, Dose = 100, Description = "Take 1 daily", Name = "Ibuprofen" }
            }
        };

        await Assert.ThrowsAsync<DueDateBeforeDateException>(() => service.AddPrescription(dto));
    }

    [Fact]
    public async Task AddPrescription_NoDoctor_Throws()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        var dto = new PrescriptionDto
        {
            DoctorId = 999, // Invalid
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(5),
            Patient = new PatientDto { IdPatient = 12, FirstName = "Ana", LastName = "Taylor", Birthdate = new DateTime(2000, 1, 1) },
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto { IdMedicament = 1, Dose = 100, Description = "Take 1 daily", Name = "Ibuprofen" }
            }
        };

        await Assert.ThrowsAsync<NoDoctorException>(() => service.AddPrescription(dto));
    }

    [Fact]
    public async Task AddPrescription_MissingMedicament_Throws()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        var dto = new PrescriptionDto
        {
            DoctorId = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(3),
            Patient = new PatientDto { IdPatient = 12, FirstName = "Lucy", LastName = "Gray", Birthdate = new DateTime(1995, 7, 15) },
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto { IdMedicament = 999, Dose = 100, Description = "Unknown", Name = "Ghostpill" }
            }
        };

        await Assert.ThrowsAsync<MedicamentNotExistsException>(() => service.AddPrescription(dto));
    }

    [Fact]
    public async Task AddPrescription_Duplicate_Throws()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        var patient = new Patient { IdPatient = 12, FirstName = "Matt", LastName = "Brown", Birthdate = new DateTime(1980, 1, 1) };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var existing = new Prescription
        {
            IdDoctor = 1,
            IdPatient = patient.IdPatient,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(5),
            PrescriptionMedicaments = new List<PrescriptionMedicament>
            {
                new PrescriptionMedicament { IdMedicament = 1, Dose = 100, Details = "Daily" }
            }
        };
        context.Prescriptions.Add(existing);
        await context.SaveChangesAsync();

        var dto = new PrescriptionDto
        {
            DoctorId = 1,
            Date = existing.Date,
            DueDate = existing.DueDate,
            Patient = new PatientDto
            {
                IdPatient = patient.IdPatient
            },
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto { IdMedicament = 1, Dose = 100, Description = "Daily", Name = "Ibuprofen" }
            }
        };

        await Assert.ThrowsAsync<PrescriptionDuplicateException>(() => service.AddPrescription(dto));
    }

    [Fact]
    public async Task GetPatientFullInfo_Success()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        var patient = new Patient { FirstName = "Emma", LastName = "Stone", Birthdate = new DateTime(1992, 3, 1) };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var prescription = new Prescription
        {
            IdPatient = patient.IdPatient,
            IdDoctor = 1,
            Date = DateTime.Today,
            DueDate = DateTime.Today.AddDays(3),
            PrescriptionMedicaments = new List<PrescriptionMedicament>
            {
                new PrescriptionMedicament { IdMedicament = 1, Dose = 50, Details = "Morning" }
            }
        };

        context.Prescriptions.Add(prescription);
        await context.SaveChangesAsync();

        var result = await service.GetPatientFullInfo(patient.IdPatient);
        Assert.Equal("Emma", result.FirstName);
        Assert.Single(result.Prescriptions);
    }

    [Fact]
    public async Task GetPatientFullInfo_PatientNotFound_Throws()
    {
        var context = await GetDbContextAsync();
        var service = new DbService(context);

        await Assert.ThrowsAsync<NoPatientFoundException>(() => service.GetPatientFullInfo(999));
    }
}
