namespace CMS.Server.Models
{
    public class Color : BaseEntity<int>
    {
        public string ColorName { get; set; }
        public int AvailiableStock { get; set; }
        public int ClothId { get; set; }
        public Cloth Cloth { get; set; }
        public Image Image { get; set; }
    }
}
