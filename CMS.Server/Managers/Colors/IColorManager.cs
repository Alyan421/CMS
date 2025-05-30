using CMS.Server.Models;

namespace CMS.Server.Managers.Colors
{
    public interface IColorManager
    {
        Task<Color> CreateColorAsync(Color entity);
        Task<Color> UpdateColorAsync(Color entity);
        Task<Color> GetColorByIdAsync(int id);
        Task<IEnumerable<Color>> GetAllColorsAsync();
        Task DeleteColorAsync(int id);
    }
}
