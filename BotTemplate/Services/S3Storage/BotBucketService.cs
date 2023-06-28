using AspNetCore.Yandex.ObjectStorage;

namespace BotTemplate.Services.S3Storage;

public static class BotBucketService
{
    public static YandexStorageService CreateBotBucketService(this Configuration configuration)
    {
        return new YandexStorageService(configuration.YandexStorageOptions);
    }
}