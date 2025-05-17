using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interface;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using Ecommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace AuthenticationApi.Infrastructure.Repositories;

public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
{
    public async Task<AppUser> GetUserByEmail(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user is null ? null! : user!;
    }

    public async Task<GetUserDTO> GetUser(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        return user is null
            ? null!
            : new GetUserDTO(
                user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!
            );
    }

    public async Task<Response> Login(LoginDTO loginDTO)
    {
        var getUser = await GetUserByEmail(loginDTO.Email);
        if (getUser is null)
        {
            return new Response(false, "invalid credentials");
        }
        bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
        if (!verifyPassword)
        {
            return new Response(false, "invalid credentials");
        }
        string token = GenerateToken(getUser);
        return new Response(true, token);
    }

    private string GenerateToken(AppUser user)
    {
        var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
        var sercurityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(sercurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name!),
            new(ClaimTypes.Email, user.Email!),
        };

        if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
        {
            claims.Add(new(ClaimTypes.Role, user.Role!));
        }

        var token = new JwtSecurityToken(
            issuer: config["Authentication:Issuer"],
            audience: config["Authentication:Audience"],
            claims: claims,
            expires: null,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Response> Register(AppUserDTO appUserDTO)
    {
        var getUser = await GetUserByEmail(appUserDTO.Email);
        if (getUser is not null)
        {
            return new Response(false, $"you can not use this email for registration");
        }

        var result = context.Users.Add(
            new AppUser()
            {
                Name = appUserDTO.Name,
                Email = appUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Address = appUserDTO.Address,
                Role = appUserDTO.Role,
            }
        );
        await context.SaveChangesAsync();
        return result.Entity.Id > 0
            ? new Response(true, "User registered successfully")
            : new Response(false, "invalid data provided");
    }
}
