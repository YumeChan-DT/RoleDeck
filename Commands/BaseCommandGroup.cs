using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using YumeChan.RoleDeck.Data;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck.Commands
{
	[Group("roledeck"), Aliases("rd"), RequireGuild, RequirePermissions(Permissions.Administrator)]
	public partial class BaseCommandGroup : BaseCommandModule
	{
		private readonly RoleMessageService service;

		public BaseCommandGroup(RoleMessageService roleMessageService)
		{
			service = roleMessageService;
		}

		[Command("track"), RequireGuild, RequirePermissions(Permissions.Administrator)]
		public async Task TrackMessageAsync(CommandContext context, DiscordMessage message)
		{
			await service.TrackNewMessageAsync(message);
			await context.RespondAsync($"Message `{message.Id}` is now tracked.");
		}

		[Command("untrack"), RequireGuild, RequirePermissions(Permissions.Administrator)]
		public async Task UntrackMessageAsync(CommandContext context, DiscordMessage message)
		{
			await service.RemoveMessageTrackingAsync(message);
			await context.RespondAsync($"Message `{message.Id}` is no longer tracked.");
		}

		[Command("add-role"), Aliases("bind"), RequireGuild, RequirePermissions(Permissions.Administrator)]
		public async Task AddRoleAsync(CommandContext context, DiscordMessage message, DiscordEmoji emoji, DiscordRole role)
		{
			await service.TrackNewRoleReactionAsync(message, emoji, role);
			await context.RespondAsync($"Reaction {emoji} will now grant Role {role.Mention} on Message `{message.Id}`.");
		}

		[Command("remove-role"), Aliases("unbind"), RequireGuild, RequirePermissions(Permissions.Administrator)]
		public async Task RemoveRoleAsync(CommandContext context, DiscordMessage message, DiscordEmoji emoji)
		{
			await service.RemoveRoleReactionAsync(message, emoji);
			await context.RespondAsync($"Reaction {emoji} will is no longer bound on Message `{message.Id}`.");
		}

		[Command("init-reactions"), RequireGuild, RequirePermissions(Permissions.Administrator)]
		public async Task InitReactionsAsync(CommandContext context, DiscordMessage message)
		{
			RoleMessage tracked = await service.GetTrackedMessageAsync(message);

			foreach (string reaction in tracked.Roles.Keys)
			{
				if (!DiscordEmoji.TryFromUnicode(context.Client, reaction, out DiscordEmoji emoji))
				{
					DiscordEmoji.TryFromName(context.Client, $":{reaction}:", out emoji);
				}

				await message.CreateReactionAsync(emoji);
			}

			await message.RespondAsync("Reactions initialized.");
		}
	}
}
