using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YumeChan.RoleDeck.Data;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck
{
	public class IncomingUsersListener : IHostedService
	{
		private readonly ILogger logger;
		private readonly InitialRolesService service;
		private readonly DiscordClient client;

		public IncomingUsersListener(ILogger logger, InitialRolesService service, DiscordClient client)
		{
			this.logger = logger;
			this.service = service;
			this.client = client;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			client.GuildMemberAdded += OnGuildMemberAddedAsync;

			logger.LogInformation("Started IncomingUsersListener.");
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			client.GuildMemberAdded -= OnGuildMemberAddedAsync;

			logger.LogInformation("Started IncomingUsersListener.");
			return Task.CompletedTask;
		}

		private async Task OnGuildMemberAddedAsync(DiscordClient sender, GuildMemberAddEventArgs e)
		{
			IEnumerable<ulong> roles = await service.GetGuildRolesAsync(e.Guild.Id);

			if (roles is not null && roles.Any())
			{
				List<Task> grantRoleTasks = new();	

				foreach (ulong roleId in roles)
				{
					grantRoleTasks.Add(e.Member.GrantRoleAsync(e.Guild.GetRole(roleId)));
				}

				await Task.WhenAll(grantRoleTasks);
			}
		}
	}
}
