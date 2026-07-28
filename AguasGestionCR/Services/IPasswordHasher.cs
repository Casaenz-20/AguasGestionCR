using System;
using System.Collections.Generic;
using System.Text;

namespace AguasGestionCR.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string Password, string hashedPassword);
    }
}
