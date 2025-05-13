using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _context.Users.Select(u => new { u.UserID, u.Username, u.Email, u.Role, u.CreatedAt,
            u.ProfileImageUrl}).ToList();
        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = _context.Users.Where(u => u.UserID == id)
                                 .Select(u => new { u.UserID, u.Username, u.Email, u.Role, u.CreatedAt, u.ProfileImageUrl })
                                 .FirstOrDefault();
        if (user == null) return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("add")]
    public IActionResult AddUser([FromBody] RegisterDTO model)
    {
        if (_context.Users.Any(u => u.Email == model.Email))
            return BadRequest(new { message = "Email already exists" });

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = Enum.Parse<UserRole>(model.Role, true) // ✅ Convert string to enum
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "User added successfully!" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update-role/{id}")]
    public IActionResult UpdateUserRole(int id, [FromBody] string newRole)
    {
        var user = _context.Users.Find(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        user.Role = Enum.Parse<UserRole>(newRole, true); // ✅ Convert string to enum
        _context.SaveChanges();

        return Ok(new { message = "User role updated successfully!" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        _context.Users.Remove(user);
        _context.SaveChanges();
        return Ok(new { message = "User deleted successfully!" });
    }


    //[Authorize]
    [HttpPut("update-profile/{userId}")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromForm] UpdateProfileDTO model, IFormFile? file)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        user.Username = model.Username;
        user.Email = model.Email;

        if (!string.IsNullOrEmpty(model.Role))
        {
            user.Role = Enum.Parse<UserRole>(model.Role, true);
        }

        if (file != null && file.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();
            string base64Image = Convert.ToBase64String(fileBytes);

            string imgbbApiKey = "dfd91e08cf9d648eb35c41aaf73f45ca";
            string uploadUrl = $"https://api.imgbb.com/1/upload?key={imgbbApiKey}";

            using var httpClient = new HttpClient();
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(base64Image), "image");

            var response = await httpClient.PostAsync(uploadUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(500, new { message = "Failed to upload image to ImgBB." });
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            user.ProfileImageUrl = result["data"]["url"];
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Profile updated successfully",
            user = new
            {
                user.UserID,
                user.Username,
                user.Email,
                user.ProfileImageUrl
            }
        });
    }




}
