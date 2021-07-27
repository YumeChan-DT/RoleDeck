using DSharpPlus.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YumeChan.PluginBase.Tools.Data;
using YumeChan.RoleDeck.Data;

namespace YumeChan.RoleDeck.Services
{
	class InitialRolesService
	{
		private readonly IMongoCollection<InitialRoles> initialRoles;

		public InitialRolesService(IDatabaseProvider<PluginManifest> databaseProvider)
		{
			 initialRoles = databaseProvider.GetMongoDatabase().GetCollection<InitialRoles>(nameof(InitialRoles));
		}

		public async Task<IEnumerable<ulong>> GetGuildRoles(ulong guildId) => (await initialRoles.FindAsync(ir => ir.GuildId == guildId)).FirstOrDefault()?.RoleIds;

		public async Task AddGuildRole(ulong guildId, DiscordRole role)
		{
			InitialRoles roles = await FindOrInsertDbAsync(guildId);

			if (!roles.RoleIds.Contains(role.Id))
			{
				await initialRoles.UpdateOneAsync(
					Builders<InitialRoles>.Filter.Eq(ir => ir.GuildId, guildId),
					Builders<InitialRoles>.Update.Push(ir => ir.RoleIds, role.Id)
				);
			}
		}

		public async Task RemoveGuildRole(ulong guildId, DiscordRole role)
		{
			InitialRoles roles = await FindOrInsertDbAsync(guildId);

			if (roles.RoleIds.Contains(role.Id))
			{
				await initialRoles.UpdateOneAsync(
					Builders<InitialRoles>.Filter.Eq(ir => ir.GuildId, guildId),
					Builders<InitialRoles>.Update.Pull(ir => ir.RoleIds, role.Id)
				);
			}
		}

		public Task ClearGuildRoles(ulong guildId) => initialRoles.DeleteOneAsync(ir => ir.GuildId == guildId);


		private async Task<InitialRoles> FindOrInsertDbAsync(ulong guildId)
		{
			if ((await initialRoles.FindAsync(ir => ir.GuildId == guildId)).FirstOrDefault() is not InitialRoles roles)
			{
				roles = new() { GuildId = guildId };
				await initialRoles.InsertOneAsync(roles);
			}

			return roles;
		}
	}
}
