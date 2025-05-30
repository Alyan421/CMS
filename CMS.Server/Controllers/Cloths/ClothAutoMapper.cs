using AutoMapper;
using CMS.Server.Models;
using CMS.Server.Controllers.Cloths.DTO;

namespace CMS.Server.Controllers.Cloths
{
    public class ClothAutoMapper : Profile
    {
        public ClothAutoMapper()
        {
            CreateMap<ClothGetDTO, Cloth>().ReverseMap();
            CreateMap<ClothCreateDTO, Cloth>().ReverseMap();
            CreateMap<ClothUpdateDTO, Cloth>().ReverseMap();
        }
    }
}