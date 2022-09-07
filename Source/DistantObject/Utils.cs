/*
		This file is part of Distant Object Enhancement /L
			© 2021-2022 LisiasT
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
using UnityEngine;

using KSPe;
using IO = KSPe.IO;


namespace DistantObject
{
    class Utility
    {
        public static Vector4 RGB2HSL(Color rgba)
        {
            float h = 0.0f, s = 0.0f, l = 0.0f;
            float r = rgba.r;
            float g = rgba.g;
            float b = rgba.b;

            float v;
            float m;
            float vm;

            float r2, g2, b2;


            v = Mathf.Max(r, g);
            v = Mathf.Max(v, b);

            m = Mathf.Min(r, g);
            m = Mathf.Min(m, b);

            l = (m + v) / 2.0f;

            if (l <= 0.0f)
            {
                return new Vector4(0.0f, 0.0f, 0.0f, rgba.a);
            }

            vm = v - m;

            s = vm;

            if (s > 0.0f)
            {
                s /= (l <= 0.5f) ? (v + m) : (2.0f - v - m);
            }
            else
            {
                return new Vector4(0.0f, 0.0f, l, rgba.a);
            }

            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;

            if (r == v)
            {
                h = (g == m ? 5.0f + b2 : 1.0f - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0f + r2 : 3.0f - b2);
            }
            else
            {
                h = (r == m ? 3.0f + g2 : 5.0f - r2);
            }

            h /= 6.0f;

            return new Vector4(h, s, l, rgba.a);
        }

    }

    class Constants
    {
        static internal readonly string DistantObject = "Distant Object Enhancement v" + Version.Text;
		internal const string SETTINGS_FILE = "Settings.cfg";
		static internal readonly string REFERENCE_CONFIG_PATHNAME = IO.Hierarchy<Startup>.GAMEDATA.Solve("PluginData", SETTINGS_FILE);
		static internal readonly string CONFIG_DIRECTORY = IO.Hierarchy<Startup>.PLUGINDATA.Solve();
		static internal readonly string CONFIG_PATHNAME = IO.Hierarchy<Startup>.PLUGINDATA.Solve(SETTINGS_FILE);
    }

    class DistantObjectSettings
    {
        //--- Config file values
        public struct DistantFlare
        {
            static public bool flaresEnabled = true;
            static public bool ignoreDebrisFlare = false;
            static public bool showNames = false;
            static public float flareSaturation = 1.0f;
            static public float flareSize = 1.0f;
            static public float flareBrightness = 1.0f;
            static readonly public string situations = "ORBITING,SUB_ORBITAL,ESCAPING,DOCKED,FLYING";
            static public float debrisBrightness = 0.15f;
        }

        public enum ERenderMode
        {
            RenderTargetOnly = 0,
            RenderAll = 1,
            RenderAllDontForget = 2,
            SIZE = 3
        }

        public struct DistantVessel
        {
            static public bool renderVessels = false;
            static public float maxDistance = 750000.0f;
            static public ERenderMode renderMode = ERenderMode.RenderTargetOnly;
            static public bool ignoreDebris = false;
        }

        public struct SkyboxBrightness
        {
            static public bool changeSkybox = true;
            static public float maxBrightness = 0.25f;
        }

        static public bool debugMode = false;
        static public bool useToolbar = true;
        static public bool useAppLauncher = true;
        static public bool onlyInSpaceCenter = false;

        //--- Internal values
        static private bool hasLoaded = false;
		static public void LoadConfig()
		{
			if (hasLoaded) return;

			ConfigNode configNode = ConfigNode.Load(Constants.CONFIG_PATHNAME);
			if (null == configNode) configNode = ConfigNode.Load(Constants.REFERENCE_CONFIG_PATHNAME);
			if (null == configNode) return;

			ConfigNodeWithSteroids settings = ConfigNodeWithSteroids.from(configNode);

			debugMode = settings.GetValue<bool>("debugMode", debugMode);
			useToolbar = settings.GetValue<bool>("useToolbar", useToolbar);
			useAppLauncher = settings.GetValue<bool>("useAppLauncher", useAppLauncher);
			onlyInSpaceCenter = settings.GetValue<bool>("onlyInSpaceCenter", onlyInSpaceCenter);

			if (settings.HasNode("DistantFlare"))
			{
				ConfigNodeWithSteroids distantFlare = ConfigNodeWithSteroids.from(settings.GetNode("DistantFlare"));
				DistantFlare.flaresEnabled = distantFlare.GetValue<bool>("flaresEnabled", DistantFlare.flaresEnabled);
				DistantFlare.flareSaturation = distantFlare.GetValue<float>("flareSaturation", DistantFlare.flareSaturation);
				DistantFlare.flareSize = distantFlare.GetValue<float>("flareSize", DistantFlare.flareSize);
				DistantFlare.flareBrightness = distantFlare.GetValue<float>("flareBrightness", DistantFlare.flareBrightness);
				DistantFlare.ignoreDebrisFlare = distantFlare.GetValue<bool>("ignoreDebrisFlare", DistantFlare.ignoreDebrisFlare);
				DistantFlare.debrisBrightness = distantFlare.GetValue<float>("debrisBrightness", DistantFlare.debrisBrightness);
				DistantFlare.showNames = distantFlare.GetValue<bool>("showNames", DistantFlare.showNames);
			}

			if (settings.HasNode("DistantVessel"))
			{
				ConfigNodeWithSteroids distantVessel = ConfigNodeWithSteroids.from(settings.GetNode("DistantVessel"));
				DistantVessel.renderVessels = distantVessel.GetValue<bool>("renderVessels", DistantVessel.renderVessels);
				DistantVessel.maxDistance = distantVessel.GetValue<float>("maxDistance", DistantVessel.maxDistance);
				DistantVessel.renderMode = (ERenderMode)distantVessel.GetValue<int>("renderMode", (int)DistantVessel.renderMode);
				DistantVessel.ignoreDebris = distantVessel.GetValue<bool>("ignoreDebris", DistantVessel.ignoreDebris);
			}

			if (settings.HasNode("SkyboxBrightness"))
			{
				ConfigNodeWithSteroids skyboxBrightness = ConfigNodeWithSteroids.from(settings.GetNode("SkyboxBrightness"));
				SkyboxBrightness.changeSkybox = skyboxBrightness.GetValue<bool>("changeSkybox", SkyboxBrightness.changeSkybox);
				SkyboxBrightness.maxBrightness = skyboxBrightness.GetValue<float>("maxBrightness", SkyboxBrightness.maxBrightness);
			}

			hasLoaded = true;
			Commit();
		}

        static public void SaveConfig()
        {
            ConfigNode settings = new ConfigNode();

            settings.AddValue("debugMode", debugMode);
            settings.AddValue("useToolbar", useToolbar);
            settings.AddValue("useAppLauncher", useAppLauncher);
            settings.AddValue("onlyInSpaceCenter", onlyInSpaceCenter);

            ConfigNode distantFlare = settings.AddNode("DistantFlare");
            distantFlare.AddValue("flaresEnabled", DistantFlare.flaresEnabled);
            distantFlare.AddValue("flareSaturation", DistantFlare.flareSaturation);
            distantFlare.AddValue("flareSize", DistantFlare.flareSize);
            distantFlare.AddValue("flareBrightness", DistantFlare.flareBrightness);
            distantFlare.AddValue("ignoreDebrisFlare", DistantFlare.ignoreDebrisFlare);
            distantFlare.AddValue("debrisBrightness", DistantFlare.debrisBrightness);
            distantFlare.AddValue("situations", DistantFlare.situations);
            distantFlare.AddValue("showNames", DistantFlare.showNames);

            ConfigNode distantVessel = settings.AddNode("DistantVessel");
            distantVessel.AddValue("renderVessels", DistantVessel.renderVessels);
            distantVessel.AddValue("maxDistance", DistantVessel.maxDistance);
            distantVessel.AddValue("renderMode", (int)DistantVessel.renderMode);
            distantVessel.AddValue("ignoreDebris", DistantVessel.ignoreDebris);

            ConfigNode skyboxBrightness = settings.AddNode("SkyboxBrightness");
            skyboxBrightness.AddValue("changeSkybox", SkyboxBrightness.changeSkybox);
            skyboxBrightness.AddValue("maxBrightness", SkyboxBrightness.maxBrightness);

            Commit();
			if (!IO.Directory.Exists(Constants.CONFIG_DIRECTORY)) IO.Directory.CreateDirectory(Constants.CONFIG_DIRECTORY);
            settings.Save(Constants.CONFIG_PATHNAME);
        }

        internal static void Commit()
        {
            #if !DEBUG
            Log.level = (debugMode ? KSPe.Util.Log.Level.DETAIL : KSPe.Util.Log.Level.INFO);
            #else
            Log.level = KSPe.Util.Log.Level.TRACE;
            #endif
            if (null != VesselDraw.Instance) VesselDraw.Instance.SetActiveTo(DistantVessel.renderVessels);
            if (null != FlareDraw.Instance) FlareDraw.Instance.SetActiveTo(DistantFlare.flaresEnabled);
            if (null != DarkenSky.Instance) DarkenSky.Instance.SetActiveTo(SkyboxBrightness.changeSkybox);
        }
    }
}
