using Ydb.Sdk;
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

    public async Task ExecuteModify(
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
        var metadataProvider = new MetadataProvider();
        await metadataProvider.Initialize();
        
        var config = new DriverConfig(
            configuration.YbEndpoint,
            configuration.YdbPath,
            metadataProvider
        );
        
        var driver = new Driver(config);
        await driver.Initialize();

        return new TableClient(driver, new TableClientConfig());
    }
}