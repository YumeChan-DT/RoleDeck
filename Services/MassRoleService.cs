using DSharpPlus;
using DSharpPlus.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YumeChan.PluginBase.Tools.Data;
using YumeChan.RoleDeck.Data;

namespace YumeChan.RoleDeck.Services
{
	public class MassRoleService
	{
		private readonly DiscordClient client;

		public MassRoleService(DiscordClient client, IDatabaseProvider<PluginManifest> databaseProvider)
		{
			this.client = client;
		}

		public async Task AssignMassRoleToGuildMembersAsync(DiscordGuild guild, DiscordRole guildRole)
		{

			List<Task> roleAssignments = new();

			foreach (DiscordMember member in await guild.GetAllMembersAsync())
			{
				roleAssignments.Add(member.GrantRoleAsync(guildRole));
			}

			await Task.WhenAll(roleAssignments);
		}

		public async Task RemoveMassRoleOnGuildMembersAsync(DiscordGuild guild, DiscordRole guildRole)
		{
			List<Task> roleAssignments = new();

			foreach (DiscordMember member in await guild.GetAllMembersAsync())
			{
				roleAssignments.Remove(member.GrantRoleAsync(guildRole));
			}

			await Task.WhenAll(roleAssignments);
		}
	}
}
