// using Flandre.Core.Messaging;
// using Flandre.Framework.Attributes;
//
// namespace YukiChanR.Plugins.Arcaea;
//
// public partial class ArcaeaPlugin
// {
//     [Command("a.b30")]
//     [StringShortcut("查b30", AllowArguments = true)]
//     [StringShortcut("查B30", AllowArguments = true)]
//     public async Task<MessageContent> OnBest30(MessageContext ctx,
//         [Option(ShortName = 'n')] bool nya,
//         [Option(ShortName = 'd')] bool dark,
//         string? user = null)
//     {
//         string target;
//         string logTarget;
//         
//         if (string.IsNullOrEmpty(user))
//         {
//             var binding = await _database.Users.AsNoTracking().FirstOrDefaultAsync(
//                 u => u.Platform == ctx.Platform && u.UserId == ctx.UserId);
//             if (binding is null)
//                 return ctx.Reply("请先使用 /a bind 名称或好友码 绑定你的账号哦~\n"
//                                  + "你也可以直接使用 /a b30 用户名或好友码 查询指定用户。");
//             target = binding.ArcaeaCode;
//             logTarget = binding.ArcaeaName;
//         }
//         else
//         {
//             logTarget = target = user;
//         }
//         
//         _logger.LogInformation("[Best30:Query] {ArcaeaUser} -> {SongName} ({Difficulty})",
//             logTarget, songname, difficulty.ToShortDisplayDifficulty());
//         
//         var bestInfo = await _uaaService.UaaClient.User.GetBestAsync(
//             target, song.SongId, AuaSongQueryType.SongId, difficulty, AuaReplyWith.SongInfo);
//         var userInfo = ArcaeaUser.FromUaa(bestInfo.AccountInfo);
//         
//         var pref = await _database.Preferences.AsNoTracking().FirstOrDefaultAsync(
//                        p => p.Platform == ctx.Platform && p.UserId == ctx.UserId)
//                    ?? new ArcaeaUserPreferences();
//         
//         pref.Nya = pref.Nya || nya;
//         pref.Dark = pref.Dark || dark;
//         
//         _logger.LogInformation("[Best30:ImageGen] {ArcaeaUser} -> {SongName} ({Difficulty})",
//             logTarget, songname, difficulty.ToShortDisplayDifficulty());
//         
//         var image = await _imageGen.GenerateSingleAsync(
//             ArcaeaRecord.FromUaa(bestInfo.Record, bestInfo.SongInfo![(int)difficulty]), pref);
//         
//         return ctx
//             .Reply($"{userInfo.Name} ({ArcaeaUtils.ToDisplayPotential(userInfo.Potential)})\n")
//             .Image(image);
//     }
// }