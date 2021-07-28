using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YumeChan.RoleDeck.Services;

/**
 * HACK:
 * Merged the two listeners, since the current Dependency Injection is broken.
 * Should be fixed upstream on YumeChan.Core ASAP.
 */

namespace YumeChan.RoleDeck
{
	public class UserInteractionsListener : IHostedService
	{
		private readonly ILogger<UserInteractionsListener> logger;
		private readonly RoleMessageService roleMessageService;
		private readonly InitialRolesService initialRolesService;
		private readonly DiscordClient client;

		public UserInteractionsListener(ILogger<UserInteractionsListener> logger, DiscordClient client, RoleMessageService roleMessageService, InitialRolesService initialRolesService)
		{
			this.logger = logger;
			this.client = client;
			this.roleMessageService = roleMessageService;
			this.initialRolesService = initialRolesService;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			client.MessageReactionAdded += OnMessageReactionAddedAsync;
			client.MessageReactionRemoved += OnMessageReactionRemovedAsync;
			client.GuildMemberAdded += OnGuildMemberAddedAsync;

			logger.LogInformation("Started UserInteractionsListener.");
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			client.MessageReactionAdded -= OnMessageReactionAddedAsync;
			client.MessageReactionRemoved -= OnMessageReactionRemovedAsync;
			client.GuildMemberAdded -= OnGuildMemberAddedAsync;

			logger.LogInformation("Stopped UserInteractionsListener.");
			return Task.CompletedTask;
		}

		public async Task OnMessageReactionAddedAsync(DiscordClient _, MessageReactionAddEventArgs e)
		{
			if (!e.User.IsCurrent && (await roleMessageService.GetTrackedRoleIdAsync(e.Message, e.Emoji)) is not 0 and ulong roleId)
			{
				await (e.User as DiscordMember).GrantRoleAsync(e.Guild.GetRole(roleId));
				logger.LogDebug("RoleMessage {0} : Granted role {1} to {2}.", e.Message.Id, roleId, e.User);
			}

		}

		public async Task OnMessageReactionRemovedAsync(DiscordClient _, MessageReactionRemoveEventArgs e)
		{
			if (!e.User.IsCurrent && (await roleMessageService.GetTrackedRoleIdAsync(e.Message, e.Emoji)) is not 0 and ulong roleId)
			{
				await (e.User as DiscordMember).RevokeRoleAsync(e.Guild.GetRole(roleId));
				logger.LogDebug("RoleMessage {0} : Revoked role {1} on {2}.", e.Message.Id, roleId, e.User);
			}
		}

		private async Task OnGuildMemberAddedAsync(DiscordClient _, GuildMemberAddEventArgs e)
		{
			IEnumerable<ulong> roles = await initialRolesService.GetGuildRolesAsync(e.Guild.Id);

			if (roles is not null && roles.Any())
			{
				List<Task> grantRoleTasks = new();

				foreach (ulong roleId in roles)
				{
					grantRoleTasks.Add(e.Member.GrantRoleAsync(e.Guild.GetRole(roleId)));
					logger.LogInformation("Granting role {role} to user {user}.", roleId, e.Member.Id);
				}

				await Task.WhenAll(grantRoleTasks);
			}
		}
	}
}
