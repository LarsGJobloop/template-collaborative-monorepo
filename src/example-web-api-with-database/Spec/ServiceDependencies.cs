using Testcontainers.PostgreSql;

namespace Spec;

public class ServiceDependencies : IAsyncDisposable
{
    private PostgreSqlContainer _databaseContainer = null!;

    public async Task InitializePostgreSQL()
    {
        var builder = new PostgreSqlBuilder("postgres:17");
        _databaseContainer = builder.Build();
        await _databaseContainer.StartAsync();
    }

    public async Task ResetPostgreSQL()
    {
        await _databaseContainer.StopAsync();
        await _databaseContainer.StartAsync();
    }

    public string GetDatabaseConnectionString()
    {
        return _databaseContainer.GetConnectionString();
    }

    public async ValueTask DisposeAsync()
    {
        await _databaseContainer.StopAsync();
        await _databaseContainer.DisposeAsync();
    }
}
