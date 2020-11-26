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
    public class BeerTypeController : ControllerBase
    {
        #region Dependency Injection
        private readonly IBeerTypeService BeerTypeService;

        public BeerTypeController(IBeerTypeService beerTypeService)
        {
            this.BeerTypeService = beerTypeService;
        }
        #endregion

        #region Create
        [HttpPost]
        [ProducesResponseType(typeof(BeerType), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<BeerType> CreateBeerType([FromBody] BeerType beerType)
        {
            try
            {
                BeerType addedType = BeerTypeService.CreateType(beerType);

                if (addedType == null)
                {
                    return StatusCode(500, "Error saving beer type to Database");
                }

                return Created("", beerType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BeerType>), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<IEnumerable<BeerType>> Get([FromQuery] Filter filter)
        {
            try
            {
                return Ok(BeerTypeService.GetTypesFilterSearch(filter));
            }
            catch (InvalidDataException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load beertypes. Please try again later.");
            }
        }

        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(BeerType), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<BeerType> GetByID(int ID)
        {
            try
            {
                BeerType beerType = BeerTypeService.GetTypeById(ID);
                if (beerType != null)
                {
                    return Ok(beerType);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading beertype with ID: {ID}\nPlease try again later.");
            }
        }
        #endregion

        #region Update
        // PUT api/<BeerTypeController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BeerType), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<BeerType> UpdateByID(int id, [FromBody] BeerType type)
        {
            try
            {
                if (id < 1 || id != type.ID)
                {
                    return BadRequest("Didn't find a matching ID.");
                }

                BeerType updatedBeerType = BeerTypeService.UpdateType(type);
                return (updatedBeerType != null) ? Accepted(updatedBeerType) : StatusCode(500, $"Server error updating beer type with Id: {id}");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BeerType), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<BeerType> DeleteByID(int id)
        {
            try
            {
                BeerType beerType = BeerTypeService.DeleteType(id);
                return (beerType != null) ? Accepted(beerType) : StatusCode(500, $"Server error deleting beer type with Id: {id}");
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
                return StatusCode(500, $"Server error deleting beer type with Id: {id}");
            }
        }
        #endregion
    }
}
