using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Controllers.Images.DTO
{
    public class ImageCreateDTO
    {
        [Required]
        public int ColorId { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
