/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky

	THIS FILE is ARR to LisiasT. No right other than using the generalted DLL on your machine is granted.
*/
using System.Collections.Generic;
using UnityEngine;

namespace DistantObject.MeshEngine
{
	public class Implementation : DistantObject.Contract.MeshEngine.Interface
	{
		private readonly Vessel vessel;
		private readonly List<GameObject> meshList = new List<GameObject>();
		private Dictionary<GameObject, ProtoPartSnapshot> referencePart = new Dictionary<GameObject, ProtoPartSnapshot>();

		public Implementation(Vessel vessel)
		{
			this.vessel = vessel;
		}

		void DistantObject.Contract.MeshEngine.Interface.Draw()
		{
			Log.detail("Drawing vessel {0}", this.vessel.vesselName);

			List<ProtoPartSnapshot> partList = this.vessel.protoVessel.protoPartSnapshots;
			foreach (ProtoPartSnapshot a in partList)
			{
				string partName;
				if (a.refTransformName.Contains(" "))
				{
					partName = a.partName.Substring(0, a.refTransformName.IndexOf(" "));
				}
				else
				{
					partName = a.partName;
				}

				AvailablePart avPart = PartLoader.getPartInfoByName(partName);

				if (MeshEngine.Contract.Module.IsBlackListed(a))
				{
					Log.detail("Ignoring part {0}", partName);
					continue;
				}

				if (!Database.PartModelDB.ContainsKey(partName))
				{
					partName = partName.Replace('.', '_');
					if (!Database.PartModelDB.ContainsKey(partName))
					{
						Log.error("Could not find config definition for {0}", partName);
						continue;
					}
				}

#if REMOVE_FAILED_PARTS
				List<string> dejects = new List<string>();
#endif
				foreach(string modelName in Database.PartModelDB.Get(partName))
				{ 
					GameObject clone = GameDatabase.Instance.GetModel(modelName);

					// FIXME: I want to get rid of this whole check. See Database.Init for details.
#if REMOVE_FAILED_PARTS
					if (null == clone)
					{
						Log.error("Mesh for model {0} not found! Part {1} will be rendered incomplete (if at all). Removing it from the pool", modelName, partName);
						dejects.Add(modelName);
					}
#else
					if (null == clone) continue; // Silently fails. Checking and logging errors at this place is a fatal performance killer!
#endif
					GameObject cloneMesh = Mesh.Instantiate(clone) as GameObject;
					clone.DestroyGameObject();
					cloneMesh.transform.SetParent(this.vessel.transform);
					cloneMesh.transform.localPosition = a.position;
					cloneMesh.transform.localRotation = a.rotation;

					VesselRanges.Situation situation = this.vessel.vesselRanges.GetSituationRanges(this.vessel.situation);
					if (Vector3d.Distance(cloneMesh.transform.position, FlightGlobals.ship_position) < situation.load)
					{
						Log.error("Tried to draw part {0} within rendering distance of active vessel!", partName);
						continue;
					}
					cloneMesh.SetActive(true);

					foreach (Collider col in cloneMesh.GetComponentsInChildren<Collider>())
					{
						col.enabled = false;
					}

					foreach (ProtoPartModuleSnapshot module in a.modules)
						cloneMesh = DistantObject.MeshEngine.Contract.Module.Render(cloneMesh, a, avPart, module);

					this.referencePart.Add(cloneMesh, a);
					this.meshList.Add(cloneMesh);
				}
#if REMOVE_FAILED_PARTS
				Database.PartModelDB.Remove(dejects);
#endif
			}
		}

		void DistantObject.Contract.MeshEngine.Interface.Destroy()
		{
			this.referencePart.Clear();

			foreach (GameObject mesh in this.meshList)
				UnityEngine.GameObject.Destroy(mesh);
			this.meshList.Clear();
		}
	}
}
