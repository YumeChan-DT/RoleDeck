using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck
{
	public class UserReactionsListener : IHostedService
	{
		private readonly ILogger<UserReactionsListener> logger;
		private readonly RoleMessageService service;
		private readonly DiscordClient discordClient;

		public UserReactionsListener(ILogger<UserReactionsListener> logger, RoleMessageService service, DiscordClient discordClient)
		{
			this.logger = logger;
			this.service = service;
			this.discordClient = discordClient;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			discordClient.MessageReactionAdded += OnMessageReactionAddedAsync;
			discordClient.MessageReactionRemoved += OnMessageReactionRemovedAsync;

			logger.LogInformation("Started UserReactionsListener.");
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			discordClient.MessageReactionAdded -= OnMessageReactionAddedAsync;
			discordClient.MessageReactionRemoved -= OnMessageReactionRemovedAsync;

			logger.LogInformation("Stopped UserReactionsListener.");
			return Task.CompletedTask;
		}

		public async Task OnMessageReactionAddedAsync(DiscordClient client, MessageReactionAddEventArgs e)
		{
			if (!e.User.IsCurrent && (await service.GetTrackedRoleIdAsync(e.Message, e.Emoji)) is not 0 and ulong roleId)
			{
				await (e.User as DiscordMember).GrantRoleAsync(e.Guild.GetRole(roleId));
				logger.LogDebug("RoleMessage {0} : Granted role {1} to {2}.", e.Message.Id, roleId, e.User);
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
