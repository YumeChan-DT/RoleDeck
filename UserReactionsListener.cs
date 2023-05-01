using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck;

public sealed class UserReactionsListener : IHostedService
{
	private readonly ILogger<UserReactionsListener> _logger;
	private readonly RoleMessageService _service;
	private readonly DiscordClient _discordClient;

	public UserReactionsListener(ILogger<UserReactionsListener> logger, RoleMessageService service, DiscordClient discordClient)
	{
		_logger = logger;
		_service = service;
		_discordClient = discordClient;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_discordClient.MessageReactionAdded += OnMessageReactionAddedAsync;
		_discordClient.MessageReactionRemoved += OnMessageReactionRemovedAsync;

		_logger.LogInformation("Started UserReactionsListener.");
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_discordClient.MessageReactionAdded -= OnMessageReactionAddedAsync;
		_discordClient.MessageReactionRemoved -= OnMessageReactionRemovedAsync;

		_logger.LogInformation("Stopped UserReactionsListener.");
		return Task.CompletedTask;
	}

	public async Task OnMessageReactionAddedAsync(DiscordClient client, MessageReactionAddEventArgs e)
	{
		if (!e.User.IsCurrent && await _service.GetTrackedRoleIdAsync(e.Message, e.Emoji) is not 0 and ulong roleId)
		{
			await ((DiscordMember)e.User).GrantRoleAsync(e.Guild.GetRole(roleId));
			_logger.LogDebug("RoleMessage {MessageId} : Granted role {RoleId} to {User}.", e.Message.Id, roleId, e.User);
		}

	}

	public async Task OnMessageReactionRemovedAsync(DiscordClient client, MessageReactionRemoveEventArgs e)
	{
		if (!e.User.IsCurrent && await _service.GetTrackedRoleIdAsync(e.Message, e.Emoji) is not 0 and ulong roleId)
		{
			await ((DiscordMember)e.User).RevokeRoleAsync(e.Guild.GetRole(roleId));
			_logger.LogDebug("RoleMessage {MessageId} : Revoked role {RoleId} on {User}.", e.Message.Id, roleId, e.User);
		}
	}
}