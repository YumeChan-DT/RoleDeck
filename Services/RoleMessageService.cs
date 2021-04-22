using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Nodsoft.YumeChan.PluginBase.Tools.Data;
using Nodsoft.YumeChan.RoleDeck.Data;

namespace Nodsoft.YumeChan.RoleDeck.Services
{
	public class RoleMessageService
	{
		private readonly IEntityRepository<RoleMessage, ulong> repository;

		public RoleMessageService(IDatabaseProvider<PluginManifest> database)
		{
			repository = database.GetEntityRepository<RoleMessage, ulong>();
		}


		public Task<RoleMessage> GetTrackedMessageAsync(DiscordMessage message) => repository.FindByIdAsync(message.Id);

		public Task TrackNewMessageAsync(DiscordMessage message)
		{
			if (MessageIsTracked(message.Id))
			{
				throw new ArgumentException("Message is already tracked.", nameof(message));
			}

			return repository.InsertOneAsync(new(message));
		}

		public Task RemoveMessageTrackingAsync(DiscordMessage message)
		{
			if (!MessageIsTracked(message.Id))
			{
				throw new ArgumentException("Message is not tracked.", nameof(message));
			}

			return repository.DeleteByIdAsync(message.Id);
		}

		public async Task TrackNewRoleReactionAsync(DiscordMessage message, DiscordEmoji emoji, DiscordRole role)
		{
			RoleMessage tracked = await repository.FindByIdAsync(message.Id);

			if (tracked is null)
			{
				tracked = new(message);
				await repository.InsertOneAsync(tracked);
			}

			if (tracked.Roles.Count >= RoleMessage.MaxReactionsPerMessage)
			{
				throw new ArgumentOutOfRangeException(nameof(message), "Cannot track more reactions for this message.");
			}

			if (tracked.Roles.ContainsKey(emoji.Name))
			{
				throw new ArgumentException("This Emoji is already used for another role.", nameof(emoji));
			}

			tracked.Roles.Add(emoji.Name, role.Id);
			await repository.ReplaceOneAsync(tracked);
		}

		public async Task RemoveRoleReactionAsync(DiscordMessage message, DiscordEmoji emoji)
		{
			RoleMessage tracked = await repository.FindByIdAsync(message.Id) ?? throw new ArgumentException("Message is not tracked.", nameof(message));

			tracked.Roles.Remove(emoji.Name);
			await repository.ReplaceOneAsync(tracked);
		}


		public async Task<ulong> GetTrackedRoleIdAsync(DiscordMessage message, DiscordEmoji emoji)
		{
			RoleMessage tracked = await repository.FindByIdAsync(message.Id);
			return tracked.Roles.GetValueOrDefault<string, ulong>(emoji.Name, 0);
		}


		public bool MessageIsTracked(ulong messageId) => repository.FindById(messageId) is not null;
	}
}
