using OrderStock.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStock.Entities.Entities;

public class User
{
    public int Id { get; set; }
    public string NameSurname { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Roles { get; set; } = EnumStringRoles.User;
}
public class RegisterDto
{
    public string NameSurname { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
public class TokenDto
{
    public string Token { get; set; }
}
public class UpdateUserDTO
{
    public string UserEmail { get; set; }
    public string NameSurname { get; set; }
    public string Password { get; set; }
    public string NewEmail { get; set; }
    public string Roles { get; set; }
}