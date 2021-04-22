using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Nodsoft.YumeChan.PluginBase;

namespace Nodsoft.YumeChan.RoleDeck
{
	public class PluginManifest : Plugin
	{
		public override string PluginDisplayName => "Yume-Chan RoleDeck";
		public override bool PluginStealth => false;


		private readonly ILogger<PluginManifest> logger;
		private readonly DiscordClient discordClient;
		private readonly UserReactionsListener reactionsListener;

		public PluginManifest(ILogger<PluginManifest> logger, DiscordClient discordClient, UserReactionsListener reactionsListener)
		{
			this.logger = logger;
			this.discordClient = discordClient;
			this.reactionsListener = reactionsListener;
		}


		public override async Task LoadPlugin()
		{
			await base.LoadPlugin();

			discordClient.MessageReactionAdded += reactionsListener.OnMessageReactionAddedAsync;
			discordClient.MessageReactionRemoved += reactionsListener.OnMessageReactionRemovedAsync;

			logger.LogInformation("Loaded {0}.", PluginDisplayName);
		}

		public override async Task UnloadPlugin() 
		{
			logger.LogInformation("Unloading {0}...", PluginDisplayName);

			await base.UnloadPlugin();
		}

		public override IServiceCollection ConfigureServices(IServiceCollection services) => services
			.AddSingleton<UserReactionsListener>();
	}
}
