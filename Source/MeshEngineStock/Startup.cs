/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

	THIS FILE is ARR to LisiasT. No right other than using the generalted DLL on your machine is granted.
*/
using KSPe.Annotations;

namespace DistantObject.MeshEngine.Stock
{
	public class Startup
	{
		[UsedImplicitly]
		private void Start()
		{
			Log.force("MeshEngineStock {0}", Version.Text);
		}
	}
}
