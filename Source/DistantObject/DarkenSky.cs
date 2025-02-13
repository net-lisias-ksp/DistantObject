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

using DistantObject.Contract;

using KSPe.Annotations;
using UnityEngine;

namespace DistantObject
{
    //Peachoftree: It was EveryScene so the sky would darken in places like the starting menu and the tracking center, not just flight and map veiw 
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class DarkenSky : MonoBehaviour
    {
		private static DarkenSky INSTANCE = null;
		internal static DarkenSky Instance => INSTANCE;

        private Color galaxyColor = Color.black;
        private float glareFadeLimit = 0.0f;
        private bool restorableGalaxyCube = false;

		[UsedImplicitly]
        private void Awake()
        {
            INSTANCE = this;

            restorableGalaxyCube = false;
        }

		[UsedImplicitly]
		private void Start()
		{
			Settings.Instance.Load();
			Settings.Instance.Commit();

			if (null != GalaxyCubeControl.Instance)
			{
				restorableGalaxyCube = true;
				galaxyColor = GalaxyCubeControl.Instance.maxGalaxyColor;
				glareFadeLimit = GalaxyCubeControl.Instance.glareFadeLimit;

				if (Settings.Instance.SkyboxBrightness.changeSkybox)
				{
					GalaxyCubeControl.Instance.maxGalaxyColor = new Color(
							(float)Settings.Instance.SkyboxBrightness.maxBrightness,
							(float)Settings.Instance.SkyboxBrightness.maxBrightness,
							(float)Settings.Instance.SkyboxBrightness.maxBrightness
						);
					GalaxyCubeControl.Instance.glareFadeLimit = 1f;
				}
			}
		}

		[UsedImplicitly]
		private void OnDestroy()
        {
            if (GalaxyCubeControl.Instance != null && restorableGalaxyCube)
            {
                GalaxyCubeControl.Instance.maxGalaxyColor = galaxyColor;
                GalaxyCubeControl.Instance.glareFadeLimit = glareFadeLimit;
                restorableGalaxyCube = false;
            }

            INSTANCE = null;
        }

		[UsedImplicitly]
        private void Update()
        {
            if (null == GalaxyCubeControl.Instance) return;
            if (MapView.MapIsEnabled)
            {
				GalaxyCubeControl.Instance.maxGalaxyColor = this.galaxyColor;
				GalaxyCubeControl.Instance.glareFadeLimit = this.glareFadeLimit;
                return;
            }

			double targetColorScalar = SolarSystemEngine.Instance.CalculateSunBrightness(
				Settings.Instance.SkyboxBrightness.minimumSignificantBodySize
				, FlightCamera.fetch.mainCamera
				);
			targetColorScalar = Math.Min(targetColorScalar,
					SolarSystemEngine.Instance.CalculatePlanetsBrightness(
						Settings.Instance.SkyboxBrightness.minimumSignificantBodySize
						, Settings.Instance.SkyboxBrightness.minimumTargetRelativeAngle
						, Settings.Instance.SkyboxBrightness.referenceBodySize
						, FlightCamera.fetch.mainCamera
					)
				);
			targetColorScalar = Math.Max(targetColorScalar,
					SolarSystemEngine.Instance.CalculateSunCoronaBrightness(
						Settings.Instance.SkyboxBrightness.minimumSignificantBodySize
						, FlightCamera.fetch.mainCamera
					)
				);
			{
				float c = (float)Settings.Instance.SkyboxBrightness.maxBrightness;
				Color color = new Color(c,c,c) * (float)targetColorScalar;
				GalaxyCubeControl.Instance.maxGalaxyColor = color;
			}
        }

		internal void SetActiveTo(bool renderVessels)
		{
			if (renderVessels)
				this.Activate();
			else
				this.Deactivate();
		}

		private void Activate()
		{
			Log.trace("DarkenSky enabled");
			this.enabled = true;
		}

		private void Deactivate()
		{
			Log.trace("DarkenSky disabled");
			this.enabled = false;

			if (this.restorableGalaxyCube && null != GalaxyCubeControl.Instance)
			{
				GalaxyCubeControl.Instance.maxGalaxyColor = this.galaxyColor;
				GalaxyCubeControl.Instance.glareFadeLimit = this.glareFadeLimit;
			}
		}
	}
}
