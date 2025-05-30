using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Server.Models
{
    public class Image : BaseEntity<int>
    {
        public string URL { get; set; }
        public bool IsPrimary { get; set; } = false;
        public int ColorId { get; set; }
        public Color Color { get; set; }
    }
}

