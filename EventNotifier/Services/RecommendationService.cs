using EventNotifier.Models;
using EventNotifier.Repositories;

namespace EventNotifier.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IEventRepo _eventRepo;
        public RecommendationService(IEventRepo eventRepo) {
            _eventRepo = eventRepo;
        }
        public double ComputeSimilarity(User user1, User user2)
        {
            // Get common events of user1 and user2
            var commonEvents = user1.Ratings.Select(r => r.Event)
                                       .Intersect(user2.Ratings.Select(r => r.Event));
            if (!commonEvents.Any())
                return 0;
            //Lists of rating numbers of user1 and user2
            var u1Ratings = commonEvents.Select(e => user1.Ratings.First(r => r.Event == e).RatingNumber);
            var u2Ratings = commonEvents.Select(e => user2.Ratings.First(r => r.Event == e).RatingNumber);

            //Calculate medians
            var dotEvent = u1Ratings.Zip(u2Ratings, (rating1, rating2) => rating1 * rating2).Sum();
            // Calculate normals of ratings
            var norm1 = Math.Sqrt(u1Ratings.Select(r => r * r).Sum());
            var norm2 = Math.Sqrt(u2Ratings.Select(r => r * r).Sum());

            return dotEvent / (norm1 * norm2);
        }

        public IEnumerable<Event> GetRecommendation(User user)
        {
            // Get all user not looked by user ratings
            var allRatings = _eventRepo.GetRatingsBySameEvent(user);

            // Take first 10 similarities of users ratings
            var similarities = allRatings.GroupBy(r => r.User)
                                         .Select(g => new { User = g.Key, Similarity = ComputeSimilarity(user, g.Key) })
                                         .Where(u => u.Similarity > 0)
                                         .OrderByDescending(u => u.Similarity)
                                         .Take(10);


            var recommendations = new List<Event>();
            foreach (var similarity in similarities)
            {
                // Get all user unrated events
                var unratedEvents = similarity.User.Ratings.Select(r => r.Event)
                                                          .Except(user.Ratings.Select(r => r.Event));
                // Recommended event by one similarity
                var recommendedEvents = unratedEvents.OrderByDescending(e => similarity.User.Ratings.First(r => r.Event == e).RatingNumber)
                                                   .Take(5);
                recommendations.AddRange(recommendedEvents);
            }
            return recommendations.Distinct();
        }
    }
}
