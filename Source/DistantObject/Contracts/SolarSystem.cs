/*
		This file is part of Distant Object Enhancement /L
			© 2021-2024 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

		Distant Object Enhancement /L is double licensed, as follows:

		* SKL 1.0 : https://ksp.lisias.net/SKL-1_0.txt
		* GPL 2.0 : https://www.gnu.org/licenses/gpl-2.0.txt

		And you are allowed to choose the License that better suit your needs.

		Distant Object Enhancement /L is distributed in the hope that it will
		be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
		of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

		You should have received a copy of the SKL Standard License 1.0
		along with Distant Object Enhancement /L.
		If not, see <https://ksp.lisias.net/SKL-1_0.txt>.

		You should have received a copy of the GNU General Public License 2.0
		along with Distant Object Enhancement /L.
		If not, see <https://www.gnu.org/licenses/>.
*/
using UnityEngine;

namespace DistantObject.Contract
{
	public static class SolarSystemEngine
	{
		public interface Interface
		{
			Vector3d GetSunPosition();

			double CalculateSunBrightness(double minimumSignificantBodySize, Camera cam);

			double CalculatePlanetsBrightness(
				double minimumSignificantBodySize
				, double minimumTargetRelativeAngle
				, double referenceBodySize
				, Camera cam
				);

			double CalculateSunCoronaBrightness(double minimumSignificantBodySize, Camera cam);
		}

		private static Interface INSTANCE;
		internal static Interface Instance => INSTANCE ?? (INSTANCE = Create()) ;

		private static Interface Create()
		{
			string assemblyToBeLoaded = KSPe.Util.SystemTools.Assembly.Exists.ByName("Kopernicus") ? "SolarSystemKopernicus" : "SolarSystemStock";
			using (KSPe.Util.SystemTools.Assembly.Loader a = new KSPe.Util.SystemTools.Assembly.Loader<Startup>())
			{
				a.LoadAndStartup(assemblyToBeLoaded);
			}

			return (Interface)KSPe.Util.SystemTools.Interface.CreateInstanceByInterface(typeof(Interface));
		}
	}
}
