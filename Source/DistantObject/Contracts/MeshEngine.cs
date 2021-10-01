/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky
*/
using System;
using System.Reflection;

namespace DistantObject.Contract
{
	public static class MeshEngine
	{
		public interface Interface
		{
			void Draw();
			void Destroy();
		}

		internal static Interface CreateFor(Vessel vessel)
		{
			Type type = KSPe.Util.SystemTools.TypeFinder.FindByInterface(typeof(DistantObject.Contract.MeshEngine.Interface));
			ConstructorInfo ctor = type.GetConstructor(new[] { typeof(Vessel) });
			return (Interface) ctor.Invoke(new object[] { vessel });
		}
	}
}
