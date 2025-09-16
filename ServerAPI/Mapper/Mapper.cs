using AutoMapper;
using ServerAPI.Dtos;
using ServerAPI.Entities;

namespace ServerAPI.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Service, ServiceDto>();
        CreateMap<ServiceDto, Service>().ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Client, ClientDto>();
        CreateMap<ClientDto, Client>().ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service))
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client));
        CreateMap<InvoiceDto, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Service, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore());
    }
}
