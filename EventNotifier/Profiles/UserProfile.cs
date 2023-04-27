using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;

namespace EventNotifier.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            // Source --> Target
            CreateMap<CreateUserDTO, User>();
        }

    }
}
