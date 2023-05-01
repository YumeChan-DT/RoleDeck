using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YumeChan.PluginBase;
using YumeChan.RoleDeck.Services;



namespace YumeChan.RoleDeck;

// ReSharper disable once ClassNeverInstantiated.Global
public class PluginManifest : Plugin
{
	public override string DisplayName => "Yume-Chan RoleDeck";
	public override bool StealthMode => false;


	private readonly ILogger<PluginManifest> _logger;
	private readonly UserReactionsListener _reactionsListener;
	private readonly IncomingUsersListener _incomingUsersListener;

	public PluginManifest(ILogger<PluginManifest> logger, UserReactionsListener reactionsListener, IncomingUsersListener incomingUsersListener)
	{
		_logger = logger;
		_reactionsListener = reactionsListener;
		_incomingUsersListener = incomingUsersListener;
	}


	public override async Task LoadAsync()
	{
		CancellationToken ct = CancellationToken.None; // May get added into method parameters later on.

		await base.LoadAsync();
		await _reactionsListener.StartAsync(ct);
		await _incomingUsersListener.StartAsync(ct);

		_logger.LogInformation("Loaded {Plugin}.", DisplayName);
	}

	public override async Task UnloadAsync()
	{
		CancellationToken ct = CancellationToken.None; // May get added into method parameters later on.
		_logger.LogInformation("Unloading {Plugin}...", DisplayName);

		await _reactionsListener.StopAsync(ct);
		await _incomingUsersListener.StopAsync(ct);
		await base.UnloadAsync();
	}
}

public sealed class DependencyRegistrations : DependencyInjectionHandler
{
	public override IServiceCollection ConfigureServices(IServiceCollection services) => services
		.AddSingleton<IncomingUsersListener>()
		.AddSingleton<UserReactionsListener>()
		.AddSingleton<InitialRolesService>()
		.AddSingleton<RoleMessageService>()
		.AddSingleton<MassRoleService>()
	;
}