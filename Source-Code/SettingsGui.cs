using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DistantObject
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    partial class SettingsGui : MonoBehaviour
    {
        protected Rect windowPos = new Rect(Screen.width / 4, Screen.height / 4, 10f, 10f);

        public static bool toolbarInstalled = false;
        public static bool activated = false;

        private bool flaresEnabled = false;
        private float flareSaturation = 0.65f;
        private float flareSize = 1.0f;
        private float flareBrightness = 1.0f;
        private bool ignoreDebrisFlare = false;
        private float debrisBrightness = 0.15f;
        private bool showNames = false;
        private bool renderVessels = false;
        private float maxDistance = 750000f;
        private int renderMode = 1;
        private bool ignoreDebris = false;
        private bool changeSkybox = true;
        private float maxBrightness = 0.25f;
        private bool debugMode = false;

        ConfigNode settings;

        public void Awake()
        {
            print("Distant Object Enhancement v1.3 -- SettingsGUI initialized");

            foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
            {
                if (assembly.name == "Toolbar")
                    toolbarInstalled = true;
            }

            if (toolbarInstalled)
                toolbarButton();

            RenderingManager.AddToPostDrawQueue(3, new Callback(drawGUI));

            //Load settings
            settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/DistantObject/Settings.cfg");
            foreach (ConfigNode node in settings.GetNodes("DistantFlare"))
            {
                flaresEnabled = bool.Parse(node.GetValue("flaresEnabled"));
                flareSaturation = float.Parse(node.GetValue("flareSaturation"));
                flareSize = float.Parse(node.GetValue("flareSize"));
                flareBrightness = float.Parse(node.GetValue("flareBrightness"));
                ignoreDebrisFlare = bool.Parse(node.GetValue("ignoreDebrisFlare"));
                debrisBrightness = float.Parse(node.GetValue("debrisBrightness"));
                debugMode = bool.Parse(node.GetValue("debugMode"));
                showNames = bool.Parse(node.GetValue("showNames"));
            }

            foreach (ConfigNode node in settings.GetNodes("DistantVessel"))
            {
                renderVessels = bool.Parse(node.GetValue("renderVessels"));
                maxDistance = float.Parse(node.GetValue("maxDistance"));
                renderMode = int.Parse(node.GetValue("renderMode"));
                ignoreDebris = bool.Parse(node.GetValue("ignoreDebris"));
            }

            foreach (ConfigNode node in settings.GetNodes("SkyboxBrightness"))
            {
                changeSkybox = bool.Parse(node.GetValue("changeSkybox"));
                maxBrightness = float.Parse(node.GetValue("maxBrightness"));
            }
        }

        private void mainGUI(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("version 1.3");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("_____________________________________");
            GUILayout.EndHorizontal();

            //--Flare Rendering
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            if (GUILayout.Button(GetStatus(!flaresEnabled) + " Flares"))
            {
                flaresEnabled = !flaresEnabled;
                settings.GetNode("DistantFlare").SetValue("flaresEnabled", "" + flaresEnabled);
            }
            GUILayout.EndHorizontal();

            if (flaresEnabled)
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Flare Saturation");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                flareSaturation = GUILayout.HorizontalSlider(flareSaturation, 0f, 1f, GUILayout.Width(240));
                settings.GetNode("DistantFlare").SetValue("flareSaturation", "" + flareSaturation);
                GUILayout.Label(string.Format("{0:0}", 100 * flareSaturation) + "%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Flare Size");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                flareSize = GUILayout.HorizontalSlider(flareSize, 0.5f, 1.5f, GUILayout.Width(240));
                settings.GetNode("DistantFlare").SetValue("flareSize", "" + flareSize);
                GUILayout.Label(string.Format("{0:0}", 100 * flareSize) + "%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Flare Brightness");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                flareBrightness = GUILayout.HorizontalSlider(flareBrightness, 0.0f, 1.0f, GUILayout.Width(240));
                settings.GetNode("DistantFlare").SetValue("flareBrightness", "" + flareBrightness);
                GUILayout.Label(string.Format("{0:0}", 100 * flareBrightness) + "%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                if (GUILayout.Button(GetStatus(!ignoreDebrisFlare) + " Debris Flares"))
                {
                    ignoreDebrisFlare = !ignoreDebrisFlare;
                    settings.GetNode("DistantFlare").SetValue("ignoreDebrisFlare", "" + ignoreDebrisFlare);
                }
                GUILayout.EndHorizontal();

                if (!ignoreDebrisFlare)
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    GUILayout.Label("Debris Brightness");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    debrisBrightness = GUILayout.HorizontalSlider(debrisBrightness, 0f, 1f, GUILayout.Width(240));
                    settings.GetNode("DistantFlare").SetValue("debrisBrightness", "" + debrisBrightness);
                    GUILayout.Label(string.Format("{0:0}", 100 * debrisBrightness) + "%");
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button(GetStatus(!showNames) + " showing body names on mouseover"))
                {
                    showNames = !showNames;
                    settings.GetNode("DistantFlare").SetValue("showNames", "" + showNames);
                }
            }

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("_____________________________________");
            GUILayout.EndHorizontal();
            
            //--Vessel Rendering
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            if (GUILayout.Button(GetStatus(!renderVessels) + " Distant Vessel Rendering"))
            {
                renderVessels = !renderVessels;
                settings.GetNode("DistantVessel").SetValue("renderVessels", "" + renderVessels);
            }
            GUILayout.EndHorizontal();

            if (renderVessels)
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Max Distance to Render");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                maxDistance = GUILayout.HorizontalSlider(maxDistance, 2500f, 750000f, GUILayout.Width(200));
                settings.GetNode("DistantVessel").SetValue("maxDistance", "" + maxDistance);
                GUILayout.Label(string.Format("{0:0}", maxDistance) + "m");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                if (GUILayout.Button(renderMode == 0 ? "Render All Unloaded Vessels" : "Render Targeted Vessel Only"))
                {
                    if (renderMode == 0)
                        renderMode = 1;
                    else
                        renderMode = 0;
                    settings.GetNode("DistantVessel").SetValue("renderMode", "" + renderMode);
                }
                GUILayout.EndHorizontal();

                if (renderMode == 1)
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    if (GUILayout.Button(GetStatus(ignoreDebris) + " Debris Rendering"))
                    {
                        ignoreDebris = !ignoreDebris;
                        settings.GetNode("DistantVessel").SetValue("ignoreDebris", "" + ignoreDebris);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("_____________________________________");
            GUILayout.EndHorizontal();

            //--Skybox Brightness
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            if (GUILayout.Button(GetStatus(!changeSkybox) + " Dynamic Sky Dimming"))
            {
                changeSkybox = !changeSkybox;
                settings.GetNode("SkyboxBrightness").SetValue("changeSkybox", "" + changeSkybox);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("Maximum Sky Brightness");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            maxBrightness = GUILayout.HorizontalSlider(maxBrightness, 0f, 1f, GUILayout.Width(240));
            settings.GetNode("SkyboxBrightness").SetValue("maxBrightness", "" + maxBrightness);
            GUILayout.Label(string.Format("{0:0}", 100 * maxBrightness) + "%");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("_____________________________________");
            GUILayout.EndHorizontal();

            //--Misc.
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            if (GUILayout.Button(GetStatus(!debugMode) + " Debug Mode"))
            {
                debugMode = !debugMode;
                settings.GetNode("DistantFlare").SetValue("debugMode", "" + debugMode);
                settings.GetNode("DistantVessel").SetValue("debugMode", "" + debugMode);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
            if (GUILayout.Button("Reset To Default"))
            {
                Reset();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
            if (GUILayout.Button("Apply", GUILayout.Height(50)))
            {
                settings.Save(KSPUtil.ApplicationRootPath + "GameData/DistantObject/Settings.cfg");
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void drawGUI()
        {
            if (activated)
            {
                windowPos = GUILayout.Window(-5234628, windowPos, mainGUI, "Distant Object Enhancement Settings", GUILayout.Width(300), GUILayout.Height(200));
            }
        }

        private void Reset()
        {
            flaresEnabled = true;
            flareSaturation = 0.65f;
            flareSize = 1.0f;
            ignoreDebrisFlare = false;
            debrisBrightness = 0.15f;
            debugMode = false;
            renderVessels = true;
            maxDistance = 750000f;
            renderMode = 1;
            ignoreDebris = false;
            changeSkybox = true;
            maxBrightness = 0.25f;

            settings.GetNode("DistantFlare").SetValue("flaresEnabled", "" + flaresEnabled);
            settings.GetNode("DistantFlare").SetValue("flareSaturation", "" + flareSaturation);
            settings.GetNode("DistantFlare").SetValue("flareSize", "" + flareSize);
            settings.GetNode("DistantFlare").SetValue("ignoreDebrisFlare", "" + ignoreDebrisFlare);
            settings.GetNode("DistantFlare").SetValue("debrisBrightness", "" + debrisBrightness);
            settings.GetNode("DistantFlare").SetValue("debugMode", "" + debugMode);
            settings.GetNode("DistantVessel").SetValue("renderVessels", "" + renderVessels);
            settings.GetNode("DistantVessel").SetValue("maxDistance", "" + maxDistance);
            settings.GetNode("DistantVessel").SetValue("renderMode", "" + renderMode);
            settings.GetNode("DistantVessel").SetValue("ignoreDebris", "" + ignoreDebris);
            settings.GetNode("DistantVessel").SetValue("debugMode", "" + debugMode);
            settings.GetNode("SkyboxBrightness").SetValue("changeSkybox", "" + changeSkybox);
            settings.GetNode("SkyboxBrightness").SetValue("maxBrightness", "" + maxBrightness);

            settings.Save(KSPUtil.ApplicationRootPath + "GameData/DistantObject/Settings.cfg");
        }

        public static void Toggle()
        {
            activated = !activated;
        }

        private string GetStatus(bool value)
        {
            if (value)
                return "Enable";
            else
                return "Disable";
        }
    }
}
