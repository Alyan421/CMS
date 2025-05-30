using System.ComponentModel.DataAnnotations;
using CMS.Server.Models;
namespace CMS.Server.Controllers.Cloths.DTO
{
    public class ClothCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}
