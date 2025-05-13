using System.ComponentModel.DataAnnotations;

namespace YourProject.DTOs
{
    public class CustomerUpdateDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();
    }

    public class AddressDto
    {
        public int? AddressID { get; set; }  // Optional for updates
        [Required]
        public string FullAddress { get; set; } = string.Empty;
    }
}
