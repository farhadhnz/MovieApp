using AutoMapper;
using MovieApp.API.Models.Linking;
using MovieApp.API.Models.Resources;
using MovieApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.API.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MovieEntity, Movie>()
                .ForMember(dest => dest.Self, opt => opt.MapFrom(src => 
                    Link.To(nameof(Controllers.MoviesController.GetMovieByIdAsync), new { movieId = src.Id })));

            CreateMap<UserEntity, User>();
        }
    }
}
