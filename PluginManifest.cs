﻿using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YumeChan.PluginBase;
using YumeChan.RoleDeck.Services;



namespace YumeChan.RoleDeck
{
	public class PluginManifest : Plugin
	{
		public override string PluginDisplayName => "Yume-Chan RoleDeck";
		public override bool PluginStealth => false;


		private readonly ILogger<PluginManifest> logger;
		private readonly UserReactionsListener reactionsListener;

		public PluginManifest(ILogger<PluginManifest> logger, UserReactionsListener reactionsListener)
		{
			this.logger = logger;
			this.reactionsListener = reactionsListener;
		}


		public override async Task LoadPlugin()
		{
			CancellationToken cancellationToken = CancellationToken.None; // May get added into method parameters later on.

			await base.LoadPlugin();
			await reactionsListener.StartAsync(cancellationToken);

			logger.LogInformation("Loaded {0}.", PluginDisplayName);
		}

		public override async Task UnloadPlugin()
		{
			CancellationToken cancellationToken = CancellationToken.None; // May get added into method parameters later on.
			logger.LogInformation("Unloading {0}...", PluginDisplayName);

			await reactionsListener.StopAsync(cancellationToken);
			await base.UnloadPlugin();
		}

		public override IServiceCollection ConfigureServices(IServiceCollection services) => services
			.AddHostedService<UserReactionsListener>()
			.AddSingleton<UserReactionsListener>()
			.AddSingleton<RoleMessageService>();
	}
}
