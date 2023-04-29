using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;

namespace EventNotifier.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile() {
            CreateMap<CreateEventDTO, Event>();
        }
    }
}
