﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using KupoNuts.Events;
	using NodaTime;

	public static class TimeUtils
	{
		public static Instant Now
		{
			get
			{
				return SystemClock.Instance.GetCurrentInstant();
			}
		}

		public static DateTimeZone NZST
		{
			get
			{
				return GetTimeZone("Pacific/Auckland");
			}
		}

		public static DateTimeZone AWST
		{
			get
			{
				return GetTimeZone("Australia/Perth");
			}
		}

		public static DateTimeZone ACST
		{
			get
			{
				return GetTimeZone("Australia/Adelaide");
			}
		}

		public static DateTimeZone AEST
		{
			get
			{
				return GetTimeZone("Australia/Sydney");
			}
		}

		public static IsoDayOfWeek ToIsoDay(Event.Days day)
		{
			switch (day)
			{
				case Event.Days.Monday: return IsoDayOfWeek.Monday;
				case Event.Days.Tuesday: return IsoDayOfWeek.Tuesday;
				case Event.Days.Wednesday: return IsoDayOfWeek.Wednesday;
				case Event.Days.Thursday: return IsoDayOfWeek.Thursday;
				case Event.Days.Friday: return IsoDayOfWeek.Friday;
				case Event.Days.Saturday: return IsoDayOfWeek.Saturday;
				case Event.Days.Sunday: return IsoDayOfWeek.Sunday;
			}

			throw new Exception("Unknown day: " + day);
		}

		public static string GetDateTimeString(Instant dt)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine(GetDateString(dt));
			builder.Append(GetTimeString(dt));
			return builder.ToString();
		}

		public static string GetDateString(Instant dt)
		{
			return dt.InZone(AEST).ToString("dddd dd MMMM", CultureInfo.InvariantCulture);
		}

		public static string GetTimeString(Instant? dt)
		{
			if (dt == null)
				return string.Empty;

			StringBuilder builder = new StringBuilder();
			builder.Append(dt?.InZone(AWST).ToString("**h:mm", CultureInfo.InvariantCulture));
			builder.Append(dt?.InZone(AWST).ToString("tt**", CultureInfo.InvariantCulture).ToLower());
			builder.Append(" AWST");
			builder.Append(" - ");
			builder.Append(dt?.InZone(ACST).ToString("**h:mm", CultureInfo.InvariantCulture));
			builder.Append(dt?.InZone(ACST).ToString("tt**", CultureInfo.InvariantCulture).ToLower());
			builder.Append(" ACST");
			builder.Append(" - ");
			builder.Append(dt?.InZone(AEST).ToString("**h:mm", CultureInfo.InvariantCulture));
			builder.Append(dt?.InZone(AEST).ToString("tt**", CultureInfo.InvariantCulture).ToLower());
			builder.Append(" AEST");
			builder.Append(" - ");
			builder.Append(dt?.InZone(NZST).ToString("**h:mm", CultureInfo.InvariantCulture));
			builder.Append(dt?.InZone(NZST).ToString("tt**", CultureInfo.InvariantCulture).ToLower());
			builder.Append(" NZST");
			return builder.ToString();
		}

		public static string? GetDurationString(Duration? timeNull)
		{
			if (timeNull == null)
				return null;

			Duration time = (Duration)timeNull;
			StringBuilder builder = new StringBuilder();

			if (time.Days == 0 && time.Hours == 0 && time.Minutes == 0)
				return " now";

			if (time.Days == 1)
			{
				builder.Append(" ");
				builder.Append(time.Days);
				builder.Append(" day");
			}
			else if (time.Days > 1)
			{
				builder.Append(" ");
				builder.Append(time.Days);
				builder.Append(" days");
			}

			if (time.Hours == 1)
			{
				builder.Append(" ");
				builder.Append(time.Hours);
				builder.Append(" hour");
			}
			else if (time.Hours > 1)
			{
				builder.Append(" ");
				builder.Append(time.Hours);
				builder.Append(" hours");
			}

			if (time.Minutes == 1)
			{
				builder.Append(" ");
				builder.Append(time.Minutes);
				builder.Append(" minute");
			}
			else if (time.Minutes > 1)
			{
				builder.Append(" ");
				builder.Append(time.Minutes);
				builder.Append(" minutes");
			}

			return builder.ToString();
		}

		public static Instant RoundInstant(Instant instant)
		{
			DateTimeZone zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			ZonedDateTime zdt = instant.InZone(zone);

			instant += Duration.FromNanoseconds(-zdt.NanosecondOfSecond);
			instant += Duration.FromSeconds(-zdt.Second);

			int minute = zdt.Minute;
			int newMinute = (int)Math.Round(minute / 15.0) * 15;
			Duration minutechange = Duration.FromMinutes(newMinute - minute);
			instant += minutechange;

			return instant;
		}

		public static string GetDayName(int daysAway)
		{
			if (daysAway == 0)
				return "Today";

			if (daysAway == 1)
				return "Tomorrow";

			DateTimeZone zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
			Instant then = TimeUtils.Now + Duration.FromDays(daysAway);

			if (daysAway >= 7)
			{
				return GetDateString(then);
			}
			else
			{
				IsoDayOfWeek day = then.InZone(zone).DayOfWeek;
				return day.ToString();
			}
		}

		public static int GetDaysTill(Instant instant, DateTimeZone zone)
		{
			ZonedDateTime zdt = TimeUtils.Now.InZone(zone);
			LocalDateTime ldt = zdt.LocalDateTime;
			ldt = ldt.Date.AtMidnight();
			zdt = ldt.InZoneLeniently(zone);

			Duration duration = instant - zdt.ToInstant();

			return (int)Math.Floor(duration.TotalDays);
		}

		public static string GetDurationString(double duration)
		{
			if (duration == 0)
				return "0 minutes";

			int hours = (int)duration;
			int minutes = (int)((duration - (double)hours) * 60.0);

			StringBuilder builder = new StringBuilder();

			if (hours == 1)
			{
				builder.Append(hours);
				builder.Append(" hour ");
			}
			else if (hours > 1)
			{
				builder.Append(hours);
				builder.Append(" hours ");
			}

			if (minutes == 1)
			{
				builder.Append(minutes);
				builder.Append(" minute ");
			}
			else if (minutes > 1)
			{
				builder.Append(minutes);
				builder.Append(" minutes ");
			}

			return builder.ToString();
		}

		private static DateTimeZone GetTimeZone(string id)
		{
			DateTimeZone zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(id);
			if (zone == null)
				throw new Exception("Failed to get time zone: \"" + id + "\"");

			return zone;
		}
	}
}