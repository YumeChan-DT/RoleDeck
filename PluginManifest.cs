using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YumeChan.PluginBase;
using YumeChan.RoleDeck.Services;



namespace YumeChan.RoleDeck
{
	public class PluginManifest : Plugin
	{
		public override string DisplayName => "Yume-Chan RoleDeck";
		public override bool StealthMode => false;


		private readonly ILogger<PluginManifest> logger;
		private readonly UserReactionsListener reactionsListener;
		private readonly IncomingUsersListener incomingUsersListener;

		public PluginManifest(ILogger<PluginManifest> logger, UserReactionsListener reactionsListener, IncomingUsersListener incomingUsersListener)
		{
			this.logger = logger;
			this.reactionsListener = reactionsListener;
			this.incomingUsersListener = incomingUsersListener;
		}


		public override async Task LoadAsync()
		{
			CancellationToken cancellationToken = CancellationToken.None; // May get added into method parameters later on.

			await base.LoadAsync();
			await reactionsListener.StartAsync(cancellationToken);
			await incomingUsersListener.StartAsync(cancellationToken);

			logger.LogInformation("Loaded {plugin}.", DisplayName);
		}

		public override async Task UnloadAsync()
		{
			CancellationToken cancellationToken = CancellationToken.None; // May get added into method parameters later on.
			logger.LogInformation("Unloading {plugin}...", DisplayName);

			await reactionsListener.StopAsync(cancellationToken);
			await incomingUsersListener.StopAsync(cancellationToken);
			await UnloadAsync();
		}
	}

	public class DependencyRegistrations : DependencyInjectionHandler
	{
		public override IServiceCollection ConfigureServices(IServiceCollection services) => services
			.AddHostedService<UserReactionsListener>()
			.AddHostedService<IncomingUsersListener>()
			.AddSingleton<IncomingUsersListener>()
			.AddSingleton<UserReactionsListener>()
			.AddSingleton<InitialRolesService>()
			.AddSingleton<RoleMessageService>()
			.AddSingleton<MassRoleService>()
			;
	}
}
