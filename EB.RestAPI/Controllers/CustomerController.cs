using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EB.Core.ApplicationServices;
using EB.Core.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        // POST api/<CustomerController>
        [HttpPost]
        [ProducesResponseType(typeof(Customer), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<Customer> Post([FromBody] Customer customer)
        {
            try
            {
                return CustomerService.CreateCustomer(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        // GET api/<CustomerController>/5
        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(Customer), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Customer> GetByID(int ID)
        {
            try
            {
                return Ok(CustomerService.GetCustomerById(ID));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading Customer with ID: {ID}\nPlease try again later.");
            }
        }
        #endregion

        #region Update
        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Customer), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Customer> Put(int id, [FromBody] Customer customer)
        {
            if (id < 1 || id != customer.ID)
            {
                return BadRequest("Didn't find a matching ID.");
            }

            return Ok(CustomerService.UpdateCustomer(customer));
        }
        #endregion

    }
}
