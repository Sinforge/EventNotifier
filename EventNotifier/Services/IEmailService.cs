namespace EventNotifier.Services
{
    public interface IEmailService
    {
        Task SendMessageAsync(string emailName,string title, string htmlMessage);
    }
}
