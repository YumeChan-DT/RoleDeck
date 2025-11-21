using DSharpPlus;
using DSharpPlus.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace YumeChan.RoleDeck.Services;

public sealed class MassRoleService
{
	private readonly DiscordClient _client;

	public MassRoleService(DiscordClient client)
	{
		_client = client;
	}

	public async Task AssignMassRoleToGuildMembersAsync(DiscordGuild guild, DiscordRole guildRole) => await Task.WhenAll(
		from member in await guild.GetAllMembersAsync()
		where !member.Roles.Contains(guildRole)
		select member.GrantRoleAsync(guildRole)
	);

	public async Task RemoveMassRoleOnGuildMembersAsync(DiscordGuild guild, DiscordRole guildRole) => await Task.WhenAll(
		from member in await guild.GetAllMembersAsync()
		where member.Roles.Contains(guildRole)
		select member.RevokeRoleAsync(guildRole)
	);
}