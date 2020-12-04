using System;
using System.Collections.Generic;
using System.IO;
using EB.Core.ApplicationServices;
using EB.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EB.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Dependency Injection
        private readonly IOrderService OrderService;

        public OrderController(IOrderService orderService)
        {
            this.OrderService = orderService;
        }
        #endregion

        #region Create
        [HttpPost]
        [ProducesResponseType(typeof(Order), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public ActionResult<Order> CreateOrder([FromBody] Order order)
        {
            try
            {
                Order addedOrder = OrderService.AddOrder(order);

                if (addedOrder == null)
                {
                    return StatusCode(500, "Error creating order");
                }

                return Created("", addedOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error creating order");
            }
        }
        #endregion

        #region Read
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<IEnumerable<Order>> Get()
        {
            try
            {
                return Ok(OrderService.ReadAllOrders());
            }
            catch (InvalidDataException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load orders. Please try again later.");
            }
        }

        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Order> GetByID(int ID)
        {
            try
            {
                Order order = OrderService.ReadOrderByID(ID);
                if (order != null)
                {
                    return Ok(order);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading order with ID: {ID}\nPlease try again later.");
            }
        }

        [HttpGet("customer-{ID}")]
        [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<IEnumerable<Order>> GetOrderByCustomerID(int ID)
        {
            try
            {
                return Ok(OrderService.ReadAllOrdersByCustomer(ID));
            }
            catch (InvalidDataException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load customers orders. Please try again later.");
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{ID}")]
        [ProducesResponseType(typeof(Order), 202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Order> DeleteByID(int id)
        {
            try
            {
                Order order = OrderService.DeleteOrder(id);
                return (order != null) ? Accepted(order) : StatusCode(500, $"Server error deleting order with Id: {id}");
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
                return StatusCode(500, $"Server error deleting order with Id: {id}");
            }
        }
        #endregion
    }
}