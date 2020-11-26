using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EB.Core.ApplicationServices;
using EB.Core.Entities;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost]
        [ProducesResponseType(typeof(Brand), 201)]
        [ProducesResponseType(400)][ProducesResponseType(500)]
        public ActionResult<Brand> CreateBrand([FromBody] Brand brand)
        {
            try
            {
                Brand addedBrand = BrandService.CreateBrand(brand);

                if (addedBrand == null)
                {
                    return StatusCode(500, "Error saving brand to Database");
                }

                return Created("", addedBrand);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Read
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Brand>), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<IEnumerable<Brand>> Get([FromQuery] Filter filter)
        {
            try
            {
                return Ok(BrandService.GetBrandFilterSearch(filter));
            }
            catch (InvalidDataException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Couldn't load brand. Please try again later.");
            }
        }

        [HttpGet("{ID}")]
        [ProducesResponseType(typeof(Brand), 200)]
        [ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Brand> GetByID(int ID)
        {
            try
            {
                Brand brand = BrandService.GetBrandById(ID);
                if (brand != null)
                {
                    return Ok(brand);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading brand with ID: {ID}\nPlease try again later.");
            }
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Brand), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Brand> UpdateByID(int id, [FromBody] Brand brand)
        {
            try
            {
                if (id < 1 || id != brand.ID)
                {
                    return BadRequest("Didn't find a matching ID.");
                }

                Brand updatedBrand = BrandService.UpdateBrand(brand);
                return (updatedBrand != null) ? Accepted(updatedBrand) : StatusCode(500, $"Server error updating brand with Id: {id}");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Brand), 202)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(500)]
        public ActionResult<Brand> DeleteByID(int id)
        {
            try
            {
                Brand brand = BrandService.DeleteBrand(id);
                return (brand != null) ? Accepted(brand) : StatusCode(500, $"Server error deleting brand with Id: {id}");
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
