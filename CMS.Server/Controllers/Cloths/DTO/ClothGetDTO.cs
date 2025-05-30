using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Controllers.Cloths.DTO
{
    public class ClothGetDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Name { get; set;}
        public double Price { get; set; }
    }
}