using System.Net;
using AspNetCore.Yandex.ObjectStorage;
using AspNetCore.Yandex.ObjectStorage.Object;
using FluentResults;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace BotTemplate.Services.S3Storage;


public class S3ApiException : Exception
{
    public S3ApiException(string message)
        : base(message) { }
    
    public S3ApiException(string message, Exception innerException)
        : base(message, innerException: innerException) { }

    public static S3ApiException FromResult<T>(Result<T> result, HttpMethod method, string path)
    {
        return new S3ApiException(
            $"Error during {method.Method} {path}: " + 
            $"{string.Join('\n', result.Errors.Select(e => e.Message))}"
        );
    }
}


public class S3MessageDetailsBucket : IMessageDetailsBucket
{
    private readonly IObjectService objectService;
    public S3MessageDetailsBucket(YandexStorageService storageService)
    {
        objectService = storageService.ObjectService;
    }
    
    public async Task<List<Message>> GetMessages(long chatId)
    {
        var filename = GetFilename(chatId);
        var response = await objectService.GetAsync(filename);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new List<Message>();
        }

        if (!response.IsSuccessStatusCode)
        {
            throw S3ApiException.FromResult(
                response.ToResult(),
                HttpMethod.Get,
                filename
            );
        }
        var body = await response.ReadAsByteArrayAsync();
        
        var content = System.Text.Encoding.UTF8.GetString(body.Value);
        return JsonConvert.DeserializeObject<List<Message>>(content)!;
    }

    public async Task AddMessage(long chatId, Message message)
    {
        var json = await GetMessages(chatId);
        json.Add(message);
        var jsonString = JsonConvert.SerializeObject(json);
        var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

        var filename = GetFilename(chatId);
        var response = await objectService.PutAsync(jsonBytes, filename);

        if (!response.IsSuccessStatusCode)
        {
            throw S3ApiException.FromResult(
                response.ToResult(),
                HttpMethod.Put,
                filename
            );
        }
    }

    private static string GetFilename(long chatId)
    {
        return $"{chatId}.json";
    }
}