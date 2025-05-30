using AutoMapper;
using CMS.Server.Models;
using CMS.Server.Controllers.Images.DTO;

namespace CMS.Server.Controllers.Images
{
    public class ImageAutoMapper : Profile
    {
        public ImageAutoMapper()
        {
            CreateMap<ImageGetDTO, Image>().ReverseMap();
            CreateMap<ImageCreateDTO, Image>().ReverseMap();
            CreateMap<ImageUpdateDTO, Image>().ReverseMap();
        }
    }
}