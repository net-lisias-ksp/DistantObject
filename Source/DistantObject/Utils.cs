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
        static private string _DistantObject = null;

        static public string DistantObject
        {
            get
            {
                if (_DistantObject == null)
                {
                    Version version = Assembly.GetExecutingAssembly().GetName().Version;

                    _DistantObject = "Distant Object Enhancement v" + version.Major + "." + version.Minor + "." + version.Build;
                }

                return _DistantObject;
            }
        }
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

        public struct DistantVessel
        {
            static public bool renderVessels = false;
            static public float maxDistance = 750000.0f;
            static public int renderMode = 1;
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
        static private string configFileName = "GameData/DistantObject/PluginData/Settings.cfg";

        static public void LoadConfig()
        {
            if (hasLoaded)
            {
                return;
            }

            ConfigNode settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + configFileName);

            if (settings != null)
            {
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
                    if (distantFlare.HasValue("debugMode"))
                    {
                        debugMode = bool.Parse(distantFlare.GetValue("debugMode"));
                    }
                    if (distantFlare.HasValue("useToolbar"))
                    {
                        useToolbar = bool.Parse(distantFlare.GetValue("useToolbar"));
                    }
                    if (distantFlare.HasValue("useAppLauncher"))
                    {
                        useAppLauncher = bool.Parse(distantFlare.GetValue("useAppLauncher"));
                    }
                    if (distantFlare.HasValue("onlyInSpaceCenter"))
                    {
                        onlyInSpaceCenter = bool.Parse(distantFlare.GetValue("onlyInSpaceCenter"));
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
                        DistantVessel.renderMode = int.Parse(distantVessel.GetValue("renderMode"));
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
        }

        static public void SaveConfig()
        {
            ConfigNode settings = new ConfigNode();

            ConfigNode distantFlare = settings.AddNode("DistantFlare");
            distantFlare.AddValue("flaresEnabled", DistantFlare.flaresEnabled);
            distantFlare.AddValue("flareSaturation", DistantFlare.flareSaturation);
            distantFlare.AddValue("flareSize", DistantFlare.flareSize);
            distantFlare.AddValue("flareBrightness", DistantFlare.flareBrightness);
            distantFlare.AddValue("ignoreDebrisFlare", DistantFlare.ignoreDebrisFlare);
            distantFlare.AddValue("debrisBrightness", DistantFlare.debrisBrightness);
            distantFlare.AddValue("situations", DistantFlare.situations);
            distantFlare.AddValue("showNames", DistantFlare.showNames);
            distantFlare.AddValue("debugMode", debugMode);
            distantFlare.AddValue("useToolbar", useToolbar);
            distantFlare.AddValue("useAppLauncher", useAppLauncher);
            distantFlare.AddValue("onlyInSpaceCenter", onlyInSpaceCenter);

            ConfigNode distantVessel = settings.AddNode("DistantVessel");
            distantVessel.AddValue("renderVessels", DistantVessel.renderVessels);
            distantVessel.AddValue("maxDistance", DistantVessel.maxDistance);
            distantVessel.AddValue("renderMode", DistantVessel.renderMode);
            distantVessel.AddValue("ignoreDebris", DistantVessel.ignoreDebris);

            ConfigNode skyboxBrightness = settings.AddNode("SkyboxBrightness");
            skyboxBrightness.AddValue("changeSkybox", SkyboxBrightness.changeSkybox);
            skyboxBrightness.AddValue("maxBrightness", SkyboxBrightness.maxBrightness);

            settings.Save(KSPUtil.ApplicationRootPath + configFileName);
        }
    }
}
