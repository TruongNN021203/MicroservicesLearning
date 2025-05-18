using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.DTOs.Conversions;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.SharedLibrary.Responses;
namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            //Get all product from repo
            var products = await productInterface.GetAllAsync();
            if (products is null || products.Count() == 0)
            {
                return NotFound("No product detected in the database");
            }
            //convert data from entity to DTO and returned
            var (_, list) = ProductConversion.FromEntity(null!, products);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            //Get single product from repo
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
            {
                return NotFound("Product request not found");
            }
            //convert data from entity to DTO and returned
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return Ok(_product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            //check model state is valid anotation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);

        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            //check model state is valid anotation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);

        }


        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            //check model state is valid anotation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);

        }

    }
}
