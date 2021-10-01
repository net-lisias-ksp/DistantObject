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
using System.Reflection;
using UnityEngine;

namespace DistantObject.MeshEngine.Contract
{
	public static class Module
	{
		public interface IBlackList
		{
			List<string> Get();
		}

		public interface Interface
		{
			string GetImplementedModuleName();
			GameObject Render(GameObject go, ProtoPartSnapshot part, AvailablePart avPart);
		}

		private static readonly HashSet<string> BLACKLIST = new HashSet<string>();
		private static readonly Dictionary<string, Interface> MAP = new Dictionary<string, Interface>();
		internal static void Init()
		{
			foreach (Type type in KSPe.Util.SystemTools.TypeSearch.ByInterface(typeof(DistantObject.MeshEngine.Contract.Module.IBlackList)))
			{
				ConstructorInfo ctor = type.GetConstructor(new Type[] { });
				IBlackList i = (IBlackList) ctor.Invoke(new object[] { });
				foreach (string s in i.Get())
					BLACKLIST.Add(s);
			}

			foreach (Type type in KSPe.Util.SystemTools.TypeSearch.ByInterface(typeof(DistantObject.MeshEngine.Contract.Module.Interface)))
			{
				ConstructorInfo ctor = type.GetConstructor(new Type[] { });
				Interface i = (Interface) ctor.Invoke(new object[] { });
				MAP.Add(i.GetImplementedModuleName(), i);
			}
		}

		internal static bool IsBlackListed(ProtoPartSnapshot part)
		{
			foreach (string moduleName in BLACKLIST)
				if (part.modules.Find(n => n.moduleName == moduleName) != null)
					return true;
			return false;
		}

		internal static GameObject Render(GameObject mesh, ProtoPartSnapshot part, AvailablePart avPart, ProtoPartModuleSnapshot module)
		{
			if (!MAP.ContainsKey(module.moduleName)) return mesh;
			return MAP[module.moduleName].Render(mesh, part, avPart);
		}
	}
}
