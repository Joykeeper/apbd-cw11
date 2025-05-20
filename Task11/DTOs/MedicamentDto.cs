using System.ComponentModel.DataAnnotations;

namespace Task11.DTOs;

public class MedicamentDto
{
    public required int IdMedicament { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }
    public required int? Dose { get; set; }
    
    [MaxLength(100)]
    public string Description { get; set; }
}