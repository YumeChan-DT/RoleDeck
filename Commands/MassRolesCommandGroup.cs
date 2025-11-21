using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using JetBrains.Annotations;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck.Commands;

public partial class BaseCommandGroup
{
	[Group("mass-role"), Aliases("mr"), PublicAPI]
	public sealed class MassRolesCommandGroup : BaseCommandModule
	{
		private readonly MassRoleService _massRoleService;

		public MassRolesCommandGroup(MassRoleService massRoleService)
		{
			_massRoleService = massRoleService;
		}

		[Command("assign"), Aliases("add"), RequirePermissions(Permissions.Administrator)]
		public async Task AssignRoleAsync(CommandContext ctx, DiscordRole role)
		{
			await _massRoleService.AssignMassRoleToGuildMembersAsync(ctx.Guild, role);
			await ctx.RespondAsync($"Role {role.Mention} was applied to all members server-wide.");
		}

		[Command("revoke"), Aliases("rem"), RequirePermissions(Permissions.Administrator)]
		public async Task RevokeRoleAsync(CommandContext ctx, DiscordRole role)
		{
			await _massRoleService.RemoveMassRoleOnGuildMembersAsync(ctx.Guild, role);
			await ctx.RespondAsync($"Role {role.Mention} was revoked to all members server-wide.");
		}
	}
}