using ImageUploader;
using MacDonalsApi.Data;
using MacDonalsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MacDonalsApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProductsController(ApplicationContext Context)
        {
            _context = Context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var Products = _context.products.ToList();

            return Ok(Products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var Product = _context.products.Find(id);

            return Ok(Product);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductByCategoryId(int id)
        {
            var Products = _context.products.Where(x => x.CategoryId == id);

            return Ok(Products);
        }

        [HttpGet]
        public IActionResult PopularProduct()
        {
            var Products = _context.products.Where(x=>x.IsPopularProduct == true);

            return Ok(Products);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {
            var stream = new MemoryStream(product.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "www.root";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest("this image is not fount");
            }
            else
            {
                product.ImageUrl = file;
                _context.products.Add(product);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
           
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product product,int id)
        {
            var entity = _context.products.Find(id);
            if (entity == null)
            {
                return NotFound("this product not found");
            }
            else
            {
                var stream = new MemoryStream(product.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot";
                var response = FilesHelper.UploadImage(stream, folder, file);
                if (!response)
                {
                    return BadRequest("this image is not found");
                }
                else
                {
                    entity.CategoryId = product.CategoryId;
                    entity.Name = product.Name;
                    entity.ImageUrl = file;
                    entity.Price = product.Price;
                    entity.orderDetails = product.orderDetails;

                    _context.SaveChanges();
                    return Ok("Updated Successuflly");


                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.products.Find(id);
            if(product == null)
            {
                return NotFound("this product is not found");
            }
            else
            {
                _context.products.Remove(product);
                _context.SaveChanges();
                return Ok("Deleted Successfuly");

            }
          
        }

    }
}
