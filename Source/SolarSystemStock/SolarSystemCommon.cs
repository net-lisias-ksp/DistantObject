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
using System;
using System.Collections.Generic;

using UnityEngine;

namespace DistantObject.SolarSystem
{
	internal static class CommonUtils
	{
		static public double CalculateSunBrightness(CelestialBody sun, double minimumSignificantBodySize, Camera cam)
		{
			double r = 1.0;

			Vector3d camPos = cam.transform.position;
			double camFov = cam.fieldOfView;
			Vector3d camAngle = cam.transform.forward;

			double sunAngularSize = calculateBodyAngularSize(sun, camPos);

			if (sunAngularSize > minimumSignificantBodySize)
			{
				Vector3d sunPosition = sun.position;

				// CSAngle = Camera to Sun angle
				double CSAngle = Math.Max(0.0, Vector3.Angle((sunPosition - camPos).normalized, camAngle) - sunAngularSize);
				CSAngle = 1.0 - Math.Min(1.0, Math.Max(0.0, (CSAngle - (camFov / 2.0))) / (camFov / 4.0));

				r -= (Math.Sqrt(sunAngularSize) * CSAngle);
			}

			return r;
		}

		static public double CalculatePlanetsBrightness(
			CelestialBody sun
			, double minimumSignificantBodySize
			, double minimumTargetRelativeAngle
			, double referenceBodySize
			, Camera cam
			)
		{
			double r = 1.0;

			Vector3d camPos = cam.transform.position;
			double camFov = cam.fieldOfView;
			Vector3d camAngle = cam.transform.forward;

			for (int i = 1; i < FlightGlobals.Bodies.Count; ++i)
			{
				CelestialBody body = FlightGlobals.Bodies[i];
				double bodySize = calculateBodyAngularSize(body, camPos);

				if (bodySize < minimumSignificantBodySize) continue;

				{
					Vector3d bodyPosition = body.position;
					Vector3d targetVectorToSun = sun.position - bodyPosition;
					Vector3d targetVectorToCam = camPos - bodyPosition;

					double targetRelAngle = (float)Vector3d.Angle(targetVectorToSun, targetVectorToCam);
					targetRelAngle = Math.Max(targetRelAngle, bodySize);
					targetRelAngle = Math.Min(targetRelAngle, minimumTargetRelativeAngle);
					targetRelAngle = 1.0 - ((targetRelAngle - bodySize) / (minimumTargetRelativeAngle - bodySize));

					double CBAngle = Math.Max(0.0, Vector3.Angle((bodyPosition - camPos).normalized, camAngle) - bodySize);
					CBAngle = 1.0 - Math.Min(1.0, Math.Max(0.0, (CBAngle - (camFov / 2.0)) - 5.0) / (camFov / 4.0));
					bodySize = Math.Min(bodySize, referenceBodySize);

					double colorScalar = 1.0 - (targetRelAngle * (Math.Sqrt(bodySize / referenceBodySize)) * CBAngle);
					r = Math.Min(r, colorScalar);
				}
			}

			return r;
		}

		static private double calculateBodyAngularSize(CelestialBody body, Vector3d camPos)
		{
			double sunRadius = body.Radius;
			double sunDist = body.GetAltitude(camPos) + sunRadius;
			return Math.Acos((Math.Sqrt(sunDist * sunDist - sunRadius * sunRadius) / sunDist)) * (double)Mathf.Rad2Deg;
		}
	}
}
