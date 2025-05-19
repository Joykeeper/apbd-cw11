using System.ComponentModel.DataAnnotations;
using Task11.Models;

namespace Task11.DTOs;

public class PrescriptionDto
{
    public int? IdPrescription { get; set; }
    public PatientDto? Patient { get; set; }
    public int? DoctorId { get; set; }
    public DoctorDto? Doctor { get; set; }
    
    public DateTime? Date { get; set; }
    public DateTime? DueDate { get; set; }
        
    [MaxLength(10)]
    public List<MedicamentDto>? Medicaments { get; set; }
    
    
}