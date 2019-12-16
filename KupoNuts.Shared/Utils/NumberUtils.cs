﻿// This document is intended for use by Kupo Nut Brigade developers.

namespace KupoNuts.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public static class NumberUtils
	{
		public static string GetOrdinal(int num)
		{
			if (num <= 0)
				return string.Empty;

			switch (num % 100)
			{
				case 11:
				case 12:
				case 13: return @"th";
			}

			switch (num % 10)
			{
				case 1: return @"st";
				case 2: return @"nd";
				case 3: return @"rd";
				default: return @"th";
			}
		}
	}
}
