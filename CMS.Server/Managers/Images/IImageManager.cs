using CMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Images.DTO;

namespace CMS.Server.Managers.Images
{
    // IImageManager.cs
    public interface IImageManager
    {
        Task<List<ImageGetDTO>> GetAllAsync();
        Task<List<ImageGetDTO>> GetByColorIdAsync(int colorId);
        Task<ImageGetDTO> GetByIdAsync(int id);
        Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file);
        Task UpdateAsync(ImageUpdateDTO dto, IFormFile newFile = null);
        Task DeleteAsync(int id);
        Task<List<ImageGetDTO>> FilterByClothNameAsync(string clothName);
        Task<List<ImageGetDTO>> FilterByColorAsync(string colorName);
    }
}
