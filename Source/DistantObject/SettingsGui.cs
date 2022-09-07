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

        private DistantObjectSettings buffer = new DistantObjectSettings();

        private static ApplicationLauncherButton appLauncherButton = null;

        private void ApplySettings()
        {
			// Apply our local values to the settings file object, and then
			// save it.
			{
				DistantObjectSettings.DistantFlareClass b = buffer.DistantFlare;
				DistantObjectSettings.Instance.DistantFlare.flaresEnabled = b.flaresEnabled;
				DistantObjectSettings.Instance.DistantFlare.flareSaturation = b.flareSaturation;
				DistantObjectSettings.Instance.DistantFlare.flareSize = b.flareSize;
				DistantObjectSettings.Instance.DistantFlare.flareBrightness = b.flareBrightness;
				DistantObjectSettings.Instance.DistantFlare.ignoreDebrisFlare = b.ignoreDebrisFlare;
				DistantObjectSettings.Instance.DistantFlare.debrisBrightness = b.debrisBrightness;
				DistantObjectSettings.Instance.DistantFlare.showNames = b.showNames;
			}
			{
				DistantObjectSettings.DistantVesselClass b = buffer.DistantVessel;
				DistantObjectSettings.Instance.DistantVessel.renderVessels = b.renderVessels;
				DistantObjectSettings.Instance.DistantVessel.maxDistance = b.maxDistance;
				DistantObjectSettings.Instance.DistantVessel.renderMode = b.renderMode;
				DistantObjectSettings.Instance.DistantVessel.ignoreDebris = b.ignoreDebris;
			}
			{
				DistantObjectSettings.SkyboxBrightnessClass b = buffer.SkyboxBrightness;
				DistantObjectSettings.Instance.SkyboxBrightness.changeSkybox = b.changeSkybox;
				DistantObjectSettings.Instance.SkyboxBrightness.maxBrightness = b.maxBrightness;
			}
			DistantObjectSettings.Instance.debugMode = buffer.debugMode;
            DistantObjectSettings.Instance.useToolbar = buffer.useToolbar;
            DistantObjectSettings.Instance.useAppLauncher = buffer.useAppLauncher;
            DistantObjectSettings.Instance.onlyInSpaceCenter = buffer.onlyInSpaceCenter;

            DistantObjectSettings.Instance.SaveConfig();
        }

		private void ReadSettings()
		{
			DistantObjectSettings.Instance.LoadConfig();

			// Create local copies of the values, so we're not editing the
			// config file until the user presses "Apply"
			{
				DistantObjectSettings.DistantFlareClass b = buffer.DistantFlare;
				b.flaresEnabled = DistantObjectSettings.Instance.DistantFlare.flaresEnabled;
				b.flareSaturation = DistantObjectSettings.Instance.DistantFlare.flareSaturation;
				b.flareSize = DistantObjectSettings.Instance.DistantFlare.flareSize;
				b.flareBrightness = DistantObjectSettings.Instance.DistantFlare.flareBrightness;
				b.ignoreDebrisFlare = DistantObjectSettings.Instance.DistantFlare.ignoreDebrisFlare;
				b.debrisBrightness = DistantObjectSettings.Instance.DistantFlare.debrisBrightness;
				b.showNames = DistantObjectSettings.Instance.DistantFlare.showNames;
			}
			{
				DistantObjectSettings.DistantVesselClass b = buffer.DistantVessel;
				b.renderVessels = DistantObjectSettings.Instance.DistantVessel.renderVessels;
				b.maxDistance = DistantObjectSettings.Instance.DistantVessel.maxDistance;
				b.renderMode = DistantObjectSettings.Instance.DistantVessel.renderMode;
				b.ignoreDebris = DistantObjectSettings.Instance.DistantVessel.ignoreDebris;
			}
			{
				DistantObjectSettings.SkyboxBrightnessClass b = buffer.SkyboxBrightness;
				b.changeSkybox = DistantObjectSettings.Instance.SkyboxBrightness.changeSkybox;
				b.maxBrightness = DistantObjectSettings.Instance.SkyboxBrightness.maxBrightness;
			}
			buffer.debugMode = DistantObjectSettings.Instance.debugMode;
			buffer.useToolbar = DistantObjectSettings.Instance.useToolbar;
			buffer.useAppLauncher = DistantObjectSettings.Instance.useAppLauncher || !ToolbarManager.ToolbarAvailable;
			buffer.onlyInSpaceCenter = DistantObjectSettings.Instance.onlyInSpaceCenter;
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
                    (buffer.onlyInSpaceCenter) ? ApplicationLauncher.AppScenes.SPACECENTER : (ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPACECENTER),
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
            if (buffer.useAppLauncher && appLauncherButton == null)
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
            Log.trace("SettingsGui awake - " + this.GetInstanceID());

            //Load settings
            ReadSettings();

            GameEvents.onGUIApplicationLauncherReady.Add(AddAppLauncherButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveAppLauncherButton);

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                if (buffer.useToolbar && ToolbarManager.ToolbarAvailable)
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
			{
				DistantObjectSettings.DistantFlareClass b = buffer.DistantFlare;
				GUILayout.BeginVertical("Flare Rendering", new GUIStyle(GUI.skin.window));
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.flaresEnabled = GUILayout.Toggle(b.flaresEnabled, "Enable Flares");
				GUILayout.EndHorizontal();

				if (b.flaresEnabled)
				{
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.showNames = GUILayout.Toggle(b.showNames, "Show names on mouseover");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label("Flare Saturation");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.flareSaturation = GUILayout.HorizontalSlider(b.flareSaturation, 0f, 1f, GUILayout.Width(220));
					GUILayout.Label(string.Format("{0:0}", 100 * b.flareSaturation) + "%");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label("Flare Size");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.flareSize = GUILayout.HorizontalSlider(b.flareSize, 0.5f, 1.5f, GUILayout.Width(220));
					GUILayout.Label(string.Format("{0:0}", 100 * b.flareSize) + "%");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label("Flare Brightness");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.flareBrightness = GUILayout.HorizontalSlider(b.flareBrightness, 0.0f, 1.0f, GUILayout.Width(220));
					GUILayout.Label(string.Format("{0:0}", 100 * b.flareBrightness) + "%");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.ignoreDebrisFlare = !GUILayout.Toggle(!b.ignoreDebrisFlare, "Show Debris Flares");
					GUILayout.EndHorizontal();

					if (!b.ignoreDebrisFlare)
					{
						GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
						GUILayout.Label("Debris Brightness");
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
						b.debrisBrightness = GUILayout.HorizontalSlider(b.debrisBrightness, 0f, 1f, GUILayout.Width(220));
						GUILayout.Label(string.Format("{0:0}", 100 * b.debrisBrightness) + "%");
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();
			}

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            //--- Vessel Rendering -------------------------------------------
			{
				DistantObjectSettings.DistantVesselClass b = buffer.DistantVessel;
				GUILayout.BeginVertical("Distant Vessel", new GUIStyle(GUI.skin.window));

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.renderVessels = GUILayout.Toggle(b.renderVessels, "Distant Vessel Rendering");
				GUILayout.EndHorizontal();

				if (b.renderVessels)
				{
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label("Max Distance to Render");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.maxDistance = GUILayout.HorizontalSlider(b.maxDistance, 2500f, 750000f, GUILayout.Width(200));
					GUILayout.Label(string.Format("{0:0}", b.maxDistance) + "m");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label(RENDER_MODE_LABEL[(int)b.renderMode]);
					if (GUILayout.Button("Change"))
					{
						b.renderMode = (DistantObjectSettings.ERenderMode)((int)(++b.renderMode) % (int)DistantObjectSettings.ERenderMode.SIZE);
					}
					GUILayout.EndHorizontal();

					if (b.renderMode > DistantObjectSettings.ERenderMode.RenderTargetOnly)
					{
						GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
						b.ignoreDebris = GUILayout.Toggle(b.ignoreDebris, "Ignore Debris");
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();
			}

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            //--- Skybox Brightness ------------------------------------------
			{
				DistantObjectSettings.SkyboxBrightnessClass b = buffer.SkyboxBrightness;
				GUILayout.BeginVertical("Skybox Dimming", new GUIStyle(GUI.skin.window));
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

				b.changeSkybox = GUILayout.Toggle(b.changeSkybox, "Dynamic Sky Dimming");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				GUILayout.Label("Maximum Sky Brightness");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.maxBrightness = GUILayout.HorizontalSlider(b.maxBrightness, 0f, 1f, GUILayout.Width(220));
				GUILayout.Label(string.Format("{0:0}%", 100 * b.maxBrightness));
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
			}

			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            //--- Misc. ------------------------------------------------------
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            buffer.debugMode = GUILayout.Toggle(buffer.debugMode, "Debug Mode");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            buffer.useAppLauncher = GUILayout.Toggle(buffer.useAppLauncher, "Use KSP AppLauncher (may require restart)");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            buffer.onlyInSpaceCenter = GUILayout.Toggle(buffer.onlyInSpaceCenter, "Show AppLauncher only in Space Center");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            buffer.useToolbar = GUILayout.Toggle(buffer.useToolbar, "Use Blizzy's Toolbar (may require restart)");
            GUILayout.EndHorizontal();
            if (buffer.useAppLauncher == false && buffer.useToolbar == false)
            {
                buffer.useAppLauncher = true;
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

		private void Reset() => this.buffer = new DistantObjectSettings();

        public static void Toggle()
        {
            activated = !activated;
        }
    }
}
