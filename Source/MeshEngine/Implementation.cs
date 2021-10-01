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
			Log.detail("DistObj: Drawing vessel {0}", this.vessel.vesselName);

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

				if (a.modules.Find(n => n.moduleName == "LaunchClamp") != null)
				{
					Log.detail("Ignoring part {0}", partName);
					continue;
				}

				if (!Database.partModel.ContainsKey(partName))
				{
					partName = partName.Replace('.', '_');
					if (!Database.partModel.ContainsKey(partName))
					{
						Log.detail("DistObj ERROR: Could not find config definition for {0}", partName);
						continue;
					}
				}

				GameObject clone = GameDatabase.Instance.GetModel(Database.partModel[partName]);
				if (clone == null)
				{
					Log.detail("DistObj ERROR: Could not load part model {0}", Database.partModel[partName]);
					continue;
				}

				GameObject cloneMesh = Mesh.Instantiate(clone) as GameObject;
				clone.DestroyGameObject();
				cloneMesh.transform.SetParent(this.vessel.transform);
				cloneMesh.transform.localPosition = a.position;
				cloneMesh.transform.localRotation = a.rotation;

				//check if part has TweakScale
				ProtoPartModuleSnapshot tweakScale = a.modules.Find(n => n.moduleName == "TweakScale");
				if (tweakScale != null)
				{
					float defaultScale = float.Parse(tweakScale.moduleValues.GetValue("defaultScale"));
					float currentScale = float.Parse(tweakScale.moduleValues.GetValue("currentScale"));
					float ratio = currentScale / defaultScale;
					if (ratio > 0.001)
					{
						cloneMesh.transform.localScale = new Vector3(ratio, ratio, ratio);
						Log.detail("localScale after {0}", cloneMesh.transform.localScale);
					}
				}

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

				//check if part is a solar panel
				ProtoPartModuleSnapshot solarPanel = a.modules.Find(n => n.moduleName == "ModuleDeployableSolarPanel");
				if (solarPanel != null)
				{
					if (solarPanel.moduleValues.GetValue("stateString") == "EXTENDED")
					{
						//grab the animation name specified in the part cfg
						string animName = avPart.partPrefab.GetComponent<ModuleDeployableSolarPanel>().animationName;
						//grab the actual animation istelf
						var animator = avPart.partPrefab.FindModelAnimators();
						if (animator != null && animator.Length > 0)
						{
							AnimationClip animClip = animator[0].GetClip(animName);
							//grab the animation control module on the actual drawn model
							Animation anim = cloneMesh.GetComponentInChildren<Animation>();
							//copy the animation over to the new part!
							anim.AddClip(animClip, animName);
							anim[animName].enabled = true;
							anim[animName].normalizedTime = 1f;
						}
					}
				}

				//check if part is a light
				ProtoPartModuleSnapshot light = a.modules.Find(n => n.moduleName == "ModuleLight");
				if (light != null)
				{
					//Oddly enough the light already renders no matter what, so we'll kill the module if it's suppsed to be turned off
					if (light.moduleValues.GetValue("isOn") == "False")
					{
						Object.Destroy(cloneMesh.GetComponentInChildren<Light>());
					}
				}

				//check if part is a landing gear
				ProtoPartModuleSnapshot landingGear = a.modules.Find(n => n.moduleName == "ModuleWheelDeployment");
				if (landingGear != null)
				{
					// MOARdV TODO: This wasn't really right to start with.
					// There is no field "savedAnimationTime".
					//if (landingGear.moduleValues.GetValue("savedAnimationTime") != "0")
					{
						//grab the animation name specified in the part cfg
						string animName = avPart.partPrefab.GetComponent<ModuleWheels.ModuleWheelDeployment>().animationStateName;
						var animator = avPart.partPrefab.FindModelAnimators();
						if (animator != null && animator.Length > 0)
						{
							//grab the actual animation istelf
							AnimationClip animClip = animator[0].GetClip(animName);
							//grab the animation control module on the actual drawn model
							Animation anim = cloneMesh.GetComponentInChildren<Animation>();
							//copy the animation over to the new part!
							anim.AddClip(animClip, animName);
							anim[animName].enabled = true;
							anim[animName].normalizedTime = 1f;
						}
					}
				}

				//check if part has a generic animation
				ProtoPartModuleSnapshot animGeneric = a.modules.Find(n => n.moduleName == "ModuleAnimateGeneric");
				if (animGeneric != null)
				{
					if (animGeneric.moduleValues.GetValue("animTime") != "0")
					{
						//grab the animation name specified in the part cfg
						string animName = avPart.partPrefab.GetComponent<ModuleAnimateGeneric>().animationName;
						var animator = avPart.partPrefab.FindModelAnimators();
						if (animator != null && animator.Length > 0)
						{
							//grab the actual animation istelf
							AnimationClip animClip = animator[0].GetClip(animName);
							//grab the animation control module on the actual drawn model
							Animation anim = cloneMesh.GetComponentInChildren<Animation>();
							//copy the animation over to the new part!
							anim.AddClip(animClip, animName);
							anim[animName].enabled = true;
							anim[animName].normalizedTime = 1f;
						}
					}
				}

				this.referencePart.Add(cloneMesh, a);
				this.meshList.Add(cloneMesh);
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
