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
using System.Collections.Generic;
using KSPe.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DistantObject
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class VesselDraw:MonoBehaviour
	{
		private static VesselDraw __instance = null;
		internal static VesselDraw Instance => __instance;

		private Vessel workingTarget = null; // Used on Rendering Mode 0 (Only Targeted as rendered)
		private int n = 0;  // Clever trick to allow checking one vessel per frame! Nice!

		private void CheckErase(Vessel shipToErase)
		{
			this.workingTarget = shipToErase == this.workingTarget ? null : this.workingTarget;
			VesselDrawDatabase.Instance.CheckErase(shipToErase);
		}

		private void CheckDraw(Vessel vessel)
		{
			if (!vessel.loaded && Vector3d.Distance(vessel.GetWorldPos3D(), FlightGlobals.ship_position) < Settings.Instance.DistantVessel.maxDistance)
			{
				VesselDrawDatabase.Instance.VesselCheck(vessel);
				VesselDrawDatabase.Instance.Draw(vessel);
			}
			else
				this.CheckErase(vessel);
		}

		private void LazyCheckDraw(Vessel vessel)
		{
			VesselDrawDatabase.Instance.VesselCheck(vessel);
			if (!vessel.loaded && Vector3d.Distance(vessel.GetWorldPos3D(), FlightGlobals.ship_position) < Settings.Instance.DistantVessel.maxDistance)
			{
				VesselDrawDatabase.Instance.Draw(vessel);
			}
		}

		private static readonly List<VesselType> FORBIDDEN_VESSELS = new List<VesselType>(new VesselType[]{
				VesselType.EVA,
				VesselType.Flag,
				VesselType.SpaceObject
			});
		[UsedImplicitly]
		private void Update()
		{
			switch(Settings.Instance.DistantVessel.renderMode)
			{
				case Settings.ERenderMode.RenderTargetOnly:
				{
					ITargetable target = FlightGlobals.fetch.VesselTarget;
					if (target != null)
					{
						if (target is Vessel && !FORBIDDEN_VESSELS.Contains(((Vessel)target).vesselType))
						{
							this.workingTarget = FlightGlobals.Vessels.Find(index => index.GetName() == target.GetName());
							this.CheckDraw(workingTarget);
						}
						else if (null != this.workingTarget)
							this.CheckErase(workingTarget);
					}
					else if (null != this.workingTarget)
						this.CheckErase(workingTarget);
				} break;

				case Settings.ERenderMode.RenderAll:
				{
					n = ++n % FlightGlobals.Vessels.Count;
					if (!FORBIDDEN_VESSELS.Contains(FlightGlobals.Vessels[n].vesselType) && !(FlightGlobals.Vessels[n].vesselType is VesselType.Debris && Settings.Instance.DistantVessel.ignoreDebris))
						this.CheckDraw(FlightGlobals.Vessels[n]);
				} break;

				case Settings.ERenderMode.RenderAllDontForget:
				{
					n = ++n % FlightGlobals.Vessels.Count;
					if (!FORBIDDEN_VESSELS.Contains(FlightGlobals.Vessels[n].vesselType) && !(FlightGlobals.Vessels[n].vesselType is VesselType.Debris && Settings.Instance.DistantVessel.ignoreDebris))
						this.LazyCheckDraw(FlightGlobals.Vessels[n]);
				} break;
			}
		}

		[UsedImplicitly]
		private void Awake()
		{
			__instance = this;
		}

		[UsedImplicitly]
		private void Start()
		{
			Log.dbg("VesselDraw Start");

			//Load settings
			Settings.Instance.Load();

			GameEvents.onVesselChange.Add(this.OnVesselChange);
			GameEvents.onVesselGoOnRails.Add(this.OnVesselGoOnRails);
			GameEvents.onVesselGoOffRails.Add(this.OnVesselGoOffRails);
			GameEvents.onVesselWillDestroy.Add(this.OnVesselWillDestroy);

			Settings.Instance.Commit();
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			Log.dbg("VesselDraw OnDestroy");
			GameEvents.onVesselWillDestroy.Remove(this.OnVesselWillDestroy);
			GameEvents.onVesselGoOffRails.Remove(this.OnVesselGoOffRails);
			GameEvents.onVesselGoOnRails.Remove(this.OnVesselGoOnRails);
			GameEvents.onVesselChange.Remove(this.OnVesselChange);
			__instance = null;
		}

		internal void SetActiveTo(bool renderVessels)
		{
			Log.detail("VesselDraw.SetActiveTo {0}, it was {1}", renderVessels, this.enabled);
			this.enabled = renderVessels;
			VesselDrawDatabase.Instance.DoHouseKeeping(this.workingTarget);
		}

		private void OnVesselChange(Vessel vessel)
		{
			Log.detail("Vessel {0} was Changes.", vessel.vesselName);
			CheckErase(vessel);	// Current meshes are invalid, we need to reaload them later.
		}

		private void OnVesselGoOnRails(Vessel vessel)
		{
			Log.detail("Vessel {0} Gone ON Rails.", vessel.vesselName);
			if (Settings.Instance.DistantVessel.renderMode >= Settings.ERenderMode.RenderAllDontForget && vessel.GetType().Name == "Vessel")
				VesselDrawDatabase.Instance.VesselCheck(vessel);
		}

		private void OnVesselGoOffRails(Vessel vessel)
		{
			Log.detail("Vessel {0} Gone OFF Rails.", vessel.vesselName);
			CheckErase(vessel);	// Current meshes are invalid, we need to reaload them later.
		}

		private void OnVesselWillDestroy(Vessel vessel)
		{
			Log.detail("Vessel {0} was Destroyed.", vessel.vesselName);
			if (vessel.Equals(workingTarget)) workingTarget = null;
			CheckErase(vessel);
		}
	}

	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class VesselDrawDatabase:MonoBehaviour
	{
		private static VesselDrawDatabase INSTANCE = null;
		internal static VesselDrawDatabase Instance => INSTANCE;

		private readonly Dictionary<Vessel, Contract.MeshEngine.Interface> meshEngineForVessel = new Dictionary<Vessel, Contract.MeshEngine.Interface>();

		[UsedImplicitly]
		private void Awake()
		{
			INSTANCE = this;
			Object.DontDestroyOnLoad(this);
		}

		[UsedImplicitly]
		private void Start()
		{
			GameEvents.onGameSceneSwitchRequested.Add(this.OnGameSceneSwitchRequested);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		[UsedImplicitly]
		private void Destroy()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			GameEvents.onGameSceneSwitchRequested.Remove(this.OnGameSceneSwitchRequested);
		}

		internal void ClearBD()
		{
			foreach (KeyValuePair<Vessel, Contract.MeshEngine.Interface> tuple in this.meshEngineForVessel)
			{
				Log.detail("Erasing vessel {0} (DOE deactivated)", tuple.Key.vesselName);
				tuple.Value.Destroy();
			}
			this.meshEngineForVessel.Clear();
		}

		internal void Draw(Vessel vessel) => this.meshEngineForVessel[vessel].Draw();

		internal void VesselCheck(Vessel vessel)
		{
			if (!this.meshEngineForVessel.ContainsKey(vessel))
			{
				Log.detail("Adding new definition for {0}", vessel.vesselName);
				this.meshEngineForVessel[vessel] = Contract.MeshEngine.CreateFor(vessel);
			}
		}

		internal void CheckErase(Vessel shipToErase)
		{
			if (this.meshEngineForVessel.ContainsKey(shipToErase))
			{
				Log.detail("Erasing vessel {0} (vessel unloaded)", shipToErase.vesselName);

				this.meshEngineForVessel[shipToErase].Destroy();
				this.meshEngineForVessel.Remove(shipToErase);
			}
		}

		internal void DoHouseKeeping(Vessel workingTarget = null)
		{
			if (!VesselDraw.Instance.enabled)
			{
				this.ClearBD();
				return;
			}

			switch(Settings.Instance.DistantVessel.renderMode)
			{
				case Settings.ERenderMode.RenderTargetOnly:
				{
					List<Vessel> list = new List<Vessel>(this.meshEngineForVessel.Keys);
					foreach (Vessel i in list) if (i != workingTarget)
						CheckErase(i);
				}  break;

				case Settings.ERenderMode.RenderAll:
				{
					List<Vessel> list = new List<Vessel>(this.meshEngineForVessel.Keys);
					foreach (Vessel i in list) if (Vector3d.Distance(i.GetWorldPos3D(), FlightGlobals.ship_position) >= Settings.Instance.DistantVessel.maxDistance)
						CheckErase(i);
				} break;

				default: break;
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Log.detail("Scene changed to {0} mode {1} ({2})", scene, mode, HighLogic.LoadedScene);

			switch (HighLogic.LoadedScene)
			{
				case GameScenes.FLIGHT:
					// This is being done by VesselDraw's Start, as it's being instantiated on every Flight again.
					//this.DoHouseKeeping();
					break;
				default:
					break;
			}
		}

		private void OnGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> scenes)
		{
			Log.detail("Scene will change from {0} to {1}", scenes.from, scenes.to);
			switch (scenes.from)
			{
				case GameScenes.FLIGHT:
					switch (scenes.to)
					{
						case GameScenes.SPACECENTER:
						case GameScenes.TRACKSTATION:
							break;
						default:
							this.ClearBD();
							break;
					}
					break;

				default:
					break;
			}
		}
	}
}
