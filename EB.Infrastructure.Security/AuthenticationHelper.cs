using EB.Core.DomainServices;
using EB.Core.Entities.Security;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EB.Infrastructure.Security
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        #region Dependency Injection
        private byte[] SecretBytes;

        public AuthenticationHelper(byte[] secretBytes)
        {
            SecretBytes = secretBytes;
        }
        #endregion

        public byte[] GenerateHash(string password, byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(salt))
            {
                return hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public byte[] GenerateSalt()
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                return hmac.Key;
            }
        }

        public void ValidateLogin(User userToValidate, LoginInputModel inputModel)
        {
            byte[] hashedPassword = GenerateHash(inputModel.Password, userToValidate.Salt);
            byte[] storedPassword = userToValidate.Password;

            for (int i = 0; i < storedPassword.Length; i++)
            {
                if (storedPassword[i] != hashedPassword[i])
                {
                    throw new UnauthorizedAccessException("Entered password is incorrect");
                }
            }
        }

        public string GenerateJWTToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(SecretBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserRole),
                new Claim("userID", user.ID +""),
            };

            var token = new JwtSecurityToken(
               new JwtHeader(credentials),
               new JwtPayload(null, // issuer - not needed (ValidateIssuer = false)
                              null, // audience - not needed (ValidateAudience = false)
                              claims,
                              DateTime.Now,               // notBefore
                              DateTime.Now.AddDays(60)));  // expires

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
