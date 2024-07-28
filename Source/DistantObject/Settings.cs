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
using SIO = System.IO;

using KSPe;
using IO = KSPe.IO;

namespace DistantObject
{
	internal class Settings
	{
		private static Settings __INSTANCE = null;
		public static Settings Instance => __INSTANCE??(__INSTANCE = new Settings());

		public enum ERenderMode
		{
			RenderTargetOnly = 0,
			RenderAll = 1,
			RenderAllDontForget = 2,
			SIZE = 3
		}

		public class DefaultSettings
		{
			public class FlyOverClass
			{
				public readonly int textSize = 14;
				public readonly string fontName = "Arial"; // Options on 1.4.3 (must be dynamic): Arial; HEADINGFONT; calibri; calibrib; calibriz; kalibri; calibrii; calibril; calibrili; calibrib; calibrii; dotty
				public FlyOverClass(ConfigNodeWithSteroids node) { }
			}

			public class DistantFlareClass
			{
				public readonly bool flaresEnabled = true;
				public readonly bool ignoreDebrisFlare = false;
				public readonly bool showNames = false;
				public readonly float flareSaturation = 1.0f;
				public readonly float flareSize = 1.0f;
				public readonly float flareBrightness = 1.0f;
				public readonly string situations = "ORBITING,SUB_ORBITAL,ESCAPING,DOCKED,FLYING";
				public readonly float debrisBrightness = 0.15f;

				public DistantFlareClass(ConfigNodeWithSteroids node) { }
			}

			public class DistantVesselClass
			{
				public readonly bool renderVessels = false;
				public readonly float maxDistance;
				public readonly ERenderMode renderMode = ERenderMode.RenderTargetOnly;
				public readonly bool ignoreDebris = false;

				public DistantVesselClass(ConfigNodeWithSteroids node)
				{
					ConfigNodeWithSteroids mynode = ConfigNodeWithSteroids.from(node.GetNode("DistantVessel"));
					this.maxDistance = mynode.GetValue<float>("maxDistance", 750000.0f);
				}
			}

			public class SkyboxBrightnessClass
			{
				public readonly bool changeSkybox = true;
				public readonly double maxBrightness = 0.25f;
				public readonly double referenceBodySize;
				public readonly double minimumSignificantBodySize;
				public readonly double minimumTargetRelativeAngle;

				public SkyboxBrightnessClass(ConfigNodeWithSteroids node)
				{
					ConfigNodeWithSteroids mynode = ConfigNodeWithSteroids.from(node.GetNode("SkyboxBrightness"));
					this.referenceBodySize = mynode.GetValue<double>("referenceBodySize", 60f);
					this.minimumSignificantBodySize = mynode.GetValue<double>("minimumSignificantBodySize", 1.0f);
					this.minimumTargetRelativeAngle = mynode.GetValue<double>("minimumTargetRelativeAngle", 100f);
				}
			}

			public readonly bool debugMode = false;
			public readonly bool useToolbar = false;
			public readonly bool useAppLauncher = true;
			public readonly bool onlyInSpaceCenter = false;

			public readonly FlyOverClass FlyOver;
			public readonly DistantFlareClass DistantFlare;
			public readonly DistantVesselClass DistantVessel;
			public readonly SkyboxBrightnessClass SkyboxBrightness;

			public DefaultSettings(ConfigNodeWithSteroids node)
			{
				this.FlyOver = new FlyOverClass(node);
				this.DistantFlare = new DistantFlareClass(node);
				this.DistantVessel = new DistantVesselClass(node);
				this.SkyboxBrightness = new SkyboxBrightnessClass(node);
			}
		}

		//--- Config file values

		public class FlyOverClass
		{
			public int textSize;
			public string fontName;

			public int ScaledTextSize => (int)(GameSettings.UI_SCALE * this.textSize);

			public FlyOverClass(DefaultSettings defaults)
			{
				this.Reset(defaults.FlyOver);
			}

			public void Reset(DefaultSettings.FlyOverClass defaults)
			{
				this.textSize = defaults.textSize;
				this.fontName = defaults.fontName;
			}

			internal void Apply(FlyOverClass buffer)
			{
				this.textSize = buffer.textSize;
				this.fontName = buffer.fontName;
			}

			internal void Load(ConfigNodeWithSteroids node)
			{
				this.textSize = node.GetValue<int>("textSize", this.textSize);
				this.fontName = node.GetValue<string>("fontName", this.fontName);
			}

			internal void Save(ConfigNode node)
			{
				ConfigNode distantFlare = node.AddNode("FlyOver");
				distantFlare.AddValue("textSize", this.textSize);
				distantFlare.AddValue("fontName", this.fontName);
			}
		}

		public class DistantFlareClass
		{
			public bool flaresEnabled;
			public bool ignoreDebrisFlare;
			public bool showNames;
			public float flareSaturation;
			public float flareSize;
			public float flareBrightness;
			public string situations;
			public float debrisBrightness;

			public DistantFlareClass(DefaultSettings defaults)
			{
				this.Reset(defaults.DistantFlare);
			}

			public void Reset(DefaultSettings.DistantFlareClass defaults)
			{
				this.flaresEnabled = defaults.flaresEnabled;
				this.ignoreDebrisFlare = defaults.ignoreDebrisFlare;
				this.showNames = defaults.showNames;
				this.flareSaturation = defaults.flareSaturation;
				this.flareSize = defaults.flareSize;
				this.flareBrightness = defaults.flareBrightness;
				this.situations = defaults.situations;
				this.debrisBrightness = defaults.debrisBrightness;
			}

			internal void Apply(DistantFlareClass buffer)
			{
				this.flaresEnabled = buffer.flaresEnabled;
				this.ignoreDebrisFlare = buffer.ignoreDebrisFlare;
				this.showNames = buffer.showNames;
				this.flareSaturation = buffer.flareSaturation;
				this.flareSize = buffer.flareSize;
				this.flareBrightness = buffer.flareBrightness;
				this.situations = buffer.situations;
				this.debrisBrightness = buffer.debrisBrightness;
			}

			internal void Load(ConfigNodeWithSteroids node)
			{
				this.flaresEnabled = node.GetValue<bool>("flaresEnabled", this.flaresEnabled);
				this.flareSaturation = node.GetValue<float>("flareSaturation", this.flareSaturation);
				this.flareSize = node.GetValue<float>("flareSize", this.flareSize);
				this.flareBrightness = node.GetValue<float>("flareBrightness", this.flareBrightness);
				this.ignoreDebrisFlare = node.GetValue<bool>("ignoreDebrisFlare", this.ignoreDebrisFlare);
				this.debrisBrightness = node.GetValue<float>("debrisBrightness", this.debrisBrightness);
				this.showNames = node.GetValue<bool>("showNames", this.showNames);
			}

			internal void Save(ConfigNode node)
			{
				ConfigNode distantFlare = node.AddNode("DistantFlare");
				distantFlare.AddValue("flaresEnabled", this.flaresEnabled);
				distantFlare.AddValue("flareSaturation", this.flareSaturation);
				distantFlare.AddValue("flareSize", this.flareSize);
				distantFlare.AddValue("flareBrightness", this.flareBrightness);
				distantFlare.AddValue("ignoreDebrisFlare", this.ignoreDebrisFlare);
				distantFlare.AddValue("debrisBrightness", this.debrisBrightness);
				distantFlare.AddValue("situations", this.situations);
				distantFlare.AddValue("showNames", this.showNames);
			}
		}

		public class DistantVesselClass
		{
			public bool renderVessels;
			public float maxDistance;
			public ERenderMode renderMode;
			public bool ignoreDebris;

			public DistantVesselClass(DefaultSettings defaults)
			{
				this.Reset(defaults.DistantVessel);
			}

			public void Reset(DefaultSettings.DistantVesselClass defaults)
			{
				this.renderVessels = defaults.renderVessels;
				this.maxDistance = defaults.maxDistance;
				this.renderMode = defaults.renderMode;
				this.ignoreDebris = defaults.ignoreDebris;
			}

			internal void Apply(DistantVesselClass buffer)
			{
				this.renderVessels = buffer.renderVessels;
				this.maxDistance = buffer.maxDistance;
				this.renderMode = buffer.renderMode;
				this.ignoreDebris = buffer.ignoreDebris;
			}

			internal void Load(ConfigNodeWithSteroids node)
			{
				this.renderVessels = node.GetValue<bool>("renderVessels", this.renderVessels);
				float maxDistance = node.GetValue<float>("maxDistance", this.maxDistance);
				this.maxDistance = Math.Min(maxDistance, this.maxDistance);	// Clamps the maxDistance case the default was shrunk.
				this.renderMode = (ERenderMode)node.GetValue<int>("renderMode", (int)this.renderMode);
				this.ignoreDebris = node.GetValue<bool>("ignoreDebris", this.ignoreDebris);
			}

			internal void Save(ConfigNode node)
			{
				ConfigNode distantVessel = node.AddNode("DistantVessel");
				distantVessel.AddValue("renderVessels", this.renderVessels);
				distantVessel.AddValue("maxDistance", this.maxDistance);
				distantVessel.AddValue("renderMode", (int)this.renderMode);
				distantVessel.AddValue("ignoreDebris", this.ignoreDebris);
			}

		}

		public class SkyboxBrightnessClass
		{
			public bool changeSkybox = true;
			public double maxBrightness = 0.25f;
			public double referenceBodySize = 60f;
			public double minimumSignificantBodySize = 1.0f;
			public double minimumTargetRelativeAngle = 100f;

			public SkyboxBrightnessClass(DefaultSettings defaults)
			{
				this.Reset(defaults.SkyboxBrightness);
			}

			public void Reset(DefaultSettings.SkyboxBrightnessClass defaults)
			{
				this.changeSkybox = defaults.changeSkybox;
				this.maxBrightness = defaults.maxBrightness;
				this.referenceBodySize = defaults.referenceBodySize;
				this.minimumSignificantBodySize = defaults.minimumSignificantBodySize;
				this.minimumTargetRelativeAngle = defaults.minimumTargetRelativeAngle;
			}

			internal void Apply(SkyboxBrightnessClass buffer)
			{
				this.changeSkybox = buffer.changeSkybox;
				this.maxBrightness = buffer.maxBrightness;
				this.referenceBodySize = buffer.referenceBodySize;
				this.minimumSignificantBodySize = buffer.minimumSignificantBodySize;
				this.minimumTargetRelativeAngle = buffer.minimumTargetRelativeAngle;
			}

			internal void Load(ConfigNodeWithSteroids node)
			{
				this.changeSkybox = node.GetValue<bool>("changeSkybox", this.changeSkybox);
				this.maxBrightness = node.GetValue<double>("maxBrightness", this.maxBrightness);
				this.referenceBodySize = Math.Max(1.0, Math.Min(180,
						node.GetValue<double>("referenceBodySize", this.referenceBodySize)
					));
				this.minimumSignificantBodySize = Math.Max(1.0, Math.Min(180,
						node.GetValue<double>("minimumSignificantBodySize", this.minimumSignificantBodySize)
					));
				this.minimumTargetRelativeAngle = Math.Max(1, Math.Min(180,
						node.GetValue<double>("minimumTargetRelativeAngle", this.minimumTargetRelativeAngle)
					));
			}

			internal void Save(ConfigNode node)
			{
				ConfigNode skyboxBrightness = node.AddNode("SkyboxBrightness");
				skyboxBrightness.AddValue("changeSkybox", this.changeSkybox);
				skyboxBrightness.AddValue("maxBrightness", this.maxBrightness);
				skyboxBrightness.AddValue("referenceBodySize", this.referenceBodySize);
				skyboxBrightness.AddValue("minimumSignificantBodySize", this.minimumSignificantBodySize);
				skyboxBrightness.AddValue("minimumTargetRelativeAngle", this.minimumTargetRelativeAngle);
			}
		}

		internal readonly DefaultSettings Defaults;
		public readonly FlyOverClass FlyOver;
		public readonly DistantFlareClass DistantFlare;
		public readonly DistantVesselClass DistantVessel;
		public readonly SkyboxBrightnessClass SkyboxBrightness;

		public bool debugMode = false;
		public bool useToolbar = false;
		public bool useAppLauncher = true;
		public bool onlyInSpaceCenter = false;

		internal Settings()
		{
			UrlDir.UrlConfig url = GameDatabase.Instance.GetConfigs(Globals.SETTINGS_DEFAULTS)[0];
			ConfigNodeWithSteroids node = ConfigNodeWithSteroids.from(url.config);
			this.Defaults = new DefaultSettings(node);
			this.FlyOver = new FlyOverClass(this.Defaults);
			this.DistantFlare = new DistantFlareClass(this.Defaults);
			this.DistantVessel = new DistantVesselClass(this.Defaults);
			this.SkyboxBrightness = new SkyboxBrightnessClass(this.Defaults);
		}

		public void Reset()
		{
			this.hasLoaded = false;

			this.debugMode = this.Defaults.debugMode;
			this.useToolbar = this.Defaults.useToolbar;
			this.useAppLauncher = this.Defaults.useAppLauncher;
			this.onlyInSpaceCenter = this.Defaults.onlyInSpaceCenter;

			this.FlyOver.Reset(this.Defaults.FlyOver);
			this.DistantFlare.Reset(this.Defaults.DistantFlare);
			this.DistantVessel.Reset(this.Defaults.DistantVessel);
			this.SkyboxBrightness.Reset(this.Defaults.SkyboxBrightness);
		}

		public void Apply(Settings buffer)
		{
			this.debugMode = buffer.debugMode;
			this.useToolbar = buffer.useToolbar;
			this.useAppLauncher = buffer.useAppLauncher;
			this.onlyInSpaceCenter = buffer.onlyInSpaceCenter;

			this.FlyOver.Apply(buffer.FlyOver);
			this.DistantFlare.Apply(buffer.DistantFlare);
			this.DistantVessel.Apply(buffer.DistantVessel);
			this.SkyboxBrightness.Apply(buffer.SkyboxBrightness);
		}

		private bool hasLoaded = false;
		public void Load()
		{
			ConfigNode configNode = this.LoadSettings();
			if (null == configNode) return;

			this.Reset();

			ConfigNodeWithSteroids settings = ConfigNodeWithSteroids.from(configNode);

			this.debugMode = settings.GetValue<bool>("debugMode", this.debugMode);
			this.useToolbar = settings.GetValue<bool>("useToolbar", this.useToolbar);
			this.useAppLauncher = settings.GetValue<bool>("useAppLauncher", this.useAppLauncher);
			this.onlyInSpaceCenter = settings.GetValue<bool>("onlyInSpaceCenter", this.onlyInSpaceCenter);

			if (settings.HasNode("FlyOver")) this.DistantFlare.Load(ConfigNodeWithSteroids.from(settings.GetNode("FlyOver")));
			if (settings.HasNode("DistantFlare")) this.DistantFlare.Load(ConfigNodeWithSteroids.from(settings.GetNode("DistantFlare")));
			if (settings.HasNode("DistantVessel")) this.DistantVessel.Load(ConfigNodeWithSteroids.from(settings.GetNode("DistantVessel")));
			if (settings.HasNode("SkyboxBrightness")) this.SkyboxBrightness.Load(ConfigNodeWithSteroids.from(settings.GetNode("SkyboxBrightness")));

			hasLoaded = true;
		}

		private ConfigNode LoadSettings()
		{
			if (HighLogic.LoadedSceneIsGame && SIO.File.Exists(Globals.CONFIG_PATHNAME))
				return ConfigNode.Load(Globals.CONFIG_PATHNAME);

			return ConfigNode.Load(Globals.REFERENCE_CONFIG_PATHNAME);
		}

		public void Save()
		{
			ConfigNode settings = new ConfigNode();

			settings.AddValue("debugMode", this.debugMode);
			settings.AddValue("useToolbar", this.useToolbar);
			settings.AddValue("useAppLauncher", this.useAppLauncher);
			settings.AddValue("onlyInSpaceCenter", this.onlyInSpaceCenter);

			this.FlyOver.Save(settings);
			this.DistantFlare.Save(settings);
			this.DistantVessel.Save(settings);
			this.SkyboxBrightness.Save(settings);

			this.saveSettings(settings);
		}

		private void saveSettings(ConfigNode settings)
		{
			Log.force(Globals.CONFIG_DIRECTORY);
			SIO.Directory.CreateDirectory(Globals.CONFIG_DIRECTORY);
			settings.Save(Globals.CONFIG_PATHNAME);

		}

		internal void Commit()
		{
			if (null != VesselDraw.Instance) VesselDraw.Instance.SetActiveTo(this.DistantVessel.renderVessels);
			if (null != FlareDraw.Instance) FlareDraw.Instance.SetActiveTo(this.DistantFlare.flaresEnabled);
			if (null != DarkenSky.Instance) DarkenSky.Instance.SetActiveTo(this.SkyboxBrightness.changeSkybox);
		}
	}
}
