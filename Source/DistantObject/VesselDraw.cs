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
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class VesselDraw : MonoBehaviour
    {
        private static readonly Dictionary<Vessel, Contract.MeshEngine.Interface> meshEngineForVessel = new Dictionary<Vessel, Contract.MeshEngine.Interface>();
        private static readonly List<Vessel> watchList = new List<Vessel>();
        private static Vessel workingTarget = null;
        private int n = 0;

        public static void DrawVessel(Vessel shipToDraw)
        {
            if (!meshEngineForVessel.ContainsKey(shipToDraw))
                meshEngineForVessel[shipToDraw] = Contract.MeshEngine.CreateFor(shipToDraw);
            meshEngineForVessel[shipToDraw].Draw();
        }

        public static void CheckErase(Vessel shipToErase)
        {
            if (meshEngineForVessel.ContainsKey(shipToErase))
            {
                Log.detail("DistObj: Erasing vessel {0} (vessel unloaded)", shipToErase.vesselName);

                meshEngineForVessel[shipToErase].Destroy();
                meshEngineForVessel.Remove(shipToErase);
                watchList.Remove(shipToErase);
                workingTarget = null;
            }
        }

        public static void VesselCheck(Vessel shipToCheck)
        {
            if (!meshEngineForVessel.ContainsKey(shipToCheck))
            {
                watchList.Add(shipToCheck);
                Log.detail("DistObj: Adding new definition for {0}", shipToCheck.vesselName);
            }
            if (Vector3d.Distance(shipToCheck.GetWorldPos3D(), FlightGlobals.ship_position) < DistantObjectSettings.DistantVessel.maxDistance && !shipToCheck.loaded)
                DrawVessel(shipToCheck);
            else
                CheckErase(shipToCheck);
        }

        [UsedImplicitly]
        private void FixedUpdate()
        {
            if (DistantObjectSettings.DistantVessel.renderVessels)
            {
                for (int i = watchList.Count - 1; i >= 0; --i)
                {
                    if (!FlightGlobals.fetch.vessels.Contains(watchList[i]))
                    {
                        Log.detail("DistObj: Erasing vessel {0} (vessel destroyed)", watchList[i].vesselName);

                        if (meshEngineForVessel.ContainsKey(watchList[i]))
                        {
                            meshEngineForVessel[watchList[i]].Destroy();
                            meshEngineForVessel.Remove(watchList[i]);
                        }
                        watchList.Remove(watchList[i]);
                        workingTarget = null;
                    }
                }

                if (DistantObjectSettings.DistantVessel.renderMode == 0)
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
                        {
                            CheckErase(workingTarget);
                        }
                    }
                    else if (workingTarget != null)
                    {
                        CheckErase(workingTarget);
                    }
                }
                else if (DistantObjectSettings.DistantVessel.renderMode == 1)
                {
                    n += 1;
                    if (n >= FlightGlobals.Vessels.Count)
                    {
                        n = 0;
                    }
                    if (FlightGlobals.Vessels[n].vesselType != VesselType.Flag && FlightGlobals.Vessels[n].vesselType != VesselType.EVA && (FlightGlobals.Vessels[n].vesselType != VesselType.Debris || !DistantObjectSettings.DistantVessel.ignoreDebris))
                    {
                        VesselCheck(FlightGlobals.Vessels[n]);
                    }
                }
            }
        }

        [UsedImplicitly]
        private void Awake()
        {
            //Load settings
            DistantObjectSettings.LoadConfig();

            meshEngineForVessel.Clear();
            watchList.Clear();

            if (!DistantObjectSettings.DistantVessel.renderVessels)
                Log.trace("VesselDraw disabled");
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            Log.dbg("VesselDraw OnDestroy");
        }
    }
}
