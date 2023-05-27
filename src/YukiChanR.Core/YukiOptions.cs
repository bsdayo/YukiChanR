using System.Text;
using Sentry;

namespace YukiChanR.Core;

public sealed class YukiOptions
{
    public YukiDatabaseOptions Database { get; set; } = new();

    public SentryOptions Sentry { get; set; } = new();
}

public sealed class YukiDatabaseOptions
{
    public string Host { get; set; } = "127.0.0.1";

    public int Port { get; set; } = 5432;

    public string Username { get; set; } = "postgres";

    public string Password { get; set; } = "";

    public string Database { get; set; } = "postgres";

    private string? _connectionString;

    public string GetConnectionString()
    {
        if (_connectionString is not null)
            return _connectionString;

        return _connectionString = new StringBuilder()
            .Append($"Host={Host};")
            .Append($"Port={Port};")
            .Append($"Database={Database};")
            .Append($"Username={Username};")
            .Append(string.IsNullOrWhiteSpace(Password)
                ? ""
                : $"Password={Password};")
            .ToString();
    }
}