using AutoMapper;
using OnlineLearningPlatform.Application.DTOs.Response;
using OnlineLearningPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Application.Configuration
{
    public class AutoMappers : Profile
    {
        public AutoMappers()
        {
            // Map ApplicationUser to AuthResponse
            CreateMap<ApplicationUser, AuthResponse>()
                .ForMember(dest => dest.Token, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        }
    }
}
