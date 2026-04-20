using System.ComponentModel.DataAnnotations;

public class CreateStudentDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public string Class { get; set; }

    [Required]
    public string Section { get; set; }
}