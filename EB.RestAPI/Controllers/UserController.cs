using System;
using EB.Core.ApplicationServices;
using EB.Core.Entities.Security;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EB.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Dependency Injection
        private readonly IUserService UserService;

        public UserController(IUserService userService)
        {
            this.UserService = userService;
        }
        #endregion

        #region Create
        // POST api/<BeerController>
        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<User> Post([FromBody] User user)
        {
            try
            {
                return UserService.AddUser(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        // GET api/<BeerController>/5
        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<User> GetByID(int ID)
        {
            try
            {
                User user = UserService.GetUserByID(ID);
                if (user != null)
                {
                    return Ok(user);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading User with ID: {ID}\nPlease try again later.");
            }
        }
        #endregion

        #region Update
        // PUT api/<BeerController>/5
        [HttpPut("{ID}")]
        [ProducesResponseType(typeof(User), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<User> Put(int id, [FromBody] User user)
        {
            if (id < 1 || id != user.ID)
            {
                return BadRequest("Didn't find a matching ID.");
            }

            return Ok(UserService.UpdateUser(user));
        }
        #endregion

        #region Delete
        // DELETE api/<BeerController>/5
        [HttpDelete("{ID}")]
        [ProducesResponseType(typeof(User), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<User> Delete(int id)
        {
            var user = UserService.DeleteUser(id);
            if (user == null)
            {
                return StatusCode(404, "Couldn't find User with ID: " + id);
            }
            return Ok($"User with ID: {id} has been deleted.");
        }
        #endregion
    }
}
