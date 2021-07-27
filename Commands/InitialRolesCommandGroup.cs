using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YumeChan.RoleDeck.Services;



namespace YumeChan.RoleDeck.Commands
{
	public partial class BaseCommandGroup
	{
		[Group("initial-roles"), Aliases("ir")]
		public class InitialRolesCommandGroup : BaseCommandModule
		{
			private readonly InitialRolesService service;

			public InitialRolesCommandGroup(InitialRolesService service)
			{
				this.service = service;
			}

			[GroupCommand]
			public async Task ListRolesAsync(CommandContext ctx)
			{
				IEnumerable<ulong> roles = await service.GetGuildRoles(ctx.Guild.Id);

				if (roles.Any())
				{
					StringBuilder rolesListing = new();

					foreach (ulong role in roles)
					{
						rolesListing.AppendFormat("<@{0}>\n");
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


			[Command("add")]
			public async Task AddRoleAsync(CommandContext ctx, DiscordRole role)
			{
				await service.AddGuildRole(ctx.Guild.Id, role);
				await ctx.RespondAsync($"Role {role.Mention} will now be added to all new server members.");
			}

			[Command("remove")]
			public async Task RemoveRoleAsync(CommandContext ctx, DiscordRole role)
			{
				await service.RemoveGuildRole(ctx.Guild.Id, role);
				await ctx.RespondAsync($"Role {role.Mention} will no longer be added to all new server members.");
			}
		}
	}
}
