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
    public class ImageManager : IImageManager
    {
        private readonly IGenericRepository<Image> _imageRepository;
        private readonly IGenericRepository<Color> _colorRepository;
        private readonly IGenericRepository<Cloth> _clothRepository;
        private readonly IImageStorageService _imageStorage;

        public ImageManager(
            IGenericRepository<Image> imageRepository,
            IGenericRepository<Color> colorRepository,
            IGenericRepository<Cloth> clothRepository,
            IImageStorageService imageStorage)
        {
            _imageRepository = imageRepository;
            _colorRepository = colorRepository;
            _clothRepository = clothRepository;
            _imageStorage = imageStorage;
        }

        public async Task<List<ImageGetDTO>> GetAllAsync()
        {
            // Get all images with their colors and associated cloths in a single query
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Color)
                .ThenInclude(c => c.Cloth)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                Price = i.Color?.Cloth?.Price ?? 0,
                ClothName = i.Color?.Cloth?.Name ?? "Unknown"
            }).ToList();
        }

        public async Task<List<ImageGetDTO>> GetByColorIdAsync(int colorId)
        {
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Color)
                .ThenInclude(c => c.Cloth)
                .Where(i => i.ColorId == colorId)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                Price = i.Color?.Cloth?.Price ?? 0,
                ClothName = i.Color?.Cloth?.Name ?? "Unknown"
            }).ToList();
        }

        public async Task<ImageGetDTO> GetByIdAsync(int id)
        {
            var image = await _imageRepository.GetDbSet()
                .Include(i => i.Color)
                .ThenInclude(c => c.Cloth)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null) return null;

            return new ImageGetDTO
            {
                Id = image.Id,
                ColorId = image.ColorId,
                URL = image.URL,
                Price = image.Color?.Cloth?.Price ?? 0,
                ClothName = image.Color?.Cloth?.Name ?? "Unknown"
            };
        }

        // Update other methods to maintain the same pattern...

        public async Task<List<ImageGetDTO>> FilterByClothNameAsync(string clothName)
        {
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Color)
                .ThenInclude(c => c.Cloth)
                .Where(i => i.Color.Cloth.Name.Contains(clothName))
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                Price = i.Color?.Cloth?.Price ?? 0,
                ClothName = i.Color?.Cloth?.Name ?? "Unknown"
            }).ToList();
        }

        public async Task<List<ImageGetDTO>> FilterByColorAsync(string colorName)
        {
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Color)
                .ThenInclude(c => c.Cloth)
                .Where(i => i.Color.ColorName.Contains(colorName))
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ColorId = i.ColorId,
                URL = i.URL,
                Price = i.Color?.Cloth?.Price ?? 0,
                ClothName = i.Color?.Cloth?.Name ?? "Unknown"
            }).ToList();
        }

        // Implement the other methods from IImageManager that we didn't change...
        // CreateAsync, UpdateAsync, DeleteAsync should remain unchanged

    public async Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file)
        {
            // Check if an image already exists for this color
            var existingImage = await _imageRepository.GetDbSet()
                .FirstOrDefaultAsync(i => i.ColorId == dto.ColorId);

            var imageUrl = await _imageStorage.UploadImageAsync(file);

            if (existingImage != null)
            {
                // Optionally delete the old image file
                await _imageStorage.DeleteImageAsync(existingImage.URL);

                // Update the existing image
                existingImage.URL = imageUrl;
                await _imageRepository.UpdateAsync(existingImage);
                await _imageRepository.SaveChangesAsync();

                return new ImageGetDTO
                {
                    Id = existingImage.Id,
                    ColorId = existingImage.ColorId,
                    URL = existingImage.URL,
                };
            }
            else
            {
                // Create a new image
                var image = new Image
                {
                    URL = imageUrl,
                    ColorId = dto.ColorId,
                };
                await _imageRepository.AddAsync(image);
                await _imageRepository.SaveChangesAsync();


                return new ImageGetDTO
                {
                    Id = image.Id,
                    ColorId = image.ColorId,
                    URL = image.URL,
                };
            }
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

            await _imageRepository.UpdateAsync(image);
            await _imageRepository.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image != null)
            {
                await _imageStorage.DeleteImageAsync(image.URL);
                await _imageRepository.DeleteAsync(image);
                await _imageRepository.SaveChangesAsync();

            }
        }

    }
}