using Flandre.Adapters.OneBot.Extensions;
using Flandre.Framework;
using Flandre.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YukiChanR;
using YukiChanR.Core;
using YukiChanR.Plugins.Arcaea;
using YukiChanR.Plugins.Monitor;

MonitorPlugin.UptimeStopwatch.Start();

var builder = FlandreApp.CreateBuilder();

builder.Configuration.PrepareConfigurations(builder.Environment, args);
builder.Services.ConfigureFlandreApp(builder.Configuration);
builder.Services.Configure<YukiOptions>(builder.Configuration);

builder.Configuration.InitializeSentry();

builder.Logging.ConfigureAndAddSerilog(builder.Configuration);

builder.Adapters.AddOneBot();

builder.Plugins.AddArcaea();
builder.Plugins.AddMonitor();

var app = builder.Build();

app.UseExceptionFeedback();
app.UseCommandSession();
app.UseCommandParser();
app.UseCommandInvoker();

using (var scope = app.Services.CreateScope())
{
    void Migrate<T>() where T : YukiDbContext
    {
        var db = scope.ServiceProvider.GetRequiredService<T>();
        db.Database.Migrate();
    }

    Migrate<ArcaeaDbContext>();
}

app.Run();