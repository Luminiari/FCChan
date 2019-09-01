﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Events
{
	public class Attendee
	{
		public Attendee()
		{
		}

		public Attendee(string eventId, string userId)
		{
			this.EventId = eventId;
			this.UserId = userId;
		}

		public string? EventId { get; set; }

		public string? UserId { get; set; }

		public int? Status { get; set; }

		public string? RemindTime { get; set; }
	}
}
