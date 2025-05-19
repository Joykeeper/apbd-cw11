using System.ComponentModel.DataAnnotations;
using Task11.Models;

namespace Task11.DTOs;

public class PatientDto
{
    public required int IdPatient { get; set; }
    [MaxLength(100)] 
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
    
    public List<PrescriptionDto>? Prescriptions { get; set; }
}