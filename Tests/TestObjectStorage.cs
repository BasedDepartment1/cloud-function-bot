using System;
using System.Linq;
using AspNetCore.Yandex.ObjectStorage;
using BotTemplate;
using BotTemplate.Services.S3Storage;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace Tests;

public static class ObjectStorage
{
    public static S3MessageDetailsBucket bucket;

    static ObjectStorage()
    {
        var config = Configuration.FromJson("testsettings.json");
        
        bucket = new S3MessageDetailsBucket(
            new YandexStorageService(config.YandexStorageOptions)
        );
    }
}


public class TestObjectStorage
{
    private S3MessageDetailsBucket s3Bucket => ObjectStorage.bucket;

    [Test]
    public void Test_ListIsEmpty_OnUnexistentObject()
    {
        var response = s3Bucket.GetMessages(-1).Result;
        Assert.That(response, Is.Empty);
        s3Bucket.ClearChatMessages(-1).Wait();
    }

    [Test]
    public void Test_ListHasOneElement_OnInsertIntoEmpty()
    {
        var originalMessage = new Message {Text = "Hello", Date = DateTime.Now, Chat = new Chat()};
        s3Bucket.AddMessage(-2, originalMessage).Wait();

        var messagesFromBucket = s3Bucket.GetMessages(-2).Result;
        
        Assert.That(messagesFromBucket.Count, Is.EqualTo(1));
        var receivedMessage = messagesFromBucket.First();
        
        Assert.That(receivedMessage.Text, Is.EqualTo(originalMessage.Text));
        s3Bucket.ClearChatMessages(-2).Wait();
    }
}