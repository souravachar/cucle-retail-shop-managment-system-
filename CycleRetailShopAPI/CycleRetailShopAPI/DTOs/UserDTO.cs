public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string? ProfileImageUrl { get; set; }  // ✅ Added field for profile image
}
