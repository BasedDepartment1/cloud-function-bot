using AspNetCore.Yandex.ObjectStorage.Configuration;
using Microsoft.Extensions.Configuration;

namespace BotTemplate;

public class Configuration
{
    public YandexStorageOptions YandexStorageOptions { get; }
    public string TelegramToken => appSettings[nameof(TelegramToken)]!;
    public string YdbEndpoint => appSettings[nameof(YdbEndpoint)]!;
    public string YdbPath => appSettings[nameof(YdbPath)]!;
    public string? IamTokenPath => appSettings[nameof(IamTokenPath)];

    private readonly IConfigurationSection appSettings;

    public Configuration(IConfigurationSection appSettings, YandexStorageOptions cloudStorageOptions)
    {
        this.appSettings = appSettings;
        YandexStorageOptions = cloudStorageOptions;
    }

    public static Configuration FromJson(string path)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(path, optional: false, reloadOnChange: true)
            .Build();

        return new Configuration(
            configuration.GetSection("AppSettings"),
            configuration.GetYandexStorageOptions("CloudStorageOptions")
        );
    }
}
