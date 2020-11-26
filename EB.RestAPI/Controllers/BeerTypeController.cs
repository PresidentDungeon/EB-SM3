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
        // POST api/<BeerTypeController>
        [HttpPost]
        [ProducesResponseType(typeof(BeerType), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<BeerType> Post([FromBody] BeerType beerType)
        {
            try
            {
                return BeerTypeService.CreateType(beerType);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        // GET: api/<BeerTypeController>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BeerType>), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<IEnumerable<BeerType>> Get([FromQuery] Filter filter)
        {
            try
            {
                return Ok(BeerTypeService.GetTypesFilterSearch(filter));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load beertypes. Please try again later.");
            }
        }

        // GET api/<BeerTypeController>/5
        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(BeerType), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<BeerType> GetByID(int ID)
        {
            try
            {
                
                    return Ok(BeerTypeService.GetTypeById(ID));
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
        public ActionResult<BeerType> Put(int id, [FromBody] BeerType type)
        {
            if (id < 1 || id != type.ID)
            {
                return BadRequest("Didn't find a matching ID.");
            }

            return Ok(BeerTypeService.UpdateType(type));
        }
        #endregion

        #region Delete
        // DELETE api/<BeerTypeController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Beer), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<BeerType> Delete(int id)
        {
            var type = BeerTypeService.DeleteType(id);
            if (type == null)
            {
                return StatusCode(404, "Couldn't find type with ID: " + id);
            }
            return Ok($"Type with ID: {id} has been deleted.");
        }
        #endregion
    }
}
