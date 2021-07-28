﻿using DSharpPlus.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YumeChan.PluginBase.Tools.Data;
using YumeChan.RoleDeck.Data;

namespace YumeChan.RoleDeck.Services
{
	public class InitialRolesService
	{
		private readonly IMongoCollection<InitialRoles> initialRoles;

		public InitialRolesService(IDatabaseProvider<PluginManifest> databaseProvider)
		{
			 initialRoles = databaseProvider.GetMongoDatabase().GetCollection<InitialRoles>(nameof(InitialRoles));
		}

		public async Task<IEnumerable<ulong>> GetGuildRolesAsync(ulong guildId) => (await initialRoles.FindAsync(ir => ir.GuildId == guildId)).FirstOrDefault()?.RoleIds;

		public async Task AddGuildRoleAsync(ulong guildId, DiscordRole role)
		{
			InitialRoles roles = await FindOrInsertDbAsync(guildId);

			if (roles.RoleIds is not null && !roles.RoleIds.Contains(role.Id))
			{
				await initialRoles.UpdateOneAsync(
					Builders<InitialRoles>.Filter.Eq(ir => ir.GuildId, guildId),
					Builders<InitialRoles>.Update.Push(ir => ir.RoleIds, role.Id)
				);
			}
		}

		public async Task RemoveGuildRoleAsync(ulong guildId, DiscordRole role)
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

		public Task ClearGuildRolesAsync(ulong guildId) => initialRoles.DeleteOneAsync(ir => ir.GuildId == guildId);


		private async Task<InitialRoles> FindOrInsertDbAsync(ulong guildId)
		{
			if ((await initialRoles.FindAsync(ir => ir.GuildId == guildId)).FirstOrDefault() is not InitialRoles roles)
			{
				roles = new()
				{
					GuildId = guildId,
					RoleIds = new()
				};

				await initialRoles.InsertOneAsync(roles);
			}

			return roles;
		}
	}
}
