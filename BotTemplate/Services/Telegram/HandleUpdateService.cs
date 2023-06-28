using BotTemplate.Services.S3Storage;
using BotTemplate.Services.Telegram.Commands;
using BotTemplate.Services.YDB;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotTemplate.Services.Telegram;

public class HandleUpdateService
{
    private readonly IMessageView messageView;
    private readonly IChatCommandHandler[] commands;
    private readonly IMessageDetailsBucket messageDetailsBucket;
    private readonly IBotDatabase botDatabase;

    public HandleUpdateService(
        IMessageView messageView, 
        IChatCommandHandler[] commands,
        IMessageDetailsBucket messageDetailsBucket,
        IBotDatabase botDatabase)
    {
        this.messageView = messageView;
        this.commands = commands;
        this.messageDetailsBucket = messageDetailsBucket;
        this.botDatabase = botDatabase;
    }

    public async Task Handle(Update update)
    {
        var messageDateTable = await MessageDateRepo.InitWithDatabase(botDatabase);
        var handler = update.Type switch
        {
            UpdateType.Message => HandleMessage(update.Message!, messageDateTable),
            _ => HandleDefaultUpdate()
        };

        await handler;
    }

    private async Task HandleMessage(Message message, MessageDateRepo messageDateRepo)
    {
        await messageDetailsBucket.AddMessage(message.Chat.Id, message);
        if (message.Type == MessageType.Text)
        {
            await HandlePlainText(message.Text!, message.Chat.Id, messageDateRepo);
            messageDateRepo.UpdateOrInsertDateTime(message.Chat.Id);
            return;
        }

        await HandleNonCommandMessage(message.Chat.Id, messageDateRepo);
        messageDateRepo.UpdateOrInsertDateTime(message.Chat.Id);
    }

    private async Task HandlePlainText(string text, long fromChatId, MessageDateRepo messageDateRepo)
    {
        var command = commands.FirstOrDefault(c => text.StartsWith(c.Command));

        if (command is null)
        {
            await HandleNonCommandMessage(fromChatId, messageDateRepo);
            return;
        }

        await command.HandlePlainText(text, fromChatId);
    }

    private async Task HandleNonCommandMessage(long fromChatId, MessageDateRepo messageDateRepo)
    {
        await messageView.Say(
            await GetGreetingMessage(fromChatId, messageDateRepo),
            fromChatId
        );
    }
    
    private Task HandleDefaultUpdate()
    {
        return Task.CompletedTask;
    }

    private async Task<string> GetGreetingMessage(long fromChatId, MessageDateRepo messageDateRepo)
    {
        var lastMessageDateTime = await messageDateRepo.FindLastMessageDateTime(fromChatId);
        var separationInterval = DateTime.Now - lastMessageDateTime;
        var usersOverWeek = await messageDateRepo.GetPastWeekUsersCount();

        var peopleCountPlural = ((long) usersOverWeek).PluralizeLong(
            "человеком|людьми|людьми"
        );

        var dayCountPlural = separationInterval is null
            ? "никогда"
            : separationInterval.Value.Days.Pluralize(
                "день|дня|дней"
            );

        return $"Давно не виделись! А именно {dayCountPlural}! " +
               $"Я пообщался уже с {peopleCountPlural} за эту неделю.";
    }
}