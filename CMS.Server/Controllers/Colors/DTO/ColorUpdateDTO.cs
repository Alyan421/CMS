using CMS.Server.Models;
namespace CMS.Server.Controllers.Colors.DTO
{
    public class ColorUpdateDTO
    {
        public int Id { get; set; }
        public int ClothId {  get; set; }
        public string ColorName { get; set; }
        public int AvailiableStock { get; set; }
        public ICollection<Image>? Images { get; set; }

    }
}
