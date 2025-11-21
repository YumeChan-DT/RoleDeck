using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YumeChan.RoleDeck.Services;

namespace YumeChan.RoleDeck;

public sealed class IncomingUsersListener : IHostedService
{
	private readonly ILogger<IncomingUsersListener> _logger;
	private readonly InitialRolesService _service;
	private readonly DiscordClient _client;

	public IncomingUsersListener(ILogger<IncomingUsersListener> logger, InitialRolesService service, DiscordClient client)
	{
		_logger = logger;
		_service = service;
		_client = client;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_client.GuildMemberAdded += OnGuildMemberAddedAsync;

		_logger.LogInformation("Started IncomingUsersListener");
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_client.GuildMemberAdded -= OnGuildMemberAddedAsync;

		_logger.LogInformation("Started IncomingUsersListener");
		return Task.CompletedTask;
	}

	private async Task OnGuildMemberAddedAsync(DiscordClient sender, GuildMemberAddEventArgs e)
	{
		ulong[] roles = (await _service.GetGuildRolesAsync(e.Guild.Id)).ToArray();

		if (roles is { Length: not 0 })
		{
			await Task.WhenAll(roles.Select(roleId => e.Member.GrantRoleAsync(e.Guild.GetRole(roleId))));
		}
	}
}