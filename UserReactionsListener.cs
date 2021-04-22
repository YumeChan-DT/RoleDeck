using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace Nodsoft.YumeChan.RoleDeck
{
	public class UserReactionsListener
	{
		private readonly ILogger<UserReactionsListener> logger;

		public UserReactionsListener(ILogger<UserReactionsListener> logger)
		{
			this.logger = logger;
		}

		public async Task OnMessageReactionAddedAsync(DiscordClient client, MessageReactionAddEventArgs e)
		{
			if ((await e.Channel.GetMessageAsync(e.Message.Id)).Author.IsCurrent)
			{
				logger.LogDebug("Message {0} : Added reaction {1} by {2}.", e.Message.Id, e.Emoji, e.User);
			}
		}

		public async Task OnMessageReactionRemovedAsync(DiscordClient client, MessageReactionRemoveEventArgs e)
		{
			if ((await e.Channel.GetMessageAsync(e.Message.Id)).Author.IsCurrent)
			{
				logger.LogDebug("Message {0} : Removed reaction {1} by {2}.", e.Message.Id, e.Emoji, e.User);
			}
		}
	}
}
