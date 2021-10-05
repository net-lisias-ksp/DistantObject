/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

	THIS FILE is ARR to LisiasT. No right other than using the generalted DLL on your machine is granted.
*/
using System;
using System.Collections.Generic;

namespace DistantObject.MeshEngine.Stock
{
	public class BlackList:DistantObject.MeshEngine.Contract.Module.IBlackList
	{
		private static readonly string[] BLACKLIST = { "LaunchClamp", "CModuleLinkedMesh" };

		public BlackList()
		{
		}

		List<string> Contract.Module.IBlackList.Get()
		{
			return new List<string>(BLACKLIST);
		}
	}
}
