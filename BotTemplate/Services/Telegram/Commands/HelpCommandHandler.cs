namespace BotTemplate.Services.Telegram.Commands;

public class HelpCommandHandler : IChatCommandHandler
{
    public string Command => "/help";
    private readonly IMessageView messageView;
    
    public HelpCommandHandler(IMessageView view)
    {
        messageView = view;
    }
    
    public async Task HandlePlainText(string text, long fromChatId)
    {
        await messageView.ShowHelp(fromChatId);
    }
}