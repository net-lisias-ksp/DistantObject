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
using KSP.UI.Screens;

namespace DistantObject
{
	[KSPAddon(KSPAddon.Startup.MainMenu, false)]
	internal class SettingsGuiOnMainMenu:MonoBehaviour
	{
		private SettingsGui settingsGui;
		private void Awake()
		{
			Log.dbg("SettingsGuiOnMainMenu.Awake()");
			this.settingsGui = new SettingsGui();
			this.settingsGui.Awake();
		}

		private void Start()
		{
			Settings.Instance.Load();
		}

		private void OnGUI()
		{
			this.settingsGui.drawGUI();
		}

		private void OnDestroy()
		{
			this.settingsGui.OnDestroy();
			this.settingsGui = null;
		}
	}

	[KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
	internal class SettingsGuiOnGameScenes:MonoBehaviour, KSPe.IO.SaveGameMonitor.SaveGameLoadedListener
	{
		private SettingsGui settingsGui;
		private void Awake()
		{
			Log.dbg("SettingsGuiOnGameScenes.Awake()");
			this.settingsGui = new SettingsGui();
			this.settingsGui.Awake();
		}

		private void Start()
		{
			Settings.Instance.Load();
			if (!KSPe.IO.SaveGameMonitor.Instance.IsValid)
				KSPe.IO.SaveGameMonitor.Instance.AddSingleShot(this);
		}

		private void OnGUI()
		{
			this.settingsGui.drawGUI();
		}

		private void OnDestroy()
		{
			this.settingsGui.OnDestroy();
			this.settingsGui = null;
		}

		void KSPe.IO.SaveGameMonitor.SaveGameLoadedListener.OnSaveGameLoaded(string name)
		{
			Log.dbg("SaveGame {0} is ready!", name);
			Settings.Instance.Load();
		}

		void KSPe.IO.SaveGameMonitor.SaveGameLoadedListener.OnSaveGameClosed() { }
	}

	partial class SettingsGui
    {
		private readonly int windowId = KSPe.UI.UID.Get();
		protected Rect windowPos = new Rect(Screen.width / 4, Screen.height / 4, 10f, 10f);
		protected Vector2 scrollViewPosition = new Vector2();

        private static bool activated = false;
        private bool isActivated = false;

		private Settings buffer = new Settings();

        private static ApplicationLauncherButton appLauncherButton = null;

		~SettingsGui()
		{
			KSPe.UI.UID.Release(this.windowId);
		}

		private void ApplySettings()
		{
			Settings.Instance.Apply(buffer);
			Settings.Instance.Commit();
			Settings.Instance.Save();
		}

		private void ReadSettings()
		{
			Settings.Instance.Load();

			// Create local copies of the values, so we're not editing the
			// config file until the user presses "Apply"

			{
				Settings.FlyOverClass b = buffer.FlyOver;
				b.textSize = Settings.Instance.FlyOver.textSize;
				b.fontName = Settings.Instance.FlyOver.fontName;
			}
			{
				Settings.DistantFlareClass b = buffer.DistantFlare;
				b.flaresEnabled = Settings.Instance.DistantFlare.flaresEnabled;
				b.flareSaturation = Settings.Instance.DistantFlare.flareSaturation;
				b.flareSize = Settings.Instance.DistantFlare.flareSize;
				b.flareBrightness = Settings.Instance.DistantFlare.flareBrightness;
				b.ignoreDebrisFlare = Settings.Instance.DistantFlare.ignoreDebrisFlare;
				b.debrisBrightness = Settings.Instance.DistantFlare.debrisBrightness;
				b.showNames = Settings.Instance.DistantFlare.showNames;
			}
			{
				Settings.DistantVesselClass b = buffer.DistantVessel;
				b.renderVessels = Settings.Instance.DistantVessel.renderVessels;
				b.maxDistance = Settings.Instance.DistantVessel.maxDistance;
				b.renderMode = Settings.Instance.DistantVessel.renderMode;
				b.ignoreDebris = Settings.Instance.DistantVessel.ignoreDebris;
			}
			{
				Settings.SkyboxBrightnessClass b = buffer.SkyboxBrightness;
				b.changeSkybox = Settings.Instance.SkyboxBrightness.changeSkybox;
				b.maxBrightness = Settings.Instance.SkyboxBrightness.maxBrightness;
				b.minimumSignificantBodySize = Settings.Instance.SkyboxBrightness.minimumSignificantBodySize;
				b.minimumTargetRelativeAngle = Settings.Instance.SkyboxBrightness.minimumTargetRelativeAngle;
				b.referenceBodySize = Settings.Instance.SkyboxBrightness.referenceBodySize;
			}
			buffer.debugMode = Settings.Instance.debugMode;
			buffer.useToolbar = Settings.Instance.useToolbar;
			buffer.useAppLauncher = Settings.Instance.useAppLauncher || !ToolbarManager.ToolbarAvailable;
			buffer.onlyInSpaceCenter = Settings.Instance.onlyInSpaceCenter;
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
					ApplicationLauncher.AppScenes.MAINMENU |
						(buffer.onlyInSpaceCenter ? ApplicationLauncher.AppScenes.SPACECENTER : (ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPACECENTER)),
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
                Log.trace("creating new appLauncher instance - {0}", this.GetHashCode());
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

        internal void Awake()
        {
            Log.trace("SettingsGui awake - {0}", this.GetHashCode());

            //Load settings
            ReadSettings();

            GameEvents.onGUIApplicationLauncherReady.Add(AddAppLauncherButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(RemoveAppLauncherButton);

            //if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.FLIGHT)
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

			GUILayoutOption guiwidth200 = GUILayout.Width(200);
			GUILayoutOption guiwidth220 = GUILayout.Width(220);

            GUILayout.BeginVertical();
			this.scrollViewPosition = GUILayout.BeginScrollView(this.scrollViewPosition, false, true);

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
			GUIStyle style = new GUIStyle(GUI.skin.GetStyle("label"));
			style.alignment = TextAnchor.MiddleRight;
			GUILayout.Label(Globals.DistantObjectVersion, style);
            GUILayout.EndHorizontal();

			//--- Fly Over Test --------------------------------------------
			{
				Settings.FlyOverClass b = buffer.FlyOver;
				GUILayout.BeginVertical("Fly Over Text", new GUIStyle(GUI.skin.window));

				GUILayout.Label("Font Size/Name");

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.textSize = (int) GUILayout.HorizontalSlider(b.textSize, 8f, 64f, guiwidth200);
				GUILayout.Label(string.Format("{0:0}", b.textSize));
				GUILayout.EndHorizontal();

				{
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					Font[] fonts = this.buffer.FlyOver.fonts.ToArray();
					int index = this.fontIndexFindByName(b.fontName, fonts);
					index = (int) GUILayout.HorizontalSlider(index, 0, fonts.Length-1, guiwidth200);
					b.fontName = fonts[index].name;
					GUILayout.Label(b.fontName);
					GUILayout.EndHorizontal();
				}

				GUILayout.EndVertical();
			}

			GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
			GUILayout.Label("");
			GUILayout.EndHorizontal();

			//--- Flare Rendering --------------------------------------------
			{
				Settings.DistantFlareClass b = buffer.DistantFlare;
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
					b.flareSaturation = GUILayout.HorizontalSlider(b.flareSaturation, 0f, 1f, guiwidth220);
					GUILayout.Label(string.Format("{0:0}", 100 * b.flareSaturation) + "%");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label("Flare Size");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.flareSize = GUILayout.HorizontalSlider(b.flareSize, 0.5f, 1.5f, guiwidth220);
					GUILayout.Label(string.Format("{0:0}", 100 * b.flareSize) + "%");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label("Flare Brightness");
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					b.flareBrightness = GUILayout.HorizontalSlider(b.flareBrightness, 0.0f, 1.0f, guiwidth220);
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
						b.debrisBrightness = GUILayout.HorizontalSlider(b.debrisBrightness, 0f, 1f, guiwidth220);
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
				Settings.DistantVesselClass b = buffer.DistantVessel;
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
					b.maxDistance = GUILayout.HorizontalSlider(b.maxDistance, 2500f, Settings.Instance.Defaults.DistantVessel.maxDistance, guiwidth200);
					GUILayout.Label(string.Format("{0:0}", b.maxDistance) + "m");
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
					GUILayout.Label(RENDER_MODE_LABEL[(int)b.renderMode]);
					if (GUILayout.Button("Change"))
					{
						b.renderMode = (Settings.ERenderMode)((int)(++b.renderMode) % (int)Settings.ERenderMode.SIZE);
					}
					GUILayout.EndHorizontal();

					if (b.renderMode > Settings.ERenderMode.RenderTargetOnly)
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
				Settings.SkyboxBrightnessClass b = buffer.SkyboxBrightness;
				GUILayout.BeginVertical("Skybox Dimming", new GUIStyle(GUI.skin.window));
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

				b.changeSkybox = GUILayout.Toggle(b.changeSkybox, "Dynamic Sky Dimming");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				GUILayout.Label("Maximum Sky Brightness");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.maxBrightness = GUILayout.HorizontalSlider((float)b.maxBrightness, 0f, 1f, guiwidth220);
				GUILayout.Label(string.Format("{0:0}%", 100 * b.maxBrightness));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				GUILayout.Label("Reference Body Size");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.referenceBodySize = GUILayout.HorizontalSlider((float)b.referenceBodySize, 1f, 100f, guiwidth220);
				GUILayout.Label(string.Format("{0:0.0}", b.referenceBodySize));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				GUILayout.Label("Minimum Significant Body Size");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.minimumSignificantBodySize = GUILayout.HorizontalSlider((float)b.minimumSignificantBodySize, 1f, 180f, guiwidth220);
				GUILayout.Label(string.Format("{0:0.0}", b.minimumSignificantBodySize));
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				GUILayout.Label("Minimum Target Relative Angle");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
				b.minimumTargetRelativeAngle = GUILayout.HorizontalSlider((float)b.minimumTargetRelativeAngle, 1f, 180f, guiwidth220);
				GUILayout.Label(string.Format("{0:0.0}", b.minimumTargetRelativeAngle));
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

			GUILayout.EndScrollView();
			GUILayout.EndVertical();
            GUI.DragWindow();
        }

		private int fontIndexFindByName(string fontName, Font[] fonts)
		{
			int r = 0;
			for (; r < fonts.Length; r++)
				if (fonts[r].name.Equals(fontName)) return r;
			return 0;
		}

		internal void drawGUI()
		{
			if (activated)
			{
				if (!isActivated)
				{
					ReadSettings();
				}

				windowPos = GUILayout.Window(this.windowId, windowPos, mainGUI, Globals.DistantObject + " Settings", GUILayout.Width(320), GUILayout.Height(600));
			}
			isActivated = activated;
		}

		private void Reset() => this.buffer = new Settings();

        public static void Toggle()
        {
            activated = !activated;
        }
    }
}
