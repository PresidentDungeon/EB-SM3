using System;
using System.Collections.Generic;
using EB.Core.ApplicationServices;
using EB.Core.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        // POST api/<BeerController>
        [HttpPost]
        [ProducesResponseType(typeof(Beer), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<Beer> Post([FromBody] Beer beer)
        {
            try
            {
                return BeerService.CreateBeer(beer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        // GET: api/<BeerController>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Beer>), 200)]
        [ProducesResponseType(404)] [ProducesResponseType(500)]
        public ActionResult<IEnumerable<Beer>> Get([FromQuery] Filter filter)
        {
            try
            {
                return Ok(BeerService.GetBeerFilterSearch(filter));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load beers. Please try again later.");
            }
        }

        // GET api/<BeerController>/5
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
        // PUT api/<BeerController>/5
        [HttpPut("{ID}")]
        [ProducesResponseType(typeof(Beer), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Beer> Put(int id, [FromBody] Beer beer)
        {
            if (id < 1 || id != beer.ID)
            {
                return BadRequest("Didn't find a matching ID.");
            }

            return Ok(BeerService.UpdateBeer(beer));
        }
        #endregion

        #region Delete
        // DELETE api/<BeerController>/5
        [HttpDelete("{ID}")]
        [ProducesResponseType(typeof(Beer), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Beer> Delete(int id)
        {
            var beer = BeerService.DeleteBeer(id);
            if (beer == null)
            {
                return StatusCode(404, "Couldn't find beer with ID: " + id);
            }
            return Ok($"Beer with ID: {id} has been deleted.");
        }
        #endregion
    }
}
