﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Manager.Server.Controllers
{
	using System.Net.Http;
	using Microsoft.AspNetCore.Mvc;

	[ApiController]
	[Route("[controller]")]
	public class SettingsAPIController : ControllerBase
	{
		[HttpGet]
		public Settings Get()
		{
			return Database.Load().Settings;
		}

		[HttpPost]
		public void Post(Settings settings)
		{
			Database db = Database.Load();
			db.Settings = settings;
			db.Save();
		}
	}
}