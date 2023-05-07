using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using NetTopologySuite.Geometries;

namespace EventNotifier.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile() {
            CreateMap<CreateEventDTO, Event>()
                .ForMember(dest => dest.Point, 
                opt => opt.MapFrom(
                    (src, dest, destMember, context)=> new NetTopologySuite.Geometries.Point(new Coordinate(src.Point.x, src.Point.y)))
                );
            CreateMap<Event, ReadEventDTO>()
                .ForMember(dest => dest.CurrentSubscribers, opt => opt.MapFrom((src, dest, destMember, context) => src.Subscribers.Count))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom((src, dest, destMemver, context) => src.Ratings.Count != 0 ? src.Ratings.Sum(r => r.RatingNumber) / src.Ratings.Count : 0));
        }
    }
}
