using AutoMapper;
using System;
using Domain.Catalog;
using Domain.Catalog;
using Domain.PropertyManagement;

namespace Application.Common.Models
{
    public class LookupDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Category, LookupDto>()
                    .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));

                CreateMap<ItemMaster, LookupDto>()
                    .ForMember(d => d.Title, opt => opt.MapFrom(s => s.ItemName));

                CreateMap<Property, LookupDto>()
                    .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));

                CreateMap<PropertyLocation, LookupDto>()
                    .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Name));

            }
        }
    }
}