using Task11.DTOs;
using Task11.Models;

namespace Task11.Services;

public interface IDbService
{
    Task AddPrescription(PrescriptionDto prescription);
    Task<PatientDto> GetPatientFullInfo(int patientId);
}