using AutoMapper;
using CMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Managers.Colors;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Services;
using CMS.Server.Controllers.Colors.DTO;
using System;
using Microsoft.AspNetCore.Identity;

namespace CMS.Server.Controllers.Colors
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorController : ControllerBase, IColorController
    {
        private readonly IColorManager _colorManager;
        private readonly IMapper _mapper;

        public ColorController(IColorManager colorManager, IMapper mapper)
        {
            _colorManager = colorManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ColorCreateDTO ColorCreateDTO)
        {
            try
            {
                if (ColorCreateDTO == null)
                {
                    return BadRequest("Color data is required.");
                }

                var color = _mapper.Map<Color>(ColorCreateDTO);
                var createdColor = await _colorManager.CreateColorAsync(color);

                if (createdColor == null)
                {
                    return StatusCode(500, "An error occurred while creating thecolor.");
                }

                var colorDTO = _mapper.Map<ColorGetDTO>(createdColor);
                return Ok(colorDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalidcolor ID.");
                }

                var color = await _colorManager.GetColorByIdAsync(id);
                if (color == null)
                {
                    return NotFound($"Color with ID {id} not found.");
                }

                var colorDTO = _mapper.Map<ColorGetDTO>(color);
                return Ok(colorDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ColorUpdateDTO colorUpdateDTO)
        {
            try
            {
                if (colorUpdateDTO == null || colorUpdateDTO.Id <= 0)
                {
                    return BadRequest("Validcolor data is required.");
                }

                var color = _mapper.Map<Color>(colorUpdateDTO);
                var updatedColor = await _colorManager.UpdateColorAsync(color);

                if (updatedColor == null)
                {
                    return NotFound($"Color with ID {colorUpdateDTO.Id} not found.");
                }

                var colorDTO = _mapper.Map<ColorUpdateDTO>(updatedColor);
                return Ok(colorDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteColorAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalidcolor ID.");
                }

                var color = await _colorManager.GetColorByIdAsync(id);
                if (color == null)
                {
                    return NotFound($"Color with ID {id} not found.");
                }

                await _colorManager.DeleteColorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var colors = await _colorManager.GetAllColorsAsync();
                if (colors == null)
                {
                    return NotFound("Nocolors found.");
                }

                var colorDTOs = _mapper.Map<IEnumerable<ColorGetDTO>>(colors);
                return Ok(colorDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //public async Task<IActionResult> GetColorIdByColorname(string colorname)
        //{
        //    if (string.IsNullOrWhiteSpace(colorname))
        //    {
        //        return BadRequest("Colorname cannot be empty.");
        //    }

        //    try
        //    {
        //        var colorId = await _colorManager.GetColorIdByColornameAsync(colorname);
        //        if (colorId == null)
        //        {
        //            return NotFound("Color not found.");
        //        }

        //        return Ok(colorId);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if logging is enabled
        //        return StatusCode(500, "Internal server error.");
        //    }
        //}
    }
}