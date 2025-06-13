namespace gym.Services
{
    public interface IChatBotService
    {
        Task<string> GetAnswerAsync(string message);
    }
}
