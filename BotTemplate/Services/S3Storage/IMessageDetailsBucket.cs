using Telegram.Bot.Types;

namespace BotTemplate.Services.S3Storage;

public interface IMessageDetailsBucket
{
    Task<List<Message>> GetMessages(long chatId);
    Task AddMessage(long chatId, Message message);
}