using System;
using BotTemplate;
using NUnit.Framework;
using BotTemplate.Services.YDB;
using Moq;

namespace Tests;


public static class Db
{
    public static readonly Mock<MessageDateRepo> TestTable;
    
    static Db()
    {
        var conf = Configuration.FromJson("testsettings.json");
        TestTable = new Mock<MessageDateRepo>(new BotDatabase(conf));
        TestTable.SetupGet(repo => repo.TableName)
            .Returns("test_table");
    }
}

public class Tests
{
    private MessageDateRepo db => Db.TestTable.Object;
    
    [SetUp]
    public void Setup()
    {
        Assert.That(db.TableName == "test_table");
        db.CreateTable().Wait();
    }

    [TearDown]
    public void TearDown()
    {
        db.ClearAll().Wait();
    }
    
    [Test]
    public void Test_NoPeople_OnEmptyDatabase()
    {
        Assert.That(
            db.GetPastWeekUsersCount().Result,
            Is.Zero
        );
    }

    [Test]
    public void Test_NullDatetime_OnFirstQuery()
    {
        Assert.That(
            db.FindLastMessageDateTime(228).Result,
            Is.Null
        );
    }

    [Test]
    public void Test_CountGrows_OnNowInsertion()
    {
        for (var i = 0; i < 10; ++i)
        {
            Assert.That(db.GetPastWeekUsersCount().Result, Is.EqualTo(i));
            db.UpdateOrInsertDateTime(i);
        }
    }

    [Test]
    public void Test_CountStaysZero_OnOldMessageInsertion()
    {
        for (var i = 0; i < 10; ++i)
        {
            db.UpdateOrInsertDateTime(i, DateTime.Now - TimeSpan.FromDays(8));
            Assert.That(db.GetPastWeekUsersCount().Result, Is.Zero);
        }
    }

    /// <summary>
    /// Warning!
    /// This test was added purely for fun and
    /// it is based on the assumption that one millisecond earlier than
    /// last monday would almost certainly be included if conditions were incorrect. 
    /// </summary>
    [Test]
    public void Test_CountIncrementsOnlyOnCurrentWeek()
    {
        var lastMonday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
        Assert.That(lastMonday.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        
        db.UpdateOrInsertDateTime(1, lastMonday);
        Assert.That(db.GetPastWeekUsersCount().Result, Is.EqualTo(1));
        
        db.UpdateOrInsertDateTime(1, lastMonday - TimeSpan.FromMilliseconds(1));
        Assert.That(db.GetPastWeekUsersCount().Result, Is.Zero);
        
        var timespanThreshold = new TimeSpan(6, 23, 59, 59, 999);
        
        db.UpdateOrInsertDateTime(1, DateTime.Now - timespanThreshold);
        Assert.That(db.GetPastWeekUsersCount().Result, Is.Zero);
    }
}