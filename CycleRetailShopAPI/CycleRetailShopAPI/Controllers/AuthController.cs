using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CycleRetailShopAPI.Data;
using CycleRetailShopAPI.Models;
using CycleRetailShopAPI.DTOs;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromForm] RegisterDTO model, IFormFile? file)
    {
        if (_context.Users.Any(u => u.Email == model.Email))
            return BadRequest(new { message = "Email already exists" });

        string? profileImageUrl = null;

        // ✅ If image is uploaded, upload it to ImgBB
        if (file != null && file.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();
            string base64Image = Convert.ToBase64String(fileBytes);

            string imgbbApiKey = "dfd91e08cf9d648eb35c41aaf73f45ca";  // 🔥 Replace with actual key
            string uploadUrl = $"https://api.imgbb.com/1/upload?key={imgbbApiKey}";

            using var httpClient = new HttpClient();
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(base64Image), "image");

            var response = await httpClient.PostAsync(uploadUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                profileImageUrl = result["data"]["url"]; // ✅ Get image URL
            }
        }

        // ✅ Create user object
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = Enum.Parse<UserRole>(model.Role, true),
            ProfileImageUrl = profileImageUrl  // ✅ Store image URL (or null if not uploaded)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User registered successfully!",
            user = new
            {
                user.UserID,
                user.Username,
                user.Email,
                user.Role,
                user.ProfileImageUrl // ✅ Return image URL if available
            }
        });
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO model)
    {
        try
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials." });

            var token = GenerateJwtToken(user);

            var userDto = new UserDTO
            {
                Id = user.UserID,
                Name = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                ProfileImageUrl = user.ProfileImageUrl
            };

            return Ok(new { Token = token, User = userDto });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
        }
    }


    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("id", user.UserID.ToString()),
            new Claim("role", user.Role.ToString()) // Ensure enum role is converted to string
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["JwtSettings:ExpireMinutes"])),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
