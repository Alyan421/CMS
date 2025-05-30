using System.ComponentModel.DataAnnotations;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Colors.DTO
{
    public class ColorGetDTO
    {
        public int Id { get; set; }
        public int ClothId { get; set; }
        public string ColorName { get; set;}
        public int AvailibleStock { get; set; }
        public ICollection<Image>? Images { get; set; }
    }
}