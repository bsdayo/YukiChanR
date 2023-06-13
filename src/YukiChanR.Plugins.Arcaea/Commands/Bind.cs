using Flandre.Core.Messaging;
using Flandre.Framework.Routing;
using Microsoft.EntityFrameworkCore;
using UnofficialArcaeaAPI.Lib;
using UnofficialArcaeaAPI.Lib.Responses;
using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Entities;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.bind")]
    public async Task<MessageContent> OnBind(MessageContext ctx, string username)
    {
        UaaUserInfoContent userInfo;
        try
        {
            // 检测用户真实性，获取用户名
            userInfo = await _uaaService.UaaClient.User.GetInfoAsync(username);
        }
        catch (UaaRequestException uaaEx)
        {
            return ctx.Reply(_uaaService.GetExceptionReply(uaaEx));
        }

        // 检查之前的绑定
        var previous = await _database.Users.AsNoTracking().FirstOrDefaultAsync(
            user => user.Platform == ctx.Platform && user.UserId == ctx.UserId);

        var newBind = new ArcaeaDatabaseUser
        {
            Id = previous?.Id ?? default,
            Platform = ctx.Platform,
            UserId = ctx.UserId,
            ArcaeaCode = userInfo.AccountInfo.Code,
            ArcaeaName = userInfo.AccountInfo.Name
        };

        _database.Users.Update(newBind);
        await _database.SaveChangesAsync();

        return ctx.Reply(_localizer.GetReply("Bind",
            userInfo.AccountInfo.Name, userInfo.AccountInfo.Code));
    }
}