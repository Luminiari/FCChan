﻿// Copyright (c) FCChan. All rights reserved.
//
// Licensed under the MIT license.

namespace FC.Bot.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	using Discord;
	using Discord.WebSocket;
	using FC.Bot.Commands;
	using FC.Utils;

	public class LogService : ServiceBase
	{
		private const string FileLocation = "Log.txt";
		private bool lockFile = false;

		public override Task Initialize()
		{
			Log.MessageLogged += this.OnMessageLogged;
			Log.ExceptionLogged += this.OnExceptionLogged;

			Program.DiscordClient.UserJoined += this.DiscordClient_UserJoined;
			Program.DiscordClient.UserLeft += this.DiscordClient_UserLeft;
			Program.DiscordClient.UserBanned += this.DiscordClient_UserBanned;
			Program.DiscordClient.UserUnbanned += this.DiscordClient_UserUnbanned;

			if (File.Exists(FileLocation))
				File.Delete(FileLocation);

			return base.Initialize();
		}

		public override async Task Shutdown()
		{
			Program.DiscordClient.UserJoined -= this.DiscordClient_UserJoined;
			Program.DiscordClient.UserLeft -= this.DiscordClient_UserLeft;
			Program.DiscordClient.UserBanned -= this.DiscordClient_UserBanned;
			Program.DiscordClient.UserUnbanned -= this.DiscordClient_UserUnbanned;

			await base.Shutdown();
		}

		[Command("Log", Permissions.Administrators, "posts the bot log")]
		public async Task PostLog(CommandMessage message)
		{
			this.lockFile = true;
			await message.Channel.SendFileAsync(FileLocation);
			this.lockFile = false;
		}

		private async Task DiscordClient_UserJoined(SocketGuildUser user)
		{
			await this.PostMessage(user, Color.Green, "Joined");
		}

		private async Task DiscordClient_UserLeft(SocketGuildUser user)
		{
			await this.PostMessage(user, Color.LightGrey, "Left");
		}

		private async Task DiscordClient_UserBanned(SocketUser user, SocketGuild guild)
		{
			if (user is IGuildUser guildUser)
			{
				await this.PostMessage(guildUser, Color.Red, "Was Banned");
			}
			else
			{
				throw new Exception("User is not a guild user: " + user);
			}
		}

		private async Task DiscordClient_UserUnbanned(SocketUser user, SocketGuild guild)
		{
			if (user is IGuildUser guildUser)
			{
				await this.PostMessage(guildUser, Color.Orange, "Was Unbanned");
			}
			else
			{
				throw new Exception("User is not a guild user: " + user);
			}
		}

		private async Task PostMessage(IGuildUser user, Color color, string message)
		{
			if (!ulong.TryParse(Settings.Load().UserLogChannel, out ulong channelId))
				return;

			SocketTextChannel? channel = Program.DiscordClient.GetChannel(channelId) as SocketTextChannel;

			if (channel == null)
				return;

			// don't push logs to different guilds.
			if (user.GuildId != channel.Guild.Id)
				return;

			EmbedBuilder builder = new EmbedBuilder();
			builder.Color = color;
			builder.Title = user.Username + " " + message + " " + user.Guild.Name;
			builder.Timestamp = DateTimeOffset.Now;
			builder.ThumbnailUrl = user.GetAvatarUrl();

			if (user is SocketGuildUser guildUser)
			{
				builder.Title = guildUser.Nickname + " (" + user.Username + ") " + message;
				builder.AddField("Joined", TimeUtils.GetDateString(guildUser.JoinedAt), true);
			}

			builder.AddField("Created", TimeUtils.GetDateString(user.CreatedAt), true);

			builder.Footer = new EmbedFooterBuilder();
			builder.Footer.Text = "ID: " + user.Id;

			await channel.SendMessageAsync(null, false, builder.Build());
		}

		private void OnExceptionLogged(string str)
		{
			this.OnMessageLogged(str);
		}

		private void OnMessageLogged(string str)
		{
			// TODO: we should make this async so we can wait for the file to unlock...
			if (this.lockFile)
			{
				Log.Write("Log file is locked.", "Log");
				return;
			}

			try
			{
				File.AppendAllText(FileLocation, str + "\n");
			}
			catch (Exception)
			{
				Console.WriteLine("Unable to write log file");
			}
		}
	}
}
