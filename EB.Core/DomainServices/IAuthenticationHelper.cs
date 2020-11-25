using EB.Core.Entities.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.DomainServices
{
    public interface IAuthenticationHelper
    {
        public byte[] GenerateHash(string password, byte[] salt);
        public byte[] GenerateSalt();
        public void ValidateLogin(User userToValidate, LoginInputModel inputModel);
        public string GenerateJWTToken(User user);
    }
}
