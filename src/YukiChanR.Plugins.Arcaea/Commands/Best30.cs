using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnofficialArcaeaAPI.Lib;
using UnofficialArcaeaAPI.Lib.Models;
using UnofficialArcaeaAPI.Lib.Responses;
using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Entities;
using YukiChanR.Plugins.Arcaea.Models;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    private static readonly List<string> Best30PollingTargets = new();

    [Command("a.b30")]
    [StringShortcut("查b30", AllowArguments = true)]
    [StringShortcut("查B30", AllowArguments = true)]
    public async Task<MessageContent?> OnBest30(MessageContext ctx,
        [Option(ShortName = 'n')] bool nya,
        [Option(ShortName = 'd')] bool dark,
        string? user = null)
    {
        var b30Localizer = _localizer.GetSection("Best30");

        string target;
        string logTarget;

        if (string.IsNullOrEmpty(user))
        {
            var binding = await _database.Users.AsNoTracking().FirstOrDefaultAsync(
                u => u.Platform == ctx.Platform && u.UserId == ctx.UserId);
            if (binding is null)
                return ctx.Reply(_commonLocalizer["UserNotBound", "/a b30 用户名或好友码"]);
            target = binding.ArcaeaCode;
            logTarget = $"{binding.ArcaeaName} <{binding.ArcaeaCode}>";
        }
        else
        {
            logTarget = target = user;
        }

        _logger.LogInformation("[Best30:Query] {Target}", logTarget);

        if (Best30PollingTargets.Contains(target))
            return ctx.Reply(b30Localizer["SessionAlreadyStarted"]);
        Best30PollingTargets.Add(target);

        try
        {
            var session = await _uaaService.UaaClient.User.GetBestsSessionAsync(target);

            if (session.IsCacheSession)
                await ctx.Bot.SendMessageAsync(ctx.Message, ctx.Reply(b30Localizer["SessionCached"].ToString()));
            
            UaaUserBestsResultContent result;
            var retries = 0;

            while (true)
            {
                try
                {
                    result = await _uaaService.UaaClient.User.GetBestsResultAsync(session.SessionInfo, 9,
                        UaaReplyWith.All);
                    break;
                }
                catch (UaaBestsSessionPendingException pendingEx)
                {
                    if (retries == 0)
                    {
                        if (pendingEx.IsWaitingForAccount)
                        {
                            await ctx.Bot.SendMessageAsync(ctx.Message, ctx.Reply(
                                b30Localizer["SessionWaitingForAccount", 5 - pendingEx.CurrentAccount].ToString()));
                        }
                        else if (pendingEx.IsQuerying)
                        {
                            await ctx.Bot.SendMessageAsync(ctx.Message, ctx.Reply(
                                b30Localizer["SessionQuerying", pendingEx.QueriedCharts].ToString()));
                        }
                    }
                }

                retries++;
                _logger.LogDebug("[Best30:Polling] {Target}   Retries: {Retries}", logTarget, retries);
                await Task.Delay(_options.Best30PollingInterval);
            }

            var pref = await _database.Preferences.AsNoTracking().FirstOrDefaultAsync(
                           p => p.Platform == ctx.Platform && p.UserId == ctx.UserId)
                       ?? new ArcaeaUserPreferences();
            var b30 = ArcaeaBest30.FromUaa(result);

            pref.Nya = pref.Nya || nya;
            pref.Dark = pref.Dark || dark;

            _logger.LogInformation("[Best30:ImageGen] {Target}", logTarget);

            var image = await _imageGen.GenerateBest30Async(b30, pref);

            var builder = ctx
                .Reply(b30Localizer["Reply", b30.User.Name, ArcaeaUtils.ToDisplayPotential(b30.User.Potential)])
                .Image(image);

            if (session.IsCacheSession)
            {
                var queryTime = DateTimeOffset.FromUnixTimeMilliseconds(result.QueryTime);
                var refreshTime = queryTime.AddHours(12) - DateTimeOffset.UtcNow;

                if (refreshTime.TotalMilliseconds > 0)
                    builder.Text(b30Localizer["SessionNextRefresh", refreshTime]);
            }

            return builder.Build();
        }
        finally
        {
            Best30PollingTargets.Remove(target);
        }
    }
}