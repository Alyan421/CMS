using System.ComponentModel.DataAnnotations;
using CMS.Server.Models;
using CMS.Server.Controllers.Images.DTO;
namespace CMS.Server.Controllers.Colors.DTO
{
    public class ColorCreateDTO
    {
        [Required]
        public string ColorName { get; set; }
        [Required]
        public int AvailiableStock { get; set; }
        [Required]
        public int ClothId { get; set; }

    }
}