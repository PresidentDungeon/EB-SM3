﻿using System;
using System.Collections.Generic;
using System.IO;
using EB.Core.ApplicationServices;
using EB.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductShop.Core.Entities;

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
        [Authorize]
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(FilterList<Order>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<IEnumerable<Order>> Get([FromQuery] Filter filter)
        {
            try
            {
                return Ok(OrderService.ReadAllOrders(filter));
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
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{OrderID}/{UserID}")]
        [Authorize]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Order> GetByIDUser(int orderID, int userID)
        {
            try
            {
                Order order = OrderService.ReadOrderByIDUser(orderID, userID);
                if (order != null)
                {
                    return Ok(order);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading order with ID: {orderID} as user with {userID}\nPlease try again later.");
            }
        }

        [HttpGet("customer-{ID}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<FilterList<Order>> GetOrderByCustomerID(int ID, [FromQuery] Filter filter)
        {
            try
            {
                return Ok(OrderService.ReadAllOrdersByCustomer(ID, filter));
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

        #region Update
        [HttpPut("{ID}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Order), 202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Order> UpdateOrderStatus(int id)
        {
            try
            {
                Order order = OrderService.UpdateOrderStatus(id);
                return (order != null) ? Accepted(order) : StatusCode(500, $"Server error updating order with Id: {id}");
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
                return StatusCode(500, $"Server error updating order with Id: {id}");
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
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