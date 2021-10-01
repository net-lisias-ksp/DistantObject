/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

	THIS FILE is ARR to LisiasT. No right other than using the generalted DLL on your machine is granted.
*/
using KSPe.Annotations;

namespace DistantObject.MeshEngine
{
	public class Startup
	{
		[UsedImplicitly]
		private void Awake()
		{
			using (KSPe.Util.SystemTools.Assembly.Loader a = new KSPe.Util.SystemTools.Assembly.Loader<DistantObject.Startup>())
			{
				a.LoadAndStartup("MeshEngineStock");
				a.LoadAndStartup("MeshEngineTweakScale");
			}

			Database.Init();
			Contract.Module.Init();
		}

		[UsedImplicitly]
		private void Start()
		{
			Log.force("MeshEngine {0} ready.", Version.Text);
		}
	}
}
