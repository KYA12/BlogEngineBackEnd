using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendTestTask.DTO;
using BackendTestTask.Models;

namespace BackendTestTask.Automapper
{
    public class AutoMapperProfile : Profile
    {  
        //Setup AutoMapper profile settings
        public AutoMapperProfile()
        {
            CreateMap<PostDTO, Post>().ReverseMap();
            CreateMap<CommentDTO, Comment>().ReverseMap();
        }
    }
}
