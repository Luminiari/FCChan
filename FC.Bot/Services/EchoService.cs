﻿// Copyright (c) FCChan. All rights reserved.
//
// Licensed under the MIT license.

namespace FC.Bot.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;
	using Discord;
	using Discord.Rest;
	using Discord.WebSocket;
	using FC.Bot.Commands;
	using FC.Bot.Utils;
	using FC.Utils;

	public class EchoService : ServiceBase
	{
		public static async Task<List<RestUserMessage>> Echo(SocketTextChannel from, SocketTextChannel to, ulong fromMessageID, int count)
		{
			if (from is null)
				throw new ArgumentException("to");

			if (to is null)
				throw new ArgumentException("to");

			List<RestUserMessage> results = new List<RestUserMessage>();

			List<IMessage> messages = new List<IMessage>(await from.GetMessagesAsync(fromMessageID, Discord.Direction.Before, count).FlattenAsync());

			for (int i = messages.Count - 1; i >= 0; i--)
			{
				IMessage prevMessage = messages[i];
				if (prevMessage.Embeds.Count > 0)
				{
					await from.SendMessageAsync("Sorry, I can't echo message embeds.");
					continue;
				}

				if (prevMessage.Attachments.Count == 1)
				{
					string attachmentURL = prevMessage.Attachments.Getfirst().Url;
					string filePath = "./Temp/" + prevMessage.Id + Path.GetExtension(attachmentURL);

					await FileDownloader.Download(attachmentURL, filePath);

					results.Add(await to.SendFileAsync(filePath, prevMessage.Content));
				}

				if (!string.IsNullOrEmpty(prevMessage.Content))
				{
					results.Add(await to.SendMessageAsync(prevMessage.Content, prevMessage.IsTTS));
				}
			}

			return results;
		}

		[Command("Echo", Permissions.Administrators, "Copies a range of messages to a new channel.")]
		public async Task HandleEcho(CommandMessage message, int count, SocketTextChannel channel)
		{
			await Echo((SocketTextChannel)message.Channel, channel, message.Id, count);
		}

		[Command("Echo", Permissions.Administrators, "Copies a single message to a new channel.")]
		public async Task HandleEcho(CommandMessage message, SocketTextChannel channel)
		{
			await Echo((SocketTextChannel)message.Channel, channel, message.Id, 1);
		}

		[Command("Echo", Permissions.Administrators, "Copies a range of messages to the same channel.")]
		public async Task HandleEcho(CommandMessage message, int count)
		{
			await Echo((SocketTextChannel)message.Channel, (SocketTextChannel)message.Channel, message.Id, count);
		}

		[Command("Echo", Permissions.Administrators, "Copies a single message to the same channel.")]
		public async Task HandleEcho(CommandMessage message)
		{
			await Echo((SocketTextChannel)message.Channel, (SocketTextChannel)message.Channel, message.Id, 1);
		}

		[Command("Embed", Permissions.Administrators, "Creates an embed on the target channel")]
		public async Task HandleEmbedd(SocketTextChannel channel, string title, string content)
		{
			EmbedBuilder builder = new EmbedBuilder();
			builder.Description = content;
			builder.Title = title;

			await channel.SendMessageAsync(null, false, builder.Build());
		}
	}
}
