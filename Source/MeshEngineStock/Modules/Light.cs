/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

	THIS FILE is ARR to LisiasT. No right other than using the generalted DLL on your machine is granted.
*/
using UnityEngine;

namespace DistantObject.MeshEngine.Stock.Modules
{
	public class Light : DistantObject.MeshEngine.Contract.Module.Interface
	{
		private const string MODULE_NAME = "ModuleLight";

		public Light()
		{
		}

		string Contract.Module.Interface.GetImplementedModuleName()
		{
			return MODULE_NAME;
		}

		GameObject Contract.Module.Interface.Render(GameObject mesh, ProtoPartSnapshot part, AvailablePart avPart)
		{
			ProtoPartModuleSnapshot light = part.modules.Find(n => n.moduleName == MODULE_NAME);
			//Oddly enough the light already renders no matter what, so we'll kill the module if it's suppsed to be turned off
			if (light.moduleValues.GetValue("isOn") == "False")
			{
				Object.Destroy(mesh.GetComponentInChildren<UnityEngine.Light>());
			}
			return mesh;
		}
	}
}
