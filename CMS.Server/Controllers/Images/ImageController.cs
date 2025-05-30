using AutoMapper;
using CMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Managers.Images;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Services;
using CMS.Server.Controllers.Images.DTO;
using System;
using Microsoft.AspNetCore.Identity;

namespace CMS.Server.Controllers.Images
{
    // ImageController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase, IImageController
    {
        private readonly IImageManager _manager;

        public ImageController(IImageManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _manager.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("by-color/{colorId}")]
        public async Task<IActionResult> GetByColor(int colorId)
        {
            var result = await _manager.GetByColorIdAsync(colorId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _manager.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

 //       [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] ImageCreateDTO dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                return BadRequest("Image is required");

            var result = await _manager.CreateAsync(dto, dto.ImageFile);
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ImageUpdateDTO dto, [FromForm] IFormFile image = null)
        {
            dto.Id = id;
            await _manager.UpdateAsync(dto, image);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _manager.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("filter-by-cloth-name/{clothName}")]
        public async Task<IActionResult> FilterByClothName(string clothName)
        {
            var images = await _manager.FilterByClothNameAsync(clothName);
            return Ok(images);
        }

        [HttpGet("filter-by-color/{colorName}")]
        public async Task<IActionResult> FilterByColor(string colorName)
        {
            var images = await _manager.FilterByColorAsync(colorName);
            return Ok(images);
        }
    }

}