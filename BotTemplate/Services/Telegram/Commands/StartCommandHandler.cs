namespace BotTemplate.Services.Telegram.Commands;

public class StartCommandHandler : IChatCommandHandler
{
    public string Command => "/start";
    
    private readonly IMessageView messageView;

    public StartCommandHandler(IMessageView view)
    {
        messageView = view;
    }

    public async Task HandlePlainText(string text, long fromChatId)
    {
        await messageView.ShowHelp(fromChatId);
    }
}