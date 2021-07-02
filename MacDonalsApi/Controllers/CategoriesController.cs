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
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _Context;

        public CategoriesController(ApplicationContext Context)
        {
            _Context = Context;
        }

        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var Categories = _Context.categories.ToList();

            return Ok(Categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var Category = _Context.categories.Where(x=>x.Id == id);

            return Ok(Category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCategory([FromBody] Category category)
        {
            var stream = new MemoryStream(category.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = $"{guid}.jpg";
            var folder = "wwwroot";
            var response = FilesHelper.UploadImage(stream, folder, file);
            if (!response)
            {
                return BadRequest("this is image is not found");
            }
            else
            {
                category.ImageUrl = file;
                _Context.categories.Add(category);
                _Context.SaveChanges();

                return StatusCode(StatusCodes.Status201Created);
            }
            
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateCategory([FromBody] Category category , int id)
        {
            var entity = _Context.categories.Find(id);
            if (entity == null)
            {
                return NotFound("this is category is not found");
            }
            else
            {
                var stream = new MemoryStream(category.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot";
                var response = FilesHelper.UploadImage(stream,folder,file);
                if (!response)
                {
                    return BadRequest("this is image is not found");
                }
                else
                {
                    entity.ImageUrl = file;
                    entity.Name = category.Name;
                    _Context.SaveChanges();
                    return Ok("Updated Successfuly");
                }
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var entity = _Context.categories.Find(id);
            if (entity == null)
            {
                return NotFound("this category is not found");
            }
            else
            {
                _Context.categories.Remove(entity);
                _Context.SaveChanges();
                return Ok("Deleted Successfully");
            }
     
        }
    }
}
