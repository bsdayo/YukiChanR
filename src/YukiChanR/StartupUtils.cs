using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;
using Serilog;
using Tomlyn.Extensions.Configuration;
using YukiChanR.Core;

namespace YukiChanR;

public static class StartupUtils
{
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

        foreach (var name in configResources)
        {
            var configName = name.Split('.')[^2];
            var configPath = Path.Combine(YukiDirectories.Configs, $"{configName}.toml");
            configNames.Add(configName);

            if (File.Exists(configPath)) continue;

            var stream = assembly.GetManifestResourceStream(name)!;
            var fs = File.OpenWrite(configPath);
            stream.CopyTo(fs);
            fs.Close();
        }

        foreach (var configName in configNames)
            configuration.AddTomlFile(
                Path.Combine(YukiDirectories.Configs, $"{configName}.toml"),
                true, true);
        foreach (var configName in configNames)
            configuration.AddTomlFile(
                Path.Combine(YukiDirectories.Configs, $"{configName}.{environment.EnvironmentName}.toml"),
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