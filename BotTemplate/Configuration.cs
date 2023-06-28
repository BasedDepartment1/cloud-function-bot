using AspNetCore.Yandex.ObjectStorage.Configuration;
using Microsoft.Extensions.Configuration;

namespace BotTemplate;

public class Configuration
{
    public YandexStorageOptions YandexStorageOptions { get; }
    public string TelegramToken => appSettings[nameof(TelegramToken)]!;
    public string YbEndpoint => appSettings[nameof(YbEndpoint)]!;
    public string YdbPath => appSettings[nameof(YdbPath)]!;

    private readonly IConfigurationSection appSettings;

    public Configuration()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("/function/code/settings.json", optional: false, reloadOnChange: true)
            .Build();
        appSettings = configuration.GetSection("AppSettings");
        YandexStorageOptions = configuration.GetYandexStorageOptions("CloudStorageOptions");
    }
}
