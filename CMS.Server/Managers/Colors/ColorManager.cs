using CMS.Server.Models;
using CMS.Server.Repository;
using CMS.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Managers.Colors
{
    public class ColorManager : IColorManager
    {
        private readonly IGenericRepository<Color> _repository;
        private readonly IGenericRepository<Cloth> _clothRepository;
        private readonly IGenericRepository<Image> _imageRepository;

        public ColorManager(IGenericRepository<Color> repository, IGenericRepository<Cloth> clothRepository,IGenericRepository<Image> imageRepository)
        {
            _repository = repository;
            _clothRepository = clothRepository;
            _imageRepository = imageRepository;
        }

        public async Task<Color> CreateColorAsync(Color color)
        {
            try
            {
                var existingCloth = await _clothRepository.GetByIdAsync(color.ClothId);
                if (existingCloth == null)
                {
                    throw new KeyNotFoundException("Cloth not found.");
                }
                Console.WriteLine(existingCloth);
                await _repository.AddAsync(color);
                return color;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while creating the color.", ex);
            }
        }

        public async Task<Color> UpdateColorAsync(Color color)
        {
            try
            {
                await _repository.UpdateAsync(color);
                return color;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while updating the color.", ex);
            }
        }

        public async Task DeleteColorAsync(int id)
        {
            try
            {
                var color = await _repository.GetByIdAsync(id);
                if (color == null)
                {
                    throw new KeyNotFoundException("Color not found.");
                }

                while (true)
                {
                    var image = await _imageRepository.GetDbSet().FirstOrDefaultAsync(i => i.ColorId == id);
                    if (image == null) break;
                    else
                    {
                        await _imageRepository.DeleteAsync(image);
                        await _imageRepository.SaveChangesAsync();
                    }
                }

                await _repository.DeleteAsync(color);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the color.", ex);
            }
        }

        public async Task<Color> GetColorByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while retrieving the color by ID.", ex);
            }
        }

        public async Task<IEnumerable<Color>> GetAllColorsAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while retrieving all colors.", ex);
            }
        }
    }
}
