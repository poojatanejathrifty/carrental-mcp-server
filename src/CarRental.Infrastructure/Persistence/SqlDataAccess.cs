using System.Data;
using System.Text.Json;
using CarRental.Infrastructure.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarRental.Infrastructure.Persistence;

/// <summary>
/// Single, centralized entry point for all stored-procedure database access.
/// No repository opens a SqlConnection directly - every call flows through here.
/// </summary>
public class SqlDataAccess(IOptions<DatabaseOptions> options, ILogger<SqlDataAccess> logger) : ISqlDataAccess
{
    private readonly DatabaseOptions _options = options.Value;

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await CreateOpenSqlConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, cancellationToken);

        return await LogAndExecute(storedProcedure, parameters, () => connection.QuerySingleOrDefaultAsync<T>(command));
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await CreateOpenSqlConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, cancellationToken);

        var result = await LogAndExecute(storedProcedure, parameters, () => connection.QueryAsync<T>(command));
        return result.AsList();
    }

    public async Task<int> ExecuteAsync(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await CreateOpenSqlConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, cancellationToken);

        return await LogAndExecute(storedProcedure, parameters, () => connection.ExecuteAsync(command));
    }

    public async Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await CreateOpenSqlConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, cancellationToken);

        return await LogAndExecute(storedProcedure, parameters, () => connection.ExecuteScalarAsync<T>(command));
    }

    public async Task<(IReadOnlyList<TFirst> First, IReadOnlyList<TSecond> Second)> QueryMultipleAsync<TFirst, TSecond>(
        string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
    {
        using var connection = await CreateOpenSqlConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, cancellationToken);

        return await LogAndExecute(storedProcedure, parameters, async () =>
        {
            using var multi = await connection.QueryMultipleAsync(command);
            var first = (await multi.ReadAsync<TFirst>()).AsList();
            var second = (await multi.ReadAsync<TSecond>()).AsList();
            return ((IReadOnlyList<TFirst>)first, (IReadOnlyList<TSecond>)second);
        });
    }

    public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
        => await CreateOpenSqlConnectionAsync(cancellationToken);

    private async Task<SqlConnection> CreateOpenSqlConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    private CommandDefinition BuildCommand(string storedProcedure, object? parameters, CancellationToken cancellationToken)
        => new(storedProcedure, parameters, commandType: CommandType.StoredProcedure,
            commandTimeout: _options.CommandTimeoutSeconds, cancellationToken: cancellationToken);

    private async Task<T> LogAndExecute<T>(string storedProcedure, object? parameters, Func<Task<T>> action)
    {
        logger.LogInformation("Calling stored procedure {StoredProcedure} with parameters {Parameters}",
            storedProcedure, JsonSerializer.Serialize(parameters));

        try
        {
            return await action();
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "SQL execution failed for stored procedure {StoredProcedure}", storedProcedure);
            throw;
        }
    }
}
