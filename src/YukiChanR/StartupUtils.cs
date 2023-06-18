using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;
using Serilog;
using YukiChanR.Core;

namespace YukiChanR;

public static partial class StartupUtils
{
    [GeneratedRegex(@"(?<=Configurations\.).*(?=\.yml)")]
    private static partial Regex ConfigurationNameRegex();

    public static void PrepareConfigurations(this ConfigurationManager configuration,
        IHostEnvironment environment, string[] args)
    {
        configuration.Sources.Clear();

        Directory.CreateDirectory(YukiDirectories.Configs);

        var assembly = typeof(Program).Assembly;
        var configResources = assembly
            .GetManifestResourceNames()
            .Where(name => name.Contains("Configurations"));
        var configNames = new List<string>();

        foreach (var res in configResources)
        {
            var match = ConfigurationNameRegex().Match(res);
            if (!match.Success) continue;

            var name = match.Value;
            configNames.Add(name);
            var configPath = Path.Combine(YukiDirectories.Configs, $"{name}.yml");

            if (File.Exists(configPath)) continue;

            var stream = assembly.GetManifestResourceStream(res)!;
            var fs = File.OpenWrite(configPath);
            stream.CopyTo(fs);
            fs.Close();
        }

        foreach (var configName in configNames)
            configuration.AddYamlFile(
                Path.Combine(YukiDirectories.Configs, $"{configName}.yml"),
                true, true);
        foreach (var configName in configNames)
            configuration.AddYamlFile(
                Path.Combine(YukiDirectories.Configs, $"{configName}.{environment.EnvironmentName}.yml"),
                true, true);

        configuration.AddEnvironmentVariables();
        configuration.AddCommandLine(args);
    }

    public static void ConfigureAndAddSerilog(this ILoggingBuilder logging, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        logging.ClearProviders();
        logging.AddSerilog(dispose: true);
    }

    public static void InitializeSentry(this IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration["Sentry:Dsn"]))
            return;

        SentrySdk.Init(options => configuration.GetSection("Sentry").Bind(options));
    }
}