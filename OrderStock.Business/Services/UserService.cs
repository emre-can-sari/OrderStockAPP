using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderStock.DataAccess;
using OrderStock.Entities.Entities;
using OrderStock.Entities.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrderStock.Business.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;


    public UserService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public User register(RegisterDto registerDto)
    {
        if (registerDto == null) throw new ArgumentNullException(nameof(registerDto));
        if (_context.Users.Any(x => x.Email == registerDto.Email))
        {
            return null;
        }

        User user = new User
        {
            NameSurname = registerDto.NameSurname,
            Password = HashPassword(registerDto.Password),
            Email = registerDto.Email
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public TokenDto login(LoginDto loginDto)
    {
        if (loginDto == null)
            throw new ArgumentNullException(nameof(loginDto));

        var user = _context.Users.SingleOrDefault(u => u.Email == loginDto.Email);

        if (user == null || !VerifyPassword(loginDto.Password, user.Password))
        {
            return null;
        }


        TokenDto token = new TokenDto { Token = GenerateToken(user) };
        return token;
    }

    public List<User> GetAllUser()
    {
        var users = _context.Users.ToList();

        return users;
    }
    public User GetUserById(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return null;

        return user;
    }


    public User UpdateUser(UpdateUserDTO updateUserDTO)
    {
        if (updateUserDTO == null) throw new ArgumentNullException(nameof(updateUserDTO));

        var user = _context.Users.SingleOrDefault(x => x.Email == updateUserDTO.UserEmail);
        if (user == null) throw new InvalidOperationException("User not found.");


        if (!string.IsNullOrEmpty(updateUserDTO.NewEmail) && updateUserDTO.NewEmail != user.Email)
        {
            var existingUser = _context.Users.SingleOrDefault(x => x.Email == updateUserDTO.NewEmail);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email address is already in use.");
            }
            user.Email = updateUserDTO.NewEmail;
        }

        user.NameSurname = updateUserDTO.NameSurname;
        user.Password = HashPassword(updateUserDTO.Password);
        user.Roles = updateUserDTO.Roles;

        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }

    public void DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            user.Roles = EnumStringRoles.DeletedUser;
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }


    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    private string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.NameSurname),
            new Claim(ClaimTypes.Role, user.Roles)
        }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        try
        {
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Token generation failed.", ex);
        }
    }


}
