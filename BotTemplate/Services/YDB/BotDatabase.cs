using System.Collections.Generic;
using Ydb.Sdk;
using Ydb.Sdk.Auth;
using Ydb.Sdk.Table;
using Ydb.Sdk.Value;
using Ydb.Sdk.Yc;

namespace BotTemplate.Services.YDB;

public class BotDatabase : IBotDatabase
{
    private readonly Configuration configuration;

    public BotDatabase(Configuration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<ResultSet.Row>?> ExecuteFind(
        string query, Dictionary<string, YdbValue> parameters)
    {
        using var tableClient = await CreateTableClient();
        
        var response = await tableClient.SessionExec(async session 
            => await session.ExecuteDataQuery(
                query, 
                TxControl.BeginSerializableRW().Commit(), 
                parameters)
        );
        
        response.Status.EnsureSuccess();
        var queryResponse = (ExecuteDataQueryResponse) response;
    
        return queryResponse.Result.ResultSets.Count == 0 
            ? null 
            : queryResponse.Result.ResultSets[0].Rows;
        
    }

    public async Task ExecuteModify(string query, Dictionary<string, YdbValue> parameters)
    {
        using var tableClient = await CreateTableClient();
        
        var response = await tableClient.SessionExec(async session
            => await session.ExecuteDataQuery(
                query,
                TxControl.BeginSerializableRW().Commit(),
                parameters)
        );

        response.Status.EnsureSuccess();
    }

    public async Task ExecuteScheme(string query)
    {
        using var tableClient = await CreateTableClient();

        var response = await tableClient.SessionExec(async session
            => await session.ExecuteSchemeQuery(query)
        );

        response.Status.EnsureSuccess();
    }

    private async Task<TableClient> CreateTableClient()
    {
        ICredentialsProvider provider;

        if (configuration.IamTokenPath is null)
        {
            provider = new MetadataProvider();
            await ((MetadataProvider) provider).Initialize();
        }
        else
        {
            provider = new ServiceAccountProvider(configuration.IamTokenPath);
        }

        var config = new DriverConfig(
            configuration.YdbEndpoint,
            configuration.YdbPath,
            provider
        );
        
        var driver = new Driver(config);
        await driver.Initialize();

        return new TableClient(driver, new TableClientConfig());
    }
}