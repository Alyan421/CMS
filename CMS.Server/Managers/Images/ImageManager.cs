using CMS.Server.Models;
using CMS.Server.Repository;
using CMS.Server.Controllers.Images.DTO;
using CMS.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
namespace CMS.Server.Managers.Images
{
    // ImageManager.cs
    public class ImageManager : IImageManager
    {
        private readonly IGenericRepository<Image> _imageRepository;
        private readonly IImageStorageService _imageStorage;

        public ImageManager(IGenericRepository<Image> imageRepository, IImageStorageService imageStorage)
        {
            _imageRepository = imageRepository;
            _imageStorage = imageStorage;
        }
        public async Task<List<ImageGetDTO>> GetAllAsync()
        {
            var images = await _imageRepository.GetAllAsync();
            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                IsPrimary = i.IsPrimary
            }).ToList();
        }

        public async Task<List<ImageGetDTO>> GetByColorIdAsync(int colorId)
        {
            var images = await _imageRepository.GetDbSet().Where(i => i.ColorId == colorId).ToListAsync();
            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                IsPrimary = i.IsPrimary
            }).ToList();
        }

        public async Task<ImageGetDTO> GetByIdAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null) return null;

            return new ImageGetDTO
            {
                Id = image.Id,
                ColorId = image.ColorId,
                URL = image.URL,
                IsPrimary = image.IsPrimary
            };
        }

        public async Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file)
        {
            var imageUrl = await _imageStorage.UploadImageAsync(file);
            var image = new Image
            {
                URL = imageUrl,
                ColorId = dto.ColorId,
                IsPrimary = dto.IsPrimary
            };



            await _imageRepository.AddAsync(image);

            return new ImageGetDTO
            {
                Id = image.Id,
                ColorId = image.ColorId,
                URL = image.URL,
                IsPrimary = image.IsPrimary
            };
        }

        public async Task UpdateAsync(ImageUpdateDTO dto, IFormFile newFile = null)
        {
            var image = await _imageRepository.GetByIdAsync(dto.Id);
            if (image == null) throw new Exception("Image not found");

            if (newFile != null)
            {
                var newUrl = await _imageStorage.UploadImageAsync(newFile);
                image.URL = newUrl;
            }

            image.ColorId = dto.ColorId;
            image.IsPrimary = dto.IsPrimary;

            await _imageRepository.UpdateAsync(image);
        }

        public async Task DeleteAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image != null)
            {
                await _imageStorage.DeleteImageAsync(image.URL);
                await _imageRepository.DeleteAsync(image);
            }
        }

        public async Task<List<ImageGetDTO>> FilterByClothNameAsync(string clothName)
        {
            var images = await _imageRepository.GetDbSet()
                .Where(i => i.Color.Cloth.Name == clothName && i.IsPrimary)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                IsPrimary = i.IsPrimary
            }).ToList();
        }


        public async Task<List<ImageGetDTO>> FilterByColorAsync(string colorName)
        {
            var images = await _imageRepository.GetDbSet()
                .Where(i => i.Color.ColorName == colorName && i.IsPrimary)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                IsPrimary = i.IsPrimary
            }).ToList();
        }
    }
}
