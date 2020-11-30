using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EB.Core.ApplicationServices;
using EB.Core.Entities.Security;
using Microsoft.AspNetCore.Mvc;

namespace EB.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        #region Dependency Injection
        private readonly IUserService UserService;

        public LoginController(IUserService userService)
        {
            this.UserService = userService;
        }
        #endregion

        #region Login
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginInputModel model)
        {
            try
            {
                User foundUser = UserService.Login(model);

                if (foundUser == null)
                {
                    return Unauthorized();
                }

                var tokenString = UserService.GenerateJWTToken(foundUser);

                return Ok(new
                {
                    Token = tokenString
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        #endregion
    }
}
