/*
		This file is part of Distant Object Enhancement /L
			© 2021-2024 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

	THIS FILE is licensed to you under:

	* WTFPL - http://www.wtfpl.net
		* Everyone is permitted to copy and distribute verbatim or modified
 			copies of this license document, and changing it is allowed as long
			as the name is changed.

	THIS FILE is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
*/
using System;
using System.Diagnostics;
using KSPe.Util.Log;

namespace DistantObject
{
	public static class Log
	{
		private static readonly Logger log = Logger.CreateForType<Startup>();

		public static Level level {
			get { return log.level; }
			set {log.level = value; }
		}

		public static void force (string msg, params object [] @params)
		{
			log.force(msg, @params);
		}

		public static void info(string msg, params object[] @params)
		{
			log.info(msg, @params);
		}

		public static void warn(string msg, params object[] @params)
		{
			log.warn(msg, @params);
		}

		public static void detail(string msg, params object[] @params)
		{
			log.detail(msg, @params);
		}

		public static void trace(string msg, params object[] @params)
		{
			log.trace(msg, @params);
		}

		public static void error(string msg, params object[] @params)
		{
			log.error(msg, @params);
		}

		public static void error(Exception e, object offended)
		{
			log.error(offended, e);
		}

		public static void error(Exception e, string msg, params object[] @params)
		{
			log.error(e, msg, @params);
		}

		public static void assert(Func<bool> f, string msg, params object[] @params)
		{
			log.assert(f, msg, @params);
		}

		[ConditionalAttribute("DEBUG")]
		public static void dbg(string msg, params object[] @params)
		{
			log.detail(msg, @params);
		}
	}
}
