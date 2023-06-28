using Telegram.Bot.Types;

namespace BotTemplate.Services.Telegram.Commands;

public interface IChatCommandHandler
{
    string Command { get; }
    
    Task HandlePlainText(string text, long fromChatId);
}