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
    public class BeerController : ControllerBase
    {
        #region Dependency Injection
        private readonly IBeerService BeerService;

        public BeerController(IBeerService beerService)
        {
            this.BeerService = beerService;
        }
        #endregion

        #region Create
        [HttpPost]
        [ProducesResponseType(typeof(Beer), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<Beer> CreateBeer([FromBody] Beer beer)
        {
            try
            {
                Beer addedBeer = BeerService.CreateBeer(beer);

                if (addedBeer == null)
                {
                    return StatusCode(500, "Error saving product to Database");
                }

                return Created("", addedBeer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Beer>), 200)]
        [ProducesResponseType(404)] [ProducesResponseType(500)]
        public ActionResult<IEnumerable<Beer>> Get([FromQuery] Filter filter)
        {
            try
            {
                return Ok(BeerService.GetBeerFilterSearch(filter));
            }
            catch (InvalidDataException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load beers. Please try again later.");
            }
        }

        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(Beer), 200)]
        [ProducesResponseType(404)] [ProducesResponseType(500)]
        public ActionResult<Beer> GetByID(int ID)
        {
            try
            {
                Beer beer = BeerService.GetBeerById(ID);
                if (beer != null)
                {
                    return Ok(beer);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading beer with ID: {ID}\nPlease try again later.");
            }
        }
        #endregion

        #region Update
        [HttpPut("{ID}")]
        [ProducesResponseType(typeof(Beer), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Beer> UpdateByID(int id, [FromBody] Beer beer)
        {
            try
            {
                if (id < 1 || id != beer.ID)
                {
                    return BadRequest("Didn't find a matching ID.");
                }

                Beer updatedBeer = BeerService.UpdateBeer(beer);
                return (updatedBeer != null) ? Accepted(beer) : StatusCode(500, $"Server error updating beer with Id: {id}");
            }
            catch(InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{ID}")]
        [ProducesResponseType(typeof(Beer), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Beer> DeleteByID(int id)
        {
            try
            {
                Beer beer = BeerService.DeleteBeer(id);
                return (beer != null) ? Accepted(beer) : StatusCode(500, $"Server error deleting beer with Id: {id}");
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
                return StatusCode(500, $"Server error deleting product with Id: {id}");
            }
        }
        #endregion
    }
}
