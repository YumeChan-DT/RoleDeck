using DSharpPlus;
using DSharpPlus.Entities;
using System.Linq;
using System.Threading.Tasks;
using YumeChan.PluginBase.Tools.Data;

namespace YumeChan.RoleDeck.Services;

public sealed class MassRoleService
{
	private readonly DiscordClient _client;

	public MassRoleService(DiscordClient client, IDatabaseProvider<PluginManifest> databaseProvider)
	{
		_client = client;
	}

	public async Task AssignMassRoleToGuildMembersAsync(DiscordGuild guild, DiscordRole guildRole) 
		=> await Task.WhenAll((await guild.GetAllMembersAsync()).Select(member => member.GrantRoleAsync(guildRole)));

	public async Task RemoveMassRoleOnGuildMembersAsync(DiscordGuild guild, DiscordRole guildRole) 
		=> await Task.WhenAll((await guild.GetAllMembersAsync()).Select(member => member.RevokeRoleAsync(guildRole)));
}