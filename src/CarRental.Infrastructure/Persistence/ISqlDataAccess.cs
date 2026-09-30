using System.Data;

namespace CarRental.Infrastructure.Persistence;

public interface ISqlDataAccess
{
    Task<T?> QuerySingleOrDefaultAsync<T>(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> QueryAsync<T>(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default);
    Task<int> ExecuteAsync(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default);
    Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<TFirst> First, IReadOnlyList<TSecond> Second)> QueryMultipleAsync<TFirst, TSecond>(
        string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default);

    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
