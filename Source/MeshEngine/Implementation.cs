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
using System.Collections.Generic;
using UnityEngine;

namespace DistantObject.MeshEngine
{
	public class Implementation : DistantObject.Contract.MeshEngine.Interface
	{
		private readonly Vessel vessel;
		private readonly Dictionary<ProtoPartSnapshot, GameObject> meshes = new Dictionary<ProtoPartSnapshot, GameObject>();

		public Implementation(Vessel vessel)
		{
			this.vessel = vessel;
			this.BuildMeshDatabase();
		}

		private void BuildMeshDatabase()
		{
			List<ProtoPartSnapshot> partList = this.vessel.protoVessel.protoPartSnapshots;
			foreach (ProtoPartSnapshot a in partList)
			{ 
				string partName = (a.refTransformName.Contains(" "))
						? a.partName.Substring(0, a.refTransformName.IndexOf(" "))
						: a.partName
					;

				if (MeshEngine.Contract.Module.IsBlackListed(a))
				{
					Log.trace("Ignoring part {0}", partName);
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

				foreach(string modelName in Database.PartModelDB.Get(partName))
				{ 
					GameObject clone = GameDatabase.Instance.GetModel(modelName);

					if (null == clone)
					{
						Log.warn("Failed to load model {0} for part {1} from vessel {2}! Vessel will not be rendered as expected!", modelName, a.partName, this.vessel.vesselName);
						continue;
					}

					GameObject cloneMesh = Mesh.Instantiate(clone) as GameObject;
					clone.DestroyGameObject();
					this.meshes[a] = cloneMesh;
				}
			}
		}

		void DistantObject.Contract.MeshEngine.Interface.Draw()
		{
			Log.trace("Drawing vessel {0}", this.vessel.vesselName);

			foreach (ProtoPartSnapshot a in this.vessel.protoVessel.protoPartSnapshots)
			{ 
				string partName = (a.refTransformName.Contains(" "))
						? a.partName.Substring(0, a.refTransformName.IndexOf(" "))
						: a.partName
					;

				if (!this.meshes.ContainsKey(a)) continue; // Fails silently.

				// NOTE: This code (until the foreach ProtoPartModuleSnapshot below) could be successfuly migreted into the BuildMeshDatabase,
				// as once a vessel goes into Rails (i.e., it's "unloaded" and became available only as a ProtoVessel), it's not expected
				// that it will change - it's the whole reason we put things into Rails! :)
				//
				// However, some add'ons as PersistantRotation, effectivelly changes the ProtoVessel over time!
				//
				// So I decided to keep doing these things here (what theoretically is "wasteful") in order to be compatible with
				// these guys.
				GameObject cloneMesh = this.meshes[a];

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

				// This is the foreach I mentioned on the NOTE above.
				foreach (ProtoPartModuleSnapshot module in a.modules)
					cloneMesh = DistantObject.MeshEngine.Contract.Module.Render(cloneMesh, a, PartLoader.getPartInfoByName(partName), module);
			}
		}

		void DistantObject.Contract.MeshEngine.Interface.Destroy()
		{
			foreach (KeyValuePair<ProtoPartSnapshot, GameObject> mesh in this.meshes)
				UnityEngine.GameObject.Destroy(mesh.Value);
			this.meshes.Clear();
		}
	}
}
