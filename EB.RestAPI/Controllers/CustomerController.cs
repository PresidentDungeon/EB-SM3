using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EB.Core.ApplicationServices;
using EB.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EB.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        #region Dependency Injection
        private readonly ICustomerService CustomerService;

        public CustomerController(ICustomerService customerService)
        {
            this.CustomerService = customerService;
        }
        #endregion

        #region Create
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Customer), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                Customer addedCustomer = CustomerService.CreateCustomer(customer);

                if (addedCustomer == null)
                {
                    return StatusCode(500, "Error saving customer info to Database");
                }

                return Created("", addedCustomer);
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
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Customer> GetByID(int ID)
        {
            try
            {
                Customer customer = CustomerService.GetCustomerById(ID);
                if (customer != null)
                {
                    return Ok(customer);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading customer with ID: {ID}\nPlease try again later.");
            }
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Customer), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Customer> UpdateByID(int id, [FromBody] Customer customer)
        {
            try
            {
                if (id < 1 || id != customer.ID)
                {
                    return BadRequest("Didn't find a matching ID.");
                }

                Customer updatedCustomer = CustomerService.UpdateCustomer(customer);
                return (updatedCustomer != null) ? Accepted(updatedCustomer) : StatusCode(500, $"Server error updating customer with Id: {id}");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

    }
}
