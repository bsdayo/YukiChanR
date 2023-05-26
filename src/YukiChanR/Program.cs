using Flandre.Adapters.OneBot.Extensions;
using Flandre.Framework;
using Flandre.Framework.Extensions;
using Flandre.Framework.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YukiChanR;
using YukiChanR.Core;
using YukiChanR.Plugins.Arcaea;

var builder = FlandreApp.CreateBuilder();

builder.Configuration.PrepareConfigurations(builder.Environment, args);
builder.Services.ConfigureFlandreApp(builder.Configuration);
builder.Services.Configure<YukiOptions>(builder.Configuration);

builder.Logging.ConfigureAndAddSerilog(builder.Configuration);

builder.Adapters.AddOneBot();

builder.Plugins.AddArcaea();

var app = builder.Build();

app.UseExceptionFeedback();
app.UseCommandSession();
app.UseCommandParser();
app.UseCommandInvoker();

app.MapCommand("hello", () => "world!");

app.Run();