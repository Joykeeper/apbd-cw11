using System.ComponentModel.DataAnnotations;

namespace Task11.DTOs;

public class MedicamentDto
{
    public required int MedicamentId { get; set; }
    public required int Dose { get; set; }
    
    [MaxLength(100)]
    public string Description { get; set; }
}