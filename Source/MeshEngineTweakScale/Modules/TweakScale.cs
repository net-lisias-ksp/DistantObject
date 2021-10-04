/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

	THIS FILE is ARR to LisiasT. No right other than using the generalted DLL on your machine is granted.
*/
using UnityEngine;

namespace DistantObject.MeshEngine.TweakScale.Modules
{
	public class Light : DistantObject.MeshEngine.Contract.Module.Interface
	{
		private const string MODULE_NAME = "TweakScale";

		public Light()
		{
		}

		string Contract.Module.Interface.GetImplementedModuleName()
		{
			return MODULE_NAME;
		}

		GameObject Contract.Module.Interface.Render(GameObject mesh, ProtoPartSnapshot part, AvailablePart avPart)
		{
			ProtoPartModuleSnapshot tweakScale = part.modules.Find(n => n.moduleName == MODULE_NAME);

			float defaultScale = float.Parse(tweakScale.moduleValues.GetValue("defaultScale"));
			float currentScale = float.Parse(tweakScale.moduleValues.GetValue("currentScale"));
			float ratio = currentScale / defaultScale;
			if (ratio > 0.001)
			{
				mesh.transform.localScale = new Vector3(ratio, ratio, ratio);
				Log.dbg("localScale after {0}", mesh.transform.localScale);
			}
			return mesh;
		}
	}
}
