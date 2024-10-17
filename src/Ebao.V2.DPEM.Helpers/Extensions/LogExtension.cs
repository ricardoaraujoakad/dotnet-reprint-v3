using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Helpers.Extensions;

public static class LogExtensions
{
    /// <summary>
    /// Cria um novo escopo de log
    /// </summary>
    /// <param name="logger">Instance do log.</param>
    /// <param name="key">Nome da chave do escopo.</param>
    /// <param name="value">Valor do escopo.</param>
    /// <returns>Um <see cref="IDisposable"/> do escopo criado.</returns>
    public static IDisposable BeginScope(this ILogger logger, string key, object value)
    {
        return logger.BeginScope(new Dictionary<string, object> { { key, value } });
    }

    public static IDisposable BeginNamedScope(this ILogger logger,
        string name, params ValueTuple<string, object>[] properties)
    {
        var dictionary = properties.ToDictionary(p => p.Item1, p => p.Item2);
        dictionary[name + ".Scope"] = Guid.NewGuid();
        return logger.BeginScope(dictionary);
    }

    public static IDisposable BeginPropertyScope(this ILogger logger,
        params ValueTuple<string, object>[] properties)
    {
        var dictionary = properties.ToDictionary(p => p.Item1, p => p.Item2);
        return logger.BeginScope(dictionary);
    }
}