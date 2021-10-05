/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky
*/
using System.Collections.Generic;
using KSPe.Annotations;
using UnityEngine;

namespace DistantObject
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class VesselDraw : MonoBehaviour
    {
		private static VesselDraw INSTANCE = null;
		internal static VesselDraw Instance => INSTANCE;

        private static readonly Dictionary<Vessel, Contract.MeshEngine.Interface> meshEngineForVessel = new Dictionary<Vessel, Contract.MeshEngine.Interface>();
        private static Vessel workingTarget = null;	// Used on Rendering Mode 0 (Only Targeted as rendered)
        private int n = 0;

        private static void CheckErase(Vessel shipToErase)
        {
            if (meshEngineForVessel.ContainsKey(shipToErase))
            {
                Log.detail("Erasing vessel {0} (vessel unloaded)", shipToErase.vesselName);

                meshEngineForVessel[shipToErase].Destroy();
                meshEngineForVessel.Remove(shipToErase);
                workingTarget = shipToErase == workingTarget ? null : workingTarget;
            }
        }

		private static void CheckDraw(Vessel vessel)
		{
			if (!vessel.loaded && Vector3d.Distance(vessel.GetWorldPos3D(), FlightGlobals.ship_position) < DistantObjectSettings.DistantVessel.maxDistance)
			{
				VesselCheck(vessel);
				meshEngineForVessel[vessel].Draw();
			}
			else
				CheckErase(vessel);
		}

		private static void LazyCheckDraw(Vessel vessel)
		{
			VesselCheck(vessel);
			if (!vessel.loaded && Vector3d.Distance(vessel.GetWorldPos3D(), FlightGlobals.ship_position) < DistantObjectSettings.DistantVessel.maxDistance)
			{
				meshEngineForVessel[vessel].Draw();
			}
		}

		private static void VesselCheck(Vessel vessel)
		{
			if (!meshEngineForVessel.ContainsKey(vessel))
			{
				Log.detail("Adding new definition for {0}", vessel.vesselName);
				meshEngineForVessel[vessel] = Contract.MeshEngine.CreateFor(vessel);
			}
		}

		private void DoHouseKeeping()
		{
			switch(DistantObjectSettings.DistantVessel.renderMode)
			{
				case DistantObjectSettings.ERenderMode.RenderTargetOnly:
				{
					List<Vessel> list = new List<Vessel>(meshEngineForVessel.Keys);
					foreach (Vessel i in list) if (i != workingTarget)
						CheckErase(i);
				}  break;

				case DistantObjectSettings.ERenderMode.RenderAll:
				{
					List<Vessel> list = new List<Vessel>(meshEngineForVessel.Keys);
					foreach (Vessel i in list) CheckErase(i);
				} break;

				default: break;
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
			switch(DistantObjectSettings.DistantVessel.renderMode)
			{
				case DistantObjectSettings.ERenderMode.RenderTargetOnly:
				{
					ITargetable target = FlightGlobals.fetch.VesselTarget;
					if (target != null)
					{
						if (target is Vessel && !FORBIDDEN_VESSELS.Contains(((Vessel)target).vesselType))
						{
							workingTarget = FlightGlobals.Vessels.Find(index => index.GetName() == target.GetName());
							CheckDraw(workingTarget);
						}
						else if (workingTarget != null)
							CheckErase(workingTarget);
					}
					else if (workingTarget != null)
						CheckErase(workingTarget);
				} break;

				case DistantObjectSettings.ERenderMode.RenderAll:
				{
					n += 1;
					if (n >= FlightGlobals.Vessels.Count)
						n = 0;

					if (!FORBIDDEN_VESSELS.Contains(FlightGlobals.Vessels[n].vesselType) && !(FlightGlobals.Vessels[n].vesselType is VesselType.Debris  && DistantObjectSettings.DistantVessel.ignoreDebris))
						CheckDraw(FlightGlobals.Vessels[n]);
				} break;

				case DistantObjectSettings.ERenderMode.RenderAllDontForget:
				{
					n += 1;
					if (n >= FlightGlobals.Vessels.Count)
						n = 0;

					if (!FORBIDDEN_VESSELS.Contains(FlightGlobals.Vessels[n].vesselType) && !(FlightGlobals.Vessels[n].vesselType is VesselType.Debris  && DistantObjectSettings.DistantVessel.ignoreDebris))
						LazyCheckDraw(FlightGlobals.Vessels[n]);
				} break;
			}
		}

        [UsedImplicitly]
        private void Awake()
        {
            INSTANCE = this;

            //Load settings
            DistantObjectSettings.LoadConfig();

            meshEngineForVessel.Clear();
            Object.DontDestroyOnLoad(this);
        }

		[UsedImplicitly]
		private void Start()
		{
			GameEvents.onGameSceneSwitchRequested.Add(this.OnGameSceneSwitchRequested);
			GameEvents.onVesselChange.Add(this.OnVesselChange);
			GameEvents.onVesselGoOnRails.Add(this.OnVesselGoOnRails);
			GameEvents.onVesselGoOffRails.Add(this.OnVesselGoOffRails);
			GameEvents.onVesselWillDestroy.Add(this.OnVesselWillDestroy);
			DistantObjectSettings.Commit();
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			Log.dbg("VesselDraw OnDestroy");
			GameEvents.onVesselWillDestroy.Remove(this.OnVesselWillDestroy);
			GameEvents.onVesselGoOffRails.Add(this.OnVesselGoOffRails);
			GameEvents.onVesselGoOnRails.Add(this.OnVesselGoOnRails);
			GameEvents.onVesselChange.Remove(this.OnVesselChange);
			GameEvents.onGameSceneSwitchRequested.Remove(this.OnGameSceneSwitchRequested);
			INSTANCE = null;
		}

		internal void SetActiveTo(bool renderVessels)
		{
			this.enabled = false;	// Guarantee this is disabled on every scene but the intended ones!
			if (!HighLogic.LoadedSceneIsFlight) return;

			if (renderVessels)
				this.Activate();
			else
				this.Deactivate();
		}

		private void Activate()
		{
			Log.trace("VesselDraw enabled");
			this.enabled = true;
			this.DoHouseKeeping();
		}

		private void Deactivate()
		{
			Log.trace("VesselDraw disabled");
			this.enabled = false;
			workingTarget = null;
			foreach (KeyValuePair<Vessel, Contract.MeshEngine.Interface> tuple in meshEngineForVessel)
			{
				Log.detail("Erasing vessel {0} (DOE deactivated)", tuple.Key.vesselName);
				tuple.Value.Destroy();
			}
			meshEngineForVessel.Clear();
		}

		private void OnGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> data)
		{
			if (data.to.Equals(GameScenes.MAINMENU))	this.Deactivate();
			else if (data.to.Equals(GameScenes.FLIGHT))	this.SetActiveTo(DistantObjectSettings.DistantVessel.renderVessels);
		}

		private void OnVesselChange(Vessel vessel)
		{
			Log.dbg("Vessel {0} was Changeg.", vessel.vesselName);
			CheckErase(vessel);	// Current meshes are invalid, we need to reaload them later.
		}

		private void OnVesselGoOnRails(Vessel vessel)
		{
			Log.dbg("Vessel {0} Gone ON Rails.", vessel.vesselName);
			if (DistantObjectSettings.DistantVessel.renderMode >= DistantObjectSettings.ERenderMode.RenderAllDontForget && vessel.GetType().Name == "Vessel")
				VesselCheck(vessel);
		}

		private void OnVesselGoOffRails(Vessel vessel)
		{
			Log.dbg("Vessel {0} Gone OFF Rails.", vessel.vesselName);
			CheckErase(vessel);	// Current meshes are invalid, we need to reaload them later.
		}

		private void OnVesselWillDestroy(Vessel vessel)
		{
			Log.dbg("Vessel {0} was Destroyed.", vessel.vesselName);
			if (vessel.Equals(workingTarget)) workingTarget = null;
			CheckErase(vessel);
		}
	}
}
