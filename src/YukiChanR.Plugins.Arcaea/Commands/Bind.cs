using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Microsoft.EntityFrameworkCore;
using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Entities;

// ReSharper disable CheckNamespace

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.bind")]
    public async Task<MessageContent> OnBind(MessageContext ctx, string username)
    {
        // 检测用户真实性，获取用户名
        var userInfo = await _uaaClient.User.GetInfoAsync(username);

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

        return ctx.Reply($"绑定成功~\n{userInfo.AccountInfo.Name} / {userInfo.AccountInfo.Code}\n")
            .Text($"注册时间: {DateTimeUtils.FormatUtc8Text(userInfo.AccountInfo.JoinDate)}");
    }
}