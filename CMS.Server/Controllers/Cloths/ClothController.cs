using AutoMapper;
using CMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Managers.Cloths;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Services;
using CMS.Server.Controllers.Cloths.DTO;
using System;
using Microsoft.AspNetCore.Identity;

namespace CMS.Server.Controllers.Cloths
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClothController : ControllerBase, IClothController
    {
        private readonly IClothManager _clothManager;
        private readonly IMapper _mapper;

        public ClothController(IClothManager clothManager, IMapper mapper)
        {
            _clothManager = clothManager;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(ClothCreateDTO clothCreateDTO)
        {
            try
            {
                if (clothCreateDTO == null)
                {
                    return BadRequest("Cloth data is required.");
                }

                var cloth = _mapper.Map<Cloth>(clothCreateDTO);
                var clothCreate = await _clothManager.CreateClothAsync(cloth);

                if (clothCreate == null)
                {
                    return StatusCode(500, "An error occurred while creating thecloth.");
                }

                var clothDTO = _mapper.Map<ClothCreateDTO>(clothCreate);
                return Ok(clothDTO);
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
                    return BadRequest("Invalidcloth ID.");
                }

                var cloth = await _clothManager.GetClothByIdAsync(id);
                if(cloth == null)
                {
                    return NotFound($"Cloth with ID {id} not found.");
                }

                var clothDTO = _mapper.Map<ClothGetDTO>(cloth);
                return Ok(clothDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ClothUpdateDTO clothUpdateDTO)
        {
            try
            {
                if (clothUpdateDTO == null || clothUpdateDTO.Id <= 0)
                {
                    return BadRequest("Validcloth data is required.");
                }

                var cloth = _mapper.Map<Cloth>(clothUpdateDTO);
                var updatedCloth = await _clothManager.UpdateClothAsync(cloth);

                if (updatedCloth == null)
                {
                    return NotFound($"Cloth with ID {clothUpdateDTO.Id} not found.");
                }

                var clothDTO = _mapper.Map<ClothUpdateDTO>(updatedCloth);
                return Ok(clothDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClothAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalidcloth ID.");
                }

                var cloth = await _clothManager.GetClothByIdAsync(id);
                if(cloth == null)
                {
                    return NotFound($"Cloth with ID {id} not found.");
                }

                await _clothManager.DeleteClothAsync(id);
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
                var cloths = await _clothManager.GetAllClothsAsync();
                if (cloths == null)
                {
                    return NotFound("Nocloths found.");
                }

                var clothDTOs = _mapper.Map<IEnumerable<ClothGetDTO>>(cloths);
                return Ok(clothDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //public async Task<IActionResult> GetClothIdByClothname(string clothname)
        //{
        //    if (string.IsNullOrWhiteSpace(clothname))
        //    {
        //        return BadRequest("Clothname cannot be empty.");
        //    }

        //    try
        //    {
        //        var clothId = await _clothManager.GetClothIdByClothnameAsync(clothname);
        //        if(clothId == null)
        //        {
        //            return NotFound("Cloth not found.");
        //        }

        //        return Ok(clothId);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception if logging is enabled
        //        return StatusCode(500, "Internal server error.");
        //    }
        //}
    }
}