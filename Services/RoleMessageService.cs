using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MongoDB.Driver;
using YumeChan.PluginBase.Tools.Data;
using YumeChan.RoleDeck.Data;

namespace YumeChan.RoleDeck.Services;

public class RoleMessageService
{
	private readonly IMongoCollection<RoleMessage> _roleMessages;

	public RoleMessageService(IDatabaseProvider<PluginManifest> database)
	{
		_roleMessages = database.GetMongoDatabase().GetCollection<RoleMessage>(nameof(RoleMessage));
	}


	public async Task<RoleMessage> GetTrackedMessageAsync(DiscordMessage message) => (await _roleMessages.FindAsync(m => m.Id == message.Id)).FirstOrDefault();

	public async Task TrackNewMessageAsync(DiscordMessage message)
	{
		if (await MessageIsTracked(message.Id))
		{
			throw new ArgumentException("Message is already tracked.", nameof(message));
		}

		await _roleMessages.InsertOneAsync(new(message));
	}

	public async Task RemoveMessageTrackingAsync(DiscordMessage message)
	{
		if (!await MessageIsTracked(message.Id))
		{
			throw new ArgumentException("Message is not tracked.", nameof(message));
		}

		await _roleMessages.DeleteOneAsync(m => m.Id == message.Id);
	}

	public async Task TrackNewRoleReactionAsync(DiscordMessage message, DiscordEmoji emoji, DiscordRole role)
	{
		RoleMessage tracked = await GetTrackedMessageAsync(message);

		if (tracked is null)
		{
			tracked = new(message);
			await _roleMessages.InsertOneAsync(tracked);
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
		await _roleMessages.ReplaceOneAsync(Builders<RoleMessage>.Filter.Eq(m => m.Id, tracked.Id), tracked);
	}

	public async Task RemoveRoleReactionAsync(DiscordMessage message, DiscordEmoji emoji)
	{
		RoleMessage tracked = await GetTrackedMessageAsync(message) ?? throw new ArgumentException("Message is not tracked.", nameof(message));
		tracked.Roles.Remove(emoji.Name);
		await _roleMessages.ReplaceOneAsync(Builders<RoleMessage>.Filter.Eq(m => m.Id, tracked.Id), tracked);
	}


	public async Task<ulong> GetTrackedRoleIdAsync(DiscordMessage message, DiscordEmoji emoji)
	{
		RoleMessage tracked = await GetTrackedMessageAsync(message);
		return tracked?.Roles.GetValueOrDefault<string, ulong>(emoji.Name, 0) ?? 0;
	}


	public async Task<bool> MessageIsTracked(ulong messageId) => await (await _roleMessages.FindAsync(m => m.Id == messageId)).AnyAsync();
}