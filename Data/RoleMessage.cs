using System.Collections.Generic;
using DSharpPlus.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace YumeChan.RoleDeck.Data;

public sealed record RoleMessage
{
	[BsonIgnore]
	public const int MaxReactionsPerMessage = 20;

	[BsonId]
	public ulong Id { get; set; }
	public ulong ChannelId { get; set; }
	public ulong GuildId { get; set; }

	public Dictionary<string, ulong> Roles { get; set; }

	public RoleMessage() { }

	public RoleMessage(DiscordMessage message)
	{
		Id = message.Id;
		ChannelId = message.ChannelId;
		GuildId = message.Channel.GuildId ?? 0;

		Roles = new(MaxReactionsPerMessage);
	}
}