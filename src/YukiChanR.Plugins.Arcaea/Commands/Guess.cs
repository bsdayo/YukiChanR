using Flandre.Core.Common;
using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnofficialArcaeaAPI.Lib.Models;
using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Entities;
using YukiChanR.Plugins.Arcaea.Models;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    private class ArcaeaGuessSession
    {
        public long Timestamp { get; set; }
        public ArcaeaSongDbChart Chart { get; set; } = null!;
        public byte[] Cover { get; set; } = null!;
        public bool IsReady { get; set; }
    }

    private static readonly Dictionary<string, ArcaeaGuessSession> GuessSessions = new();

    [Command("a.guess")]
    [StringShortcut("猜曲绘", AllowArguments = true)]
    public async Task<MessageContent> OnGuess(MessageContext ctx, params string[] guessOrModeArg)
    {
        var guessOrMode = string.Join(' ', guessOrModeArg);
        var sessionId = ctx.Message.Environment == MessageEnvironment.Channel
            ? $"{ctx.Platform}:{ctx.Message.ChannelId}"
            : $"{ctx.Platform}:private:{ctx.UserId}";

        if (GuessSessions.TryGetValue(sessionId, out var session))
        {
            if (string.IsNullOrWhiteSpace(guessOrMode))
                return ctx.Reply(_localizer["Guess:GameRunning"]);

            if (!session.IsReady)
                return ctx.Reply(_localizer["Guess:GameInitializing"]);

            var guessSongId = await _songDb.SearchIdAsync(guessOrMode);
            if (guessSongId is null)
                return ctx.Reply(_localizer["Common:SongNotFound"]);

            // 判断是否猜对
            if (guessSongId != session.Chart.SongId)
                return ctx.Reply(_localizer["Guess:WrongGuess"]);

            GuessSessions.Remove(sessionId);

            var set = await _songDb.Packages
                .AsNoTracking()
                .FirstAsync(package => package.Set == session.Chart.Set);
            return ctx.Reply(_localizer["Guess:CorrectGuess"])
                .Image(session.Cover)
                .Text(_localizer["Guess:GuessAnswer", session.Chart.NameEn, session.Chart.Artist, set.Name]);
        }

        var mode = string.IsNullOrWhiteSpace(guessOrMode)
            ? ArcaeaGuessMode.Normal
            : ArcaeaUtils.GetGuessMode(guessOrMode);

        if (mode is null)
            return ctx.Reply(_localizer["Guess:GameNotStarted"]);

        return await StartNewGuess(sessionId, ctx, mode.Value);
    }

    private async Task<MessageBuilder> StartNewGuess(string sessionId, MessageContext ctx, ArcaeaGuessMode mode)
    {
        var timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();

        // 占位
        GuessSessions.TryAdd(sessionId,
            new ArcaeaGuessSession { Timestamp = timestamp });

        var allCharts = await _songDb.Charts
            .Where(chart => chart.RatingClass == (int)ArcaeaDifficulty.Future)
            .ToArrayAsync();
        var randomChart = allCharts[new Random().Next(allCharts.Length)];
        var cover = await _cacheManager.GetSongCoverAsync(
            randomChart.SongId, randomChart.JacketOverride,
            (ArcaeaDifficulty)randomChart.RatingClass);

        GuessSessions[sessionId].Chart = randomChart;
        GuessSessions[sessionId].Cover = cover;
        GuessSessions[sessionId].IsReady = true;

        _logger.LogDebug("{SessionId} 启动了新的猜曲绘会话：{ModeName} => {SongNameEn} ({SongId})",
            sessionId, mode.GetTitle(), randomChart.NameEn, randomChart.SongId);

        // 超时揭晓答案
#pragma warning disable CS4014
        Task.Run(async () =>
        {
            await Task.Delay(_options.GuessTime);
            if (!GuessSessions.TryGetValue(sessionId, out var session)) return;
            if (session.Timestamp != timestamp) return;
            GuessSessions.Remove(sessionId);
            var set = await _songDb.Packages
                .AsNoTracking()
                .FirstAsync(package => package.Set == session.Chart.Set);
            ctx.Bot.SendMessageAsync(ctx.Message,
                new MessageBuilder()
                    .Text(_localizer["Guess:GameTimeEnd"])
                    .Image(session.Cover)
                    .Text(_localizer["Guess:GuessAnswer", session.Chart.NameEn, session.Chart.Artist, set.Name]));
        });
#pragma warning restore CS4014

        var image = _imageGen.GenerateGuess(cover, mode);

        return new MessageBuilder()
            .Text(_localizer["Guess:GameTitle", mode.GetTitle()])
            .Image(image)
            .Text(_localizer["Guess:GameTime", _options.GuessTime]);
    }
}