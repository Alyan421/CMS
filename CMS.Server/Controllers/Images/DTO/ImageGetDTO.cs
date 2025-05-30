using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Controllers.Images.DTO
{
    public class ImageGetDTO
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public string URL { get; set;}
        public bool IsPrimary { get; set; }
    }
}