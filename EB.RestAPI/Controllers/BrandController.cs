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
    public class BrandController : ControllerBase
    {
        #region Dependency Injection
        private readonly IBrandService BrandService;

        public BrandController(IBrandService brandService)
        {
            this.BrandService = brandService;
        }
        #endregion

        #region Create
        // POST api/<BrandController>
        [HttpPost]
        [ProducesResponseType(typeof(Brand), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<Brand> Post([FromBody] Brand brand)
        {
            try
            {
                return BrandService.CreateBrand(brand);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        // GET: api/<BrandController>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Brand>), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<IEnumerable<Brand>> Get([FromQuery] Filter filter)
        {
            try
            {
                return Ok(BrandService.GetBrandFilterSearch(filter));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load brand. Please try again later.");
            }
        }

        // GET api/<BrandController>/5
        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(Brand), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Brand> GetByID(int ID)
        {
            try
            {

                return Ok(BrandService.GetBrandById(ID));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading brand with ID: {ID}\nPlease try again later.");
            }
        }
        #endregion

        #region Update
        // PUT api/<BrandController>/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Brand), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Brand> Put(int id, [FromBody] Brand brand)
        {
            if (id < 1 || id != brand.ID)
            {
                return BadRequest("Didn't find a matching ID.");
            }

            return Ok(BrandService.UpdateBrand(brand));
        }
        #endregion

        #region Delete
        // DELETE api/<BrandController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Brand), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Brand> Delete(int id)
        {
            var type = BrandService.DeleteBrand(id);
            if (type == null)
            {
                return StatusCode(404, "Couldn't find brand with ID: " + id);
            }
            return Ok($"Brand with ID: {id} has been deleted.");
        }
        #endregion
    }
}
