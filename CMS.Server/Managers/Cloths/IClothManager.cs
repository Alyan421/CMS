using CMS.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Server.Managers.Cloths
{
    public interface IClothManager
    {
        Task<Cloth> CreateClothAsync(Cloth cloth);
        Task<Cloth> UpdateClothAsync(Cloth cloth);
        Task<bool> DeleteClothAsync(int id);
        Task<Cloth> GetClothByIdAsync(int id);
        Task<IEnumerable<Cloth>> GetAllClothsAsync();
    }
}
