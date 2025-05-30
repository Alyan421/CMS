using CMS.Server.Models;
using CMS.Server.Repository;
using CMS.Server.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Server.Managers.Cloths
{
    public class ClothManager : IClothManager
    {
        private readonly IGenericRepository<Cloth> _repository;
        private readonly IGenericRepository<Color> _colorRepository;

        public ClothManager(IGenericRepository<Cloth> repository, IGenericRepository<Color> colorRepository)
        {
            _repository = repository;
            _colorRepository = colorRepository;
        }

        public async Task<Cloth> CreateClothAsync(Cloth cloth)
        {
            try
            {
                await _repository.AddAsync(cloth);
                return cloth;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while creating the cloth.", ex);
            }
        }

        public async Task<Cloth> UpdateClothAsync(Cloth cloth)
        {
            try
            {
                await _repository.UpdateAsync(cloth);
                return cloth;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while updating the cloth.", ex);
            }
        }

        public async Task<bool> DeleteClothAsync(int id)
        {
            try
            {
                var cloth = await _repository.GetByIdAsync(id);
                if (cloth == null) return false;

                while (true)
                {
                    var color = await _colorRepository.GetDbSet().FirstOrDefaultAsync(c => c.ClothId == id);
                    if (color == null) break;
                    else
                    {
                        await _colorRepository.DeleteAsync(color);
                        await _colorRepository.SaveChangesAsync();
                    }
                }

                await _repository.DeleteAsync(cloth);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while deleting the cloth.", ex);
            }
        }

        public async Task<Cloth> GetClothByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while retrieving the cloth by ID.", ex);
            }
        }

        public async Task<IEnumerable<Cloth>> GetAllClothsAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new Exception("An error occurred while retrieving all cloths.", ex);
            }
        }
    }
}