﻿// Copyright (c) FCChan. All rights reserved.
//
// Licensed under the MIT license.

namespace System.Net.Http
{
	using System.Threading.Tasks;
	using FC.Manager.Client.RPC;

	public static class HttpClientExtensions
	{
		public static async Task<TResult> Invoke<TResult>(this HttpClient self, string method, params object[] param)
		{
			return await RPCService.Invoke<TResult>(self, method, param);
		}
	}
}
