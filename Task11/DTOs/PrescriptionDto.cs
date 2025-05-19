using System.ComponentModel.DataAnnotations;
using Task11.Models;

namespace Task11.DTOs;

public class PrescriptionDto
{
    public PatientDto? Patient { get; set; }
    public int DoctorId { get; set; }
    
        
    [MaxLength(10)]
    public List<Medicament>? Medicaments { get; set; }
    
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
}