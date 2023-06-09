﻿using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EventNotifier.Repositories
{
    public interface IEventRepo
    {
        public bool CreateEvent(Event @event);
        public IEnumerable<Event> GetEvents();
        public Event? GetEventById(int id);

        public void CreateNotification(Notification notification);
        public void ChangeToComplete(int @eventId);
        public bool SaveChanges();
        public void ClearNotifications();
        IEnumerable<Event> GetEventsByCoords(Coordinate coord, double distance);
        //  public List<Rating> GetRatingsBySameEvent(User user);

        bool DeleteEvent(int eventId);

        IEnumerable<Notification> GetAllNotifications(string email);

    }
}
