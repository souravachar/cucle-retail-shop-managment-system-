using System.ComponentModel.DataAnnotations;

namespace CycleRetailShopAPI.Models
{
    public enum UserRole { Admin, Employee }
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ProfileImageUrl { get; set; }  // ✅ Store image URL

    }
}
