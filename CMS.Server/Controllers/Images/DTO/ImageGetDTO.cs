using System;

namespace CMS.Server.Controllers.Images.DTO
{
    public class ImageGetDTO
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public string URL { get; set; }
        public double Price { get; set; } // Add cloth price
        public string ClothName { get; set; } // Add cloth name
    }
}
