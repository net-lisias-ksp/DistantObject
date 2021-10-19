/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
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
using System.Reflection;
using UnityEngine;

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
        static private readonly string CONFIG_PATHNAME = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(typeof(DistantObjectSettings).Assembly.Location)
                , "PluginData/Settings.cfg"
            );

        static public void LoadConfig()
        {
            if (hasLoaded) return;

            ConfigNode settings = ConfigNode.Load(CONFIG_PATHNAME);

            if (settings != null)
            {
                if (settings.HasValue("debugMode"))
                {
                    debugMode = bool.Parse(settings.GetValue("debugMode"));
                }
                if (settings.HasValue("useToolbar"))
                {
                    useToolbar = bool.Parse(settings.GetValue("useToolbar"));
                }
                if (settings.HasValue("useAppLauncher"))
                {
                    useAppLauncher = bool.Parse(settings.GetValue("useAppLauncher"));
                }
                if (settings.HasValue("onlyInSpaceCenter"))
                {
                    onlyInSpaceCenter = bool.Parse(settings.GetValue("onlyInSpaceCenter"));
                }

                if (settings.HasNode("DistantFlare"))
                {
                    ConfigNode distantFlare = settings.GetNode("DistantFlare");

                    if (distantFlare.HasValue("flaresEnabled"))
                    {
                        DistantFlare.flaresEnabled = bool.Parse(distantFlare.GetValue("flaresEnabled"));
                    }
                    if (distantFlare.HasValue("flareSaturation"))
                    {
                        DistantFlare.flareSaturation = float.Parse(distantFlare.GetValue("flareSaturation"));
                    }
                    if (distantFlare.HasValue("flareSize"))
                    {
                        DistantFlare.flareSize = float.Parse(distantFlare.GetValue("flareSize"));
                    }
                    if (distantFlare.HasValue("flareBrightness"))
                    {
                        DistantFlare.flareBrightness = float.Parse(distantFlare.GetValue("flareBrightness"));
                    }
                    if (distantFlare.HasValue("ignoreDebrisFlare"))
                    {
                        DistantFlare.ignoreDebrisFlare = bool.Parse(distantFlare.GetValue("ignoreDebrisFlare"));
                    }
                    if (distantFlare.HasValue("debrisBrightness"))
                    {
                        DistantFlare.debrisBrightness = float.Parse(distantFlare.GetValue("debrisBrightness"));
                    }
                    if (distantFlare.HasValue("showNames"))
                    {
                        DistantFlare.showNames = bool.Parse(distantFlare.GetValue("showNames"));
                    }
                }

                if (settings.HasNode("DistantVessel"))
                {
                    ConfigNode distantVessel = settings.GetNode("DistantVessel");

                    if (distantVessel.HasValue("renderVessels"))
                    {
                        DistantVessel.renderVessels = bool.Parse(distantVessel.GetValue("renderVessels"));
                    }
                    if (distantVessel.HasValue("maxDistance"))
                    {
                        DistantVessel.maxDistance = float.Parse(distantVessel.GetValue("maxDistance"));
                    }
                    if (distantVessel.HasValue("renderMode"))
                    {
                        DistantVessel.renderMode = (ERenderMode)int.Parse(distantVessel.GetValue("renderMode"));
                    }
                    if (distantVessel.HasValue("ignoreDebris"))
                    {
                        DistantVessel.ignoreDebris = bool.Parse(distantVessel.GetValue("ignoreDebris"));
                    }
                }

                if (settings.HasNode("SkyboxBrightness"))
                {
                    ConfigNode skyboxBrightness = settings.GetNode("SkyboxBrightness");

                    if (skyboxBrightness.HasValue("changeSkybox"))
                    {
                        SkyboxBrightness.changeSkybox = bool.Parse(skyboxBrightness.GetValue("changeSkybox"));
                    }
                    if (skyboxBrightness.HasValue("maxBrightness"))
                    {
                        SkyboxBrightness.maxBrightness = float.Parse(skyboxBrightness.GetValue("maxBrightness"));
                    }
                }
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
            settings.Save(CONFIG_PATHNAME);
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
            if (null != DarkenSky.Instance) FlareDraw.Instance.SetActiveTo(SkyboxBrightness.changeSkybox);
        }
    }
}
