using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.YumeChan.PluginBase;

namespace Nodsoft.YumeChan.RoleDeck
{
	public class PluginManifest : Plugin
	{
		private readonly ILogger<PluginManifest> logger;

		public override string PluginDisplayName => "Yume-Chan RoleDeck";
		public override bool PluginStealth => false;


		public PluginManifest(ILogger<PluginManifest> logger)
		{
			this.logger = logger;
		}


		public override async Task LoadPlugin()
		{
			await base.LoadPlugin();

			logger.LogInformation("Loaded {0}.", PluginDisplayName);
		}

		public override async Task UnloadPlugin() 
		{
			logger.LogInformation("Unloading {0}...", PluginDisplayName);

			await base.UnloadPlugin();
		}

		public override IServiceCollection ConfigureServices(IServiceCollection services) => services;
	}
}
