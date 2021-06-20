using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using YumeChan.PluginBase.Tools.Data;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck.Data
{
	public record RoleMessage : IDocument<ulong>
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
}
