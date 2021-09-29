/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
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
using D = UnityEngine.Debug;

namespace DistantObject
{
	public static class Log
	{
		private static readonly string PREFIX = string.Format("[{0}] ", typeof(Log).Namespace);
		public static void force (string msg, params object [] @params)
		{
			D.LogFormat(PREFIX + msg, @params);
		}

		public static void info(string msg, params object[] @params)
		{
			D.LogFormat(PREFIX + "INFO : " + msg, @params);
		}

		public static void warn(string msg, params object[] @params)
		{
			D.LogWarningFormat(PREFIX + "WARN : " + msg, @params);
		}

		public static void detail(string msg, params object[] @params)
		{
			D.LogFormat(PREFIX + "DETAIL : " + msg, @params);
		}

		public static void trace(string msg, params object[] @params)
		{
			if (!DistantObjectSettings.debugMode) return;
			D.LogFormat(PREFIX + "TRACE : " + msg, @params);
		}

		public static void error(string msg, params object[] @params)
		{
			D.LogErrorFormat(PREFIX + "ERROR : " + msg, @params);
		}

		public static void error(Exception e)
		{
			D.LogException(e);
		}

		public static void error(Exception e, string msg, params object[] @params)
		{
			error(msg, @params);
			error(e);
		}

		[ConditionalAttribute("DEBUG")]
		public static void dbg(string msg, params object[] @params)
		{
			D.LogFormat(PREFIX + "TRACE : " + msg, @params);
		}
	}
}
