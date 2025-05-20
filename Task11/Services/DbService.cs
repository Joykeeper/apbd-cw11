using Microsoft.EntityFrameworkCore;
using Task11.Data;
using Task11.DTOs;
using Task11.Exceptions;
using Task11.Models;

namespace Task11.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;
    public DbService(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task AddPrescription(PrescriptionDto prescription)
    {
        if (prescription.Medicaments?.Count > 10)
            throw new MedicamentsOverflowException();

        if (prescription.DueDate < prescription.Date)
            throw new DueDateBeforeDateException();
        
        var doctor = await _context.Doctors.FindAsync(prescription.DoctorId);
        if (doctor == null)
            throw new NoDoctorException();

        Patient? patient;

        if (prescription.Patient?.IdPatient > 0)
        {
            patient = await _context.Patients.FindAsync(prescription.Patient.IdPatient);
        }
        else
        {
            patient = await _context.Patients.FirstOrDefaultAsync(p =>
                prescription.Patient != null &&
                p.FirstName == prescription.Patient.FirstName &&
                p.LastName == prescription.Patient.LastName &&
                p.Birthdate == prescription.Patient.Birthdate);
        }

        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = prescription.Patient.FirstName,
                LastName = prescription.Patient.LastName,
                Birthdate = prescription.Patient.Birthdate
            };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }
        
        var possibleDuplicates = await _context.Prescriptions
            .Include(p => p.PrescriptionMedicaments)
            .Where(p => p.IdPatient == patient.IdPatient &&
                        p.IdDoctor == doctor.IdDoctor &&
                        p.Date == prescription.Date &&
                        p.DueDate == prescription.DueDate)
            .ToListAsync();

        foreach (var existing in possibleDuplicates)
        {
            var existingMeds = existing.PrescriptionMedicaments
                .Select(pm => new { pm.IdMedicament, pm.Dose })
                .OrderBy(m => m.IdMedicament)
                .ToList();

            var newMeds = prescription.Medicaments
                .Select(pm => new { pm.IdMedicament, pm.Dose })
                .OrderBy(m => m.IdMedicament)
                .ToList();

            if (existingMeds.SequenceEqual(newMeds))
                throw new PrescriptionDuplicateException();
        }


        var medicamentIds = prescription.Medicaments.Select(m => m.IdMedicament).ToList();
        var existingIds = await _context.Medicaments
            .Where(m => medicamentIds.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();

        var missing = medicamentIds.Except(existingIds).ToList();
        if (missing.Any())
            throw new MedicamentNotExistsException($"Missing medicaments: {string.Join(", ", missing)}");
        
        
        var newPrescription = new Prescription
        {
            Date = prescription.Date,
            DueDate = prescription.DueDate,
            IdDoctor = doctor.IdDoctor,
            IdPatient = patient.IdPatient,
            PrescriptionMedicaments = prescription.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Description
            }).ToList()
        };

        await _context.Prescriptions.AddAsync(newPrescription);
        await _context.SaveChangesAsync();
    }

    
    public async Task<PatientDto> GetPatientFullInfo(int idPatient)
    {
        var patient = await _context.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Where(p => p.IdPatient == idPatient)
            .Select(p => new PatientDto()
            {
                IdPatient = p.IdPatient,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Birthdate = p.Birthdate,
                Prescriptions = p.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionDto
                    {
                        IdPrescription = pr.IdPrescription,
                        Date = pr.Date,
                        DueDate = pr.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor = pr.Doctor.IdDoctor,
                            FirstName = pr.Doctor.FirstName,
                            LastName = pr.Doctor.LastName,
                            Email = pr.Doctor.Email,
                        },
                        Medicaments = pr.PrescriptionMedicaments
                            .Select(pm => new MedicamentDto
                            {
                                IdMedicament = pm.Medicament.IdMedicament,
                                Name = pm.Medicament.Name,
                                Description = pm.Medicament.Description,
                                Dose = pm.Dose
                            }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync();
        
        if (patient == null)
            throw new NoPatientFoundException();

        return patient;
    }
}