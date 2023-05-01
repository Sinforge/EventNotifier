using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;

namespace EventNotifier.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile() {
            CreateMap<CreateEventDTO, Event>();
            CreateMap<Event, ReadEventDTO>()
                .ForMember(dest => dest.CurrentSubscribers, opt => opt.MapFrom((src, dest, destMember, context) => src.Subscribers.Count));
        }
    }
}
