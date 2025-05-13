using System.ComponentModel.DataAnnotations;

public class UpdateProfileDTO
{
    [Required]
    public string Username { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Role { get; set; }
}
