using AutoMapper;
using CMS.Server.Models;
using CMS.Server.Controllers.Colors.DTO;

namespace CMS.Server.Controllers.Colors
{
    public class ClothAutoMapper : Profile
    {
        public ClothAutoMapper()
        {
            CreateMap<ColorGetDTO, Color>().ReverseMap();
            CreateMap<ColorCreateDTO, Color>().ReverseMap();
            CreateMap<ColorUpdateDTO, Color>().ReverseMap();
        }
    }
}