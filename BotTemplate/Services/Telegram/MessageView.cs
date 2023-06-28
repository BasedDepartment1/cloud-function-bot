using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace BotTemplate.Services.Telegram;

public interface IMessageView
{
    Task Say(string text, long chatId);
    Task ShowHelp(long chatId);
    Task SendFile(long chatId, byte[] content, string filename, string caption);
    Task SendPicture(long chatId, byte[] picture, string caption);
}

public class HtmlMessageView : IMessageView
{
    private readonly ITelegramBotClient botClient;

    public HtmlMessageView(ITelegramBotClient client)
    {
        botClient = client;
    }


    public async Task Say(string text, long chatId)
    {
        await botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: ParseMode.Html
        );
    }

    public async Task ShowHelp(long chatId)
    {
        await Say(
            "Это шаблон telegram-бота, поддерживающий <b>Yandex Cloud Function</b>!\n" +
                "Если он тебе нужен, то тогда тебе <a href=\"https://google.com\">сюда</a>.",
            chatId
        );
    }

    public async Task SendFile(long chatId, byte[] content, string filename, string caption)
    {
        await botClient.SendDocumentAsync(
            chatId,
            InputFile.FromStream(new MemoryStream(content), filename),
            caption: EscapeForHtml(caption)
        );
    }

    public async Task SendPicture(long chatId, byte[] picture, string caption)
    {
        await botClient.SendPhotoAsync(
            chatId,
            InputFile.FromStream(new MemoryStream(picture)),
            caption: EscapeForHtml(caption)
        );
    }
    
    private static string EscapeForHtml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
    }
}