using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;



namespace YumeChan.RoleDeck.Data;

/// <summary>
/// Provides initial roles to apply to user, upon server join.
/// </summary>
public sealed record InitialRoles
{
	[BsonId, BsonRepresentation(BsonType.Int64)]
	public ulong GuildId { get; set; }

	public List<ulong> RoleIds { get; set; }
}