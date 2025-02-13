/*
		This file is part of Distant Object Enhancement /L
			© 2020-2025 LisiasT
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

using DistantObject.Contract;

namespace DistantObject.SolarSystem.Stock
{
	public partial class SolarSystem : SolarSystemEngine.Interface
	{
		public SolarSystem() { }

		Vector3d SolarSystemEngine.Interface.GetSunPosition() => FlightGlobals.Bodies[0].position;

		double SolarSystemEngine.Interface.CalculatePlanetsBrightness(double minimumSignificantBodySize, double minimumTargetRelativeAngle, double referenceBodySize, Camera cam)
		{
			CelestialBody sun = FlightGlobals.Bodies[0];
			return CommonUtils.CalculatePlanetsBrightness(sun, minimumSignificantBodySize, minimumTargetRelativeAngle, referenceBodySize, cam);
		}

		double SolarSystemEngine.Interface.CalculateSunBrightness(double minimumSignificantBodySize, Camera cam)
		{
			CelestialBody sun = FlightGlobals.Bodies[0];
			return CommonUtils.CalculateSunBrightness(sun, minimumSignificantBodySize, cam);
		}

		double SolarSystemEngine.Interface.CalculateSunCoronaBrightness(double minimumSignificantBodySize, Camera cam)
		{
			CelestialBody sun = FlightGlobals.Bodies[0];
			return CommonUtils.CalculateSunCoronaBrightness(sun, minimumSignificantBodySize, cam);
		}

	}
}
