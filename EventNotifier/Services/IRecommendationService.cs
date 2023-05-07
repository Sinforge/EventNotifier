using EventNotifier.Models;

namespace EventNotifier.Services
{
    // Collaborative recommendation 
    public interface IRecommendationService
    {
        double ComputeSimilarity(User user1, User user2);

        IEnumerable<Event> GetRecommendation(User user);
    }
}