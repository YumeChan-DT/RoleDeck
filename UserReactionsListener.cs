﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Nodsoft.YumeChan.RoleDeck.Data;
using Nodsoft.YumeChan.RoleDeck.Services;

namespace Nodsoft.YumeChan.RoleDeck
{
	public class UserReactionsListener
	{
		private readonly ILogger<UserReactionsListener> logger;
		private readonly RoleMessageService service;

		public UserReactionsListener(ILogger<UserReactionsListener> logger, RoleMessageService service)
		{
			this.logger = logger;
			this.service = service;
		}

		public async Task OnMessageReactionAddedAsync(DiscordClient client, MessageReactionAddEventArgs e)
		{
			if (!e.User.IsCurrent && await service.GetTrackedMessageAsync(e.Message) is RoleMessage message)
			{
				if (message.MutuallyExclusive)
				{
					foreach (KeyValuePair<string, ulong> reactionRole in message.Roles)
					{
						if (reactionRole.Key == e.Emoji.Name)
						{
							await (e.User as DiscordMember).GrantRoleAsync(e.Guild.GetRole(reactionRole.Value));
							logger.LogDebug("RoleMessage {0} : Granted role (exclusive) {1} to {2}.", e.Message.Id, reactionRole.Value, e.User);
						}
						else
						{
							await (e.User as DiscordMember).RevokeRoleAsync(e.Guild.GetRole(reactionRole.Value));
						}
					}
				}
				else
				{

					if (message.Roles.TryGetValue(e.Emoji, out ulong roleId))
					{
						await (e.User as DiscordMember).GrantRoleAsync(e.Guild.GetRole(roleId));
						logger.LogDebug("RoleMessage {0} : Granted role {1} to {2}.", e.Message.Id, roleId, e.User);
					}
				}
			}
		}

		public async Task OnMessageReactionRemovedAsync(DiscordClient client, MessageReactionRemoveEventArgs e)
		{
			if (!e.User.IsCurrent && (await service.GetTrackedRoleIdAsync(e.Message, e.Emoji)) is not 0 and ulong roleId)
			{
				await (e.User as DiscordMember).RevokeRoleAsync(e.Guild.GetRole(roleId));
				logger.LogDebug("RoleMessage {0} : Revoked role {1} on {2}.", e.Message.Id, roleId, e.User);
			}
		}
	}
}
