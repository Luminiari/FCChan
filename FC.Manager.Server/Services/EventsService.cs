﻿// Copyright (c) FCChan. All rights reserved.
//
// Licensed under the MIT license.

namespace FC.Manager.Server.Services
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using FC.Data;
	using FC.Events;
	using FC.Manager.Server.RPC;

	public class EventsService : ServiceBase
	{
		private Table<Event> eventsDb = new Table<Event>("KupoNuts_Events", 1);

		public override async Task Initialize()
		{
			await this.eventsDb.Connect();
		}

		[GuildRpc]
		public async Task<List<Event>> GetEvents(ulong guildId)
		{
			Dictionary<string, object> search = new Dictionary<string, object>();
			search.Add("ServerIdStr", guildId.ToString());
			return await this.eventsDb.LoadAll(search);
		}

		[GuildRpc]
		public async Task DeleteEvent(ulong guildId, string eventId)
		{
			await this.eventsDb.Delete(eventId);
		}

		[GuildRpc]
		public async Task UpdateEvent(ulong guildId, Event evt)
		{
			evt.ServerIdStr = guildId.ToString();
			await this.eventsDb.Save(evt);
		}

		[GuildRpc]
		public Task<EventsSettings> GetSettings(ulong guildId)
		{
			return SettingsService.GetSettings<EventsSettings>(guildId);
		}

		[GuildRpc]
		public Task SaveSettings(ulong guildId, EventsSettings settings)
		{
			// Don't let clients change this!
			settings.Guild = guildId;

			return SettingsService.SaveSettings(settings);
		}
	}
}
