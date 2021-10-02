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
                workingTarget = null;
            }
        }

		private static void VesselCheck(Vessel vessel)
		{
			if (!vessel.loaded && Vector3d.Distance(vessel.GetWorldPos3D(), FlightGlobals.ship_position) < DistantObjectSettings.DistantVessel.maxDistance)
			{
				if (!meshEngineForVessel.ContainsKey(vessel))
				{
					Log.detail("Adding new definition for {0}", vessel.vesselName);
					meshEngineForVessel[vessel] = Contract.MeshEngine.CreateFor(vessel);
				}
				meshEngineForVessel[vessel].Draw();
			}
			else
				CheckErase(vessel);
		}

		private static void VesselLazyCheck(Vessel vessel)
		{
			if (!meshEngineForVessel.ContainsKey(vessel))
			{
				Log.detail("Adding new definition for {0}", vessel.vesselName);
				meshEngineForVessel[vessel] = Contract.MeshEngine.CreateFor(vessel);
			}
			if (Vector3d.Distance(vessel.GetWorldPos3D(), FlightGlobals.ship_position) < DistantObjectSettings.DistantVessel.maxDistance && !vessel.loaded)
			{
				meshEngineForVessel[vessel].Draw();
			}
		}

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
						if (target.GetType().Name == "Vessel")
						{
							workingTarget = FlightGlobals.Vessels.Find(index => index.GetName() == target.GetName());
							VesselCheck(workingTarget);
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

					if (FlightGlobals.Vessels[n].vesselType != VesselType.Flag && FlightGlobals.Vessels[n].vesselType != VesselType.EVA && (FlightGlobals.Vessels[n].vesselType != VesselType.Debris || !DistantObjectSettings.DistantVessel.ignoreDebris))
						VesselCheck(FlightGlobals.Vessels[n]);
				} break;

				case DistantObjectSettings.ERenderMode.RenderAllDontForget:
				{
					n += 1;
					if (n >= FlightGlobals.Vessels.Count)
						n = 0;

					if (FlightGlobals.Vessels[n].vesselType != VesselType.Flag && FlightGlobals.Vessels[n].vesselType != VesselType.EVA && (FlightGlobals.Vessels[n].vesselType != VesselType.Debris || !DistantObjectSettings.DistantVessel.ignoreDebris))
						VesselLazyCheck(FlightGlobals.Vessels[n]);
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
			GameEvents.onVesselCreate.Add(this.OnVesselCreate);
			GameEvents.onVesselChange.Add(this.OnVesselChange);
			GameEvents.onVesselDestroy.Add(this.OnVesselDestroy);
		}

		[UsedImplicitly]
		private void OnDestroy()
		{
			Log.dbg("VesselDraw OnDestroy");
			GameEvents.onVesselDestroy.Remove(this.OnVesselDestroy);
			GameEvents.onVesselChange.Remove(this.OnVesselChange);
			GameEvents.onVesselCreate.Remove(this.OnVesselCreate);
			GameEvents.onGameSceneSwitchRequested.Remove(this.OnGameSceneSwitchRequested);
			INSTANCE = null;
		}

		internal void SetActiveTo(bool renderVessels)
		{
			if (renderVessels)
				this.Activate();
			else
				this.Deactivate();
		}

		private void Activate()
		{
			Log.trace("VesselDraw enabled");
			this.enabled = true;
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

		private void OnVesselCreate(Vessel vessel)
		{
			Log.dbg("Vessel {0} was Created.", vessel.vesselName);
			if (DistantObjectSettings.DistantVessel.renderMode >= DistantObjectSettings.ERenderMode.RenderAllDontForget && vessel.GetType().Name == "Vessel")
			{
				Log.detail("Adding new definition for {0}", vessel.vesselName);
				meshEngineForVessel[vessel] = Contract.MeshEngine.CreateFor(vessel);
			}
		}

		private void OnVesselChange(Vessel vessel)
		{
			Log.dbg("Vessel {0} was Changeg.", vessel.vesselName);
			CheckErase(vessel);	// Current meshes are invalid, we need to reaload them later.
		}

		private void OnVesselDestroy(Vessel vessel)
		{
			Log.dbg("Vessel {0} was Destroyed.", vessel.vesselName);
			if (vessel.Equals(workingTarget)) workingTarget = null;
			CheckErase(vessel);
		}
	}
}
