using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Colors.DTO;

namespace CMS.Server.Controllers.Colors
{
    public interface IColorController
    {
        Task<IActionResult> CreateAsync(ColorCreateDTO colorCreateDTO);
        Task<IActionResult> GetByIdAsync(int id);
        Task<IActionResult> UpdateAsync(ColorUpdateDTO colorUpdateDTO);
        Task<IActionResult> DeleteColorAsync(int id);
        Task<IActionResult> GetAllAsync();

    }
}
