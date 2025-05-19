using System.ComponentModel.DataAnnotations;

namespace Task11.DTOs;

public class PatientDto
{
    public required int IdPatient { get; set; }
    [MaxLength(100)] 
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
    public DateTime Birthdate { get; set; }
}