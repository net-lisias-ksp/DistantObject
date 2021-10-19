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
using UnityEngine;
using KSP.UI.Screens;

namespace DistantObject
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    partial class SettingsGui : MonoBehaviour
    {
        protected Rect windowPos = new Rect(Screen.width / 4, Screen.height / 4, 10f, 10f);

        private static bool activated = false;
        private bool isActivated = false;

        private bool flaresEnabled = false;
        private float flareSaturation = 1.0f;
        private float flareSize = 1.0f;
        private float flareBrightness = 1.0f;
        private bool ignoreDebrisFlare = false;
        private float debrisBrightness = 0.15f;
        private bool showNames = false;
        private bool renderVessels = false;
        private float maxDistance = 750000f;
        private DistantObjectSettings.ERenderMode renderMode = DistantObjectSettings.ERenderMode.RenderTargetOnly;
        private bool ignoreDebris = false;
        private bool changeSkybox = true;
        private float maxBrightness = 0.25f;
        private bool debugMode = false;
        private bool useToolbar = true;
        private bool useAppLauncher = true;
        private bool onlyInSpaceCenter = false;

        private static ApplicationLauncherButton appLauncherButton = null;

        private void ApplySettings()
        {
            // Apply our local values to the settings file object, and then
            // save it.
            DistantObjectSettings.DistantFlare.flaresEnabled = flaresEnabled;
            DistantObjectSettings.DistantFlare.flareSaturation = flareSaturation;
            DistantObjectSettings.DistantFlare.flareSize = flareSize;
            DistantObjectSettings.DistantFlare.flareBrightness = flareBrightness;
            DistantObjectSettings.DistantFlare.ignoreDebrisFlare = ignoreDebrisFlare;
            DistantObjectSettings.DistantFlare.debrisBrightness = debrisBrightness;
            DistantObjectSettings.DistantFlare.showNames = showNames;

            DistantObjectSettings.DistantVessel.renderVessels = renderVessels;
            DistantObjectSettings.DistantVessel.maxDistance = maxDistance;
            DistantObjectSettings.DistantVessel.renderMode = renderMode;
            DistantObjectSettings.DistantVessel.ignoreDebris = ignoreDebris;

            DistantObjectSettings.SkyboxBrightness.changeSkybox = changeSkybox;
            DistantObjectSettings.SkyboxBrightness.maxBrightness = maxBrightness;

            DistantObjectSettings.debugMode = debugMode;
            DistantObjectSettings.useToolbar = useToolbar;
            DistantObjectSettings.useAppLauncher = useAppLauncher;
            DistantObjectSettings.onlyInSpaceCenter = onlyInSpaceCenter;

            DistantObjectSettings.SaveConfig();
        }

        private void ReadSettings()
        {
            DistantObjectSettings.LoadConfig();

            // Create local copies of the values, so we're not editing the
            // config file until the user presses "Apply"
            flaresEnabled = DistantObjectSettings.DistantFlare.flaresEnabled;
            flareSaturation = DistantObjectSettings.DistantFlare.flareSaturation;
            flareSize = DistantObjectSettings.DistantFlare.flareSize;
            flareBrightness = DistantObjectSettings.DistantFlare.flareBrightness;
            ignoreDebrisFlare = DistantObjectSettings.DistantFlare.ignoreDebrisFlare;
            debrisBrightness = DistantObjectSettings.DistantFlare.debrisBrightness;
            showNames = DistantObjectSettings.DistantFlare.showNames;

            renderVessels = DistantObjectSettings.DistantVessel.renderVessels;
            maxDistance = DistantObjectSettings.DistantVessel.maxDistance;
            renderMode = DistantObjectSettings.DistantVessel.renderMode;
            ignoreDebris = DistantObjectSettings.DistantVessel.ignoreDebris;

            changeSkybox = DistantObjectSettings.SkyboxBrightness.changeSkybox;
            maxBrightness = DistantObjectSettings.SkyboxBrightness.maxBrightness;

            debugMode = DistantObjectSettings.debugMode;
            useToolbar = DistantObjectSettings.useToolbar;
            useAppLauncher = DistantObjectSettings.useAppLauncher || !ToolbarManager.ToolbarAvailable;
            onlyInSpaceCenter = DistantObjectSettings.onlyInSpaceCenter;
        }

        void onAppLauncherTrue()
        {
            if (appLauncherButton == null)
            {
                Log.warn("onAppLauncherTrue called without a button?!?");
                return;
            }

            activated = true;
            ToggleIcon();
        }

        void onAppLauncherFalse()
        {
            if (appLauncherButton == null)
            {
                Log.warn("onAppLauncherFalse called without a button?!?");
                return;
            }

            activated = false;
            ToggleIcon();
        }

        ApplicationLauncherButton InitAppLauncherButton()
        {
            ApplicationLauncherButton button = null;
            Texture2D iconTexture = null;
            Log.trace("InitAppLauncherButton");

            if (GameDatabase.Instance.ExistsTexture("DistantObject/Icons/toolbar_disabled_38"))
            {
                iconTexture = GameDatabase.Instance.GetTexture("DistantObject/Icons/toolbar_disabled_38", false);
            }

            if (iconTexture == null)
            {
                Log.error("Failed to load toolbar_disabled_38");
            }
            else
            {
                button = ApplicationLauncher.Instance.AddModApplication(onAppLauncherTrue, onAppLauncherFalse,
                    null, null, null, null,
                    (onlyInSpaceCenter) ? ApplicationLauncher.AppScenes.SPACECENTER : (ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPACECENTER),
                    iconTexture);

                if (button == null)
                {
                    Log.warn("Unable to create AppLauncher button");
                }
            }

            return button;
        }

        private void AddAppLauncherButton()
        {
            if (useAppLauncher && appLauncherButton == null)
            {
                Log.trace("creating new appLauncher instance - " + this.GetInstanceID());
                appLauncherButton = InitAppLauncherButton();
            }
        }

        private void RemoveAppLauncherButton()
        {
            if (appLauncherButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
                appLauncherButton = null;
            }
        }

        private void Awake()
        {
            //Load settings
            ReadSettings();

            Log.trace("SettingsGui awake - " + this.GetInstanceID());

            GameEvents.onGUIApplicationLauncherReady.Add(AddAppLauncherButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveAppLauncherButton);

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (useToolbar && ToolbarManager.ToolbarAvailable)
                {
                    toolbarButton();
                }
            }
        }

        private readonly string[] RENDER_MODE_LABEL =
        {
            "Render Targeted Vessel Only",
            "Render All Unloaded Vessels",
            "Render All Unloaded Vessels Smoother (memory intensive!)",
        };

        private void mainGUI(int windowID)
        {
            GUIStyle styleWindow = new GUIStyle(GUI.skin.window);
            styleWindow.padding.left = 4;
            styleWindow.padding.top = 4;
            styleWindow.padding.bottom = 4;
            styleWindow.padding.right = 4;

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            //--- Flare Rendering --------------------------------------------
            GUILayout.BeginVertical("Flare Rendering", new GUIStyle(GUI.skin.window));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            flaresEnabled = GUILayout.Toggle(flaresEnabled, "Enable Flares");
            GUILayout.EndHorizontal();

            if (flaresEnabled)
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                showNames = GUILayout.Toggle(showNames, "Show names on mouseover");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Flare Saturation");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                flareSaturation = GUILayout.HorizontalSlider(flareSaturation, 0f, 1f, GUILayout.Width(220));
                GUILayout.Label(string.Format("{0:0}", 100 * flareSaturation) + "%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Flare Size");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                flareSize = GUILayout.HorizontalSlider(flareSize, 0.5f, 1.5f, GUILayout.Width(220));
                GUILayout.Label(string.Format("{0:0}", 100 * flareSize) + "%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Flare Brightness");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                flareBrightness = GUILayout.HorizontalSlider(flareBrightness, 0.0f, 1.0f, GUILayout.Width(220));
                GUILayout.Label(string.Format("{0:0}", 100 * flareBrightness) + "%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                ignoreDebrisFlare = !GUILayout.Toggle(!ignoreDebrisFlare, "Show Debris Flares");
                GUILayout.EndHorizontal();

                if (!ignoreDebrisFlare)
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    GUILayout.Label("Debris Brightness");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    debrisBrightness = GUILayout.HorizontalSlider(debrisBrightness, 0f, 1f, GUILayout.Width(220));
                    GUILayout.Label(string.Format("{0:0}", 100 * debrisBrightness) + "%");
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            //--- Vessel Rendering -------------------------------------------
            GUILayout.BeginVertical("Distant Vessel", new GUIStyle(GUI.skin.window));

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            renderVessels = GUILayout.Toggle(renderVessels, "Distant Vessel Rendering");
            GUILayout.EndHorizontal();

            if (renderVessels)
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label("Max Distance to Render");
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                maxDistance = GUILayout.HorizontalSlider(maxDistance, 2500f, 750000f, GUILayout.Width(200));
                GUILayout.Label(string.Format("{0:0}", maxDistance) + "m");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label(RENDER_MODE_LABEL[(int)renderMode]);
                if (GUILayout.Button("Change"))
                {
                    renderMode = (DistantObjectSettings.ERenderMode)((int)(++renderMode)%(int)DistantObjectSettings.ERenderMode.SIZE);
                }
                GUILayout.EndHorizontal();

                if (renderMode > DistantObjectSettings.ERenderMode.RenderTargetOnly)
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    ignoreDebris = GUILayout.Toggle(ignoreDebris, "Ignore Debris");
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            //--- Skybox Brightness ------------------------------------------
            GUILayout.BeginVertical("Skybox Dimming", new GUIStyle(GUI.skin.window));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

            changeSkybox = GUILayout.Toggle(changeSkybox, "Dynamic Sky Dimming");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("Maximum Sky Brightness");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            maxBrightness = GUILayout.HorizontalSlider(maxBrightness, 0f, 1f, GUILayout.Width(220));
            GUILayout.Label(string.Format("{0:0}", 100 * maxBrightness) + "%");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            //--- Misc. ------------------------------------------------------
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            debugMode = GUILayout.Toggle(debugMode, "Debug Mode");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            useAppLauncher = GUILayout.Toggle(useAppLauncher, "Use KSP AppLauncher (may require restart)");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            onlyInSpaceCenter = GUILayout.Toggle(onlyInSpaceCenter, "Show AppLauncher only in Space Center");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            useToolbar = GUILayout.Toggle(useToolbar, "Use Blizzy's Toolbar (may require restart)");
            GUILayout.EndHorizontal();
            if (useAppLauncher == false && useToolbar == false)
            {
                useAppLauncher = true;
            }

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
            if (GUILayout.Button("Reset To Default"))
            {
                Reset();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
            GUIStyle styleApply = new GUIStyle(GUI.skin.button);
            styleApply.fontSize = styleApply.fontSize + 2;
            if (GUILayout.Button("Apply", GUILayout.Height(50)))
            {
                ApplySettings();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void OnGUI()
        {
            drawGUI();
        }

        private void drawGUI()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (activated)
                {
                    if (!isActivated)
                    {
                        ReadSettings();
                    }
                    windowPos = GUILayout.Window(-5234628, windowPos, mainGUI, Constants.DistantObject + " Settings", GUILayout.Width(300), GUILayout.Height(200));
                }
                isActivated = activated;
            }
        }

        private void Reset()
        {
            flaresEnabled = true;
            flareSaturation = 0.65f;
            flareSize = 1.0f;
            flareBrightness = 1.0f;
            ignoreDebrisFlare = false;
            debrisBrightness = 0.15f;
            showNames = false;

            renderVessels = false;
            maxDistance = 750000f;
            renderMode = DistantObjectSettings.ERenderMode.RenderTargetOnly;
            ignoreDebris = false;

            changeSkybox = true;
            maxBrightness = 0.25f;

            debugMode = false;
            useToolbar = true;
            useAppLauncher = true;
            onlyInSpaceCenter = true;
        }

        public static void Toggle()
        {
            activated = !activated;
        }
    }
}
