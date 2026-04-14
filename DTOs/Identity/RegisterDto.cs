using System.ComponentModel.DataAnnotations;

namespace Practice.DTO;

public class RegisterDto
{
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = null!;
}