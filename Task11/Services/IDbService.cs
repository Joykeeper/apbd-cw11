using Task11.DTOs;
using Task11.Models;

namespace Task11.Services;

public interface IDbService
{
    Task<int> AddPrescription(PrescriptionDto prescription);
}