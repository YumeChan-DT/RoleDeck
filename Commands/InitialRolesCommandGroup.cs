using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using YumeChan.RoleDeck.Services;



namespace YumeChan.RoleDeck.Commands;

public partial class BaseCommandGroup
{
	[Group("initial-roles"), Aliases("ir"), RequireGuild]
	public class InitialRolesCommandGroup : BaseCommandModule
	{
		private readonly InitialRolesService _service;

		public InitialRolesCommandGroup(InitialRolesService service)
		{
			_service = service;
		}

		[GroupCommand]
		public async Task ListRolesAsync(CommandContext ctx)
		{
			ImmutableArray<ulong> roles = (await _service.GetGuildRolesAsync(ctx.Guild.Id)).ToImmutableArray();

			if (roles is { Length: > 0 })
			{
				StringBuilder rolesListing = new();

				foreach (ulong role in roles)
				{
					rolesListing.Append($"{ctx.Guild.GetRole(role).Mention}\n");
				}

				DiscordEmbedBuilder embed = new()
				{
					Title = "Initial Roles",
					Description = rolesListing.ToString(),
					Color = DiscordColor.Aquamarine
				};

				await ctx.RespondAsync("Listing current Initial Roles :", embed);
			}
			else
			{
				await ctx.RespondAsync("No Initial Roles set for this server.");
			}
		}


		[Command("add"), RequirePermissions(Permissions.Administrator)]
		public async Task AddRoleAsync(CommandContext ctx, DiscordRole role)
		{
			await _service.AddGuildRoleAsync(ctx.Guild.Id, role);
			await ctx.RespondAsync($"Role {role.Mention} will now be added to all new server members.");
		}

		[Command("remove"), RequirePermissions(Permissions.Administrator)]
		public async Task RemoveRoleAsync(CommandContext ctx, DiscordRole role)
		{
			await _service.RemoveGuildRoleAsync(ctx.Guild.Id, role);
			await ctx.RespondAsync($"Role {role.Mention} will no longer be added to all new server members.");
		}
	}
}