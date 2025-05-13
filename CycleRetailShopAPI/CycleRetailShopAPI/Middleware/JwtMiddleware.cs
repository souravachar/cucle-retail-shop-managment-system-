using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CycleRetailShopAPI.Data;
using Microsoft.EntityFrameworkCore;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;

    public JwtMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config;
    }

    public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
            AttachUserToContext(context, dbContext, token);

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, ApplicationDbContext dbContext, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["JwtSettings:Audience"],
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var userId = int.Parse(principal.Claims.First(x => x.Type == "id").Value);

            var user = dbContext.Users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                context.Items["User"] = user;
            }
        }
        catch
        {
            // Do nothing if JWT validation fails
        }
    }
}
