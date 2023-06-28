using BotTemplate.Services.S3Storage;
using BotTemplate.Services.Telegram;
using BotTemplate.Services.Telegram.Commands;
using BotTemplate.Services.YDB;
using Grpc.Core.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yandex.Cloud.Functions;

namespace BotTemplate;

public class Response
{
    public int StatusCode { get; set; }
    public string Body { get; set; }

    public Response(int statusCode, string body)
    {
        StatusCode = statusCode;
        Body = body;
    }
}

public class TelegramHandler : YcFunction<string, Response>
{
    private const string CodePath = "/function/code/";
    
    public Response FunctionHandler(string request, Context context)
    {
        var logger = new ConsoleLogger();
        logger.ForType<string>().Info(request);
        try
        {
            HandleRequest(request, context).Wait();
            return new Response(200, "ok");
        }
        catch (Exception e)
        {
            logger.ForType<Exception>().Error(e, "Error");
            return new Response(500, $"Error {e}");
        }
    }

    private async Task HandleRequest(string request, Context context)
    {
        var configuration = Configuration.FromJson(CodePath + "settings.json");
        var tgClient = new TelegramBotClient(configuration.TelegramToken);
        var body = JObject.Parse(request).GetValue("body")!.Value<string>()!;
        var update = JsonConvert.DeserializeObject<Update>(body)!;
        var view = new HtmlMessageView(tgClient);
        var messagesRepo = new S3MessageDetailsBucket(configuration.CreateBotBucketService());
        var botDatabase = new BotDatabase(configuration);

        var commands = new IChatCommandHandler[]
        {
            new StartCommandHandler(view),
            new HelpCommandHandler(view),
        };

        var updateService = new HandleUpdateService(view, commands, messagesRepo, botDatabase);
        await updateService.Handle(update);
    }
}

