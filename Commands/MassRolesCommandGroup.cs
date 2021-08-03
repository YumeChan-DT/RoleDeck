using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck.Commands
{
	public partial class BaseCommandGroup
	{
		[Group("mass-role"), Aliases("mr")]
		public class MassRolesCommandGroup : BaseCommandModule
		{
			private readonly MassRoleService massRoleService;

			public MassRolesCommandGroup(MassRoleService massRoleService)
			{
				this.massRoleService = massRoleService;
			}

			[Command("assign"), Aliases("add"), RequirePermissions(Permissions.Administrator)]
			public async Task AssignRoleAsync(CommandContext ctx, DiscordRole role)
			{
				await massRoleService.AssignMassRoleToGuildMembersAsync(ctx.Guild, role);
				await ctx.RespondAsync($"Role {role.Mention} was applied to all members server-wide.");
			}

			[Command("revoke"), Aliases("rem"), RequirePermissions(Permissions.Administrator)]
			public async Task RevokeRoleAsync(CommandContext ctx, DiscordRole role)
			{
				await massRoleService.RemoveMassRoleOnGuildMembersAsync(ctx.Guild, role);
				await ctx.RespondAsync($"Role {role.Mention} was revoked to all members server-wide.");
			}
		}
	}
}
