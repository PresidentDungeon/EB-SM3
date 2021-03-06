﻿using System;
using System.IO;
using EB.Core.ApplicationServices;
using EB.Core.Entities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<User> CreateUser([FromBody] LoginInputModel model)
        {
            try
            {
                User createdUser = UserService.CreateUser(model.Username, model.Password, "User");
                User addedUser = UserService.AddUser(createdUser);

                if (addedUser == null)
                {
                    return StatusCode(500, "Error saving user info to Database");
                }

                return Created("", addedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        [HttpGet("{ID}")]
        [Authorize]
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
        [HttpPut("{ID}")]
        [Authorize]
        [ProducesResponseType(typeof(User), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<User> UpdateByID(int id, [FromBody] User user)
        {
            try
            {
                if (id < 1 || id != user.ID)
                {
                    return BadRequest("Didn't find a matching ID.");
                }

                User updatedUser = UserService.UpdateUser(user);
                return (updatedUser != null) ? Accepted(updatedUser) : StatusCode(500, $"Server error updating user with Id: {id}");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("password/{ID}")]
        [Authorize]
        [ProducesResponseType(typeof(User), 202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<User> UpdatePasswordByID(int id, [FromBody] UpdatePasswordModel updateModel)
        {
            try
            {
                User updatedUser = UserService.UpdatePassword(id, updateModel);
                return (updatedUser != null) ? Accepted(updatedUser) : StatusCode(500, $"Server error updating user with Id: {id}");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(User), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<User> DeleteByID(int id)
        {
            try
            {
                User user = UserService.DeleteUser(id);
                return (user != null) ? Accepted(user) : StatusCode(500, $"Server error deleting user with Id: {id}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error deleting brand with Id: {id}");
            }
        }
        #endregion
    }
}
