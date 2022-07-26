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
using System;
using System.Collections.Generic;

using KSPe.Annotations;

using UnityEngine;

//#define SHOW_FIXEDUPDATE_TIMING

namespace DistantObject
{
	abstract class Flare
	{
		public readonly GameObject mesh;
		public readonly MeshRenderer meshRenderer;
		public readonly Vector4 hslColor;
		public abstract double sizeInDegrees { get; }

		public Flare(string name, GameObject flare, Color colour)
		{
			GameObject flareMesh = Mesh.Instantiate(flare) as GameObject;
			UnityEngine.Object.Destroy(flareMesh.GetComponent<Collider>());
			flareMesh.name = name;
			flareMesh.SetActive(true);
			this.mesh = flareMesh;

			MeshRenderer flareMR = flareMesh.GetComponentInChildren<MeshRenderer>();

			// With KSP 1.0, putting these on layer 10 introduces 
			// ghost flares that render for a while before fading away.
			// These flares were moved to 10 because of an
			// interaction with PlanetShine.  However, I don't see
			// that problem any longer (where flares changed brightness
			// during sunrise / sunset).  Valerian proposes instead using 15.

			// MOARdV: valerian recommended moving vessel and body flares to
			// layer 10, but that behaves poorly for nearby / co-orbital objects.
			// Move vessels back to layer 0 until I can find a better place to
			// put it.
			// Renderer layers: http://wiki.kerbalspaceprogram.com/wiki/API:Layers

			flareMR.receiveShadows = false;
			flareMR.gameObject.layer = 15;
			flareMR.material.shader = Shader.Find("KSP/Alpha/Unlit Transparent");
			flareMR.material.color = colour;
			flareMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			flareMR.receiveShadows = false;
			this.meshRenderer = flareMR;
			this.hslColor = Utility.RGB2HSL(flareMR.material.color);
		}

		public void Destroy()
		{
			if (null != this.meshRenderer)
			{
				if (null != this.meshRenderer.material)
					UnityEngine.Object.Destroy(this.meshRenderer.material);
				UnityEngine.Object.Destroy(this.meshRenderer);
			}
			if (null != this.mesh)
				UnityEngine.Object.Destroy(this.mesh);
		}

		// Faster, but by some reason Minmus is not triggered by this one!
		public bool IsOnFieldOfViewOf2(Camera camera)
		{
			Vector3 vector = (camera.transform.position - this.meshRenderer.transform.position);
			float angle = Vector3.Angle(FlightCamera.fetch.mainCamera.transform.forward, vector);
			return (angle > FlightCamera.fetch.mainCamera.fieldOfView / 2);
		}

		// Slower but precise
		public bool IsOnFieldOfViewOf(Camera camera)
		{
			Vector3 screenPoint = camera.WorldToViewportPoint(this.meshRenderer.transform.position);
			return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		}

		public bool IsVisibleFrom(Camera camera)
		{
			if (!this.IsOnFieldOfViewOf(camera)) return false;
			Vector3 heading = this.meshRenderer.transform.position - camera.transform.position;
			Vector3 direction = heading.normalized;
			if (Physics.Linecast(camera.transform.position, this.meshRenderer.bounds.center, out RaycastHit hit))
				return hit.transform.name == this.meshRenderer.transform.name;
			return true;
		}
	}

	// @ 1920x1080, 1 pixel with 60* FoV covers about 2 minutes of arc / 0.03 degrees
	class BodyFlare:Flare
	{
		public static double kerbinSMA = -1.0;
		public static double kerbinRadius;

		// Scale body flare distance to try to ameliorate z-fighting of moons.
		public static double bodyFlareDistanceScalar = 0.0;

		public static readonly double MinFlareDistance = 739760.0;
		public static readonly double MaxFlareDistance = 750000.0;
		public static readonly double FlareDistanceRange = MaxFlareDistance - MinFlareDistance;

		private double __sizeInDegrees = 0;
		public override double sizeInDegrees => this.__sizeInDegrees;

		public readonly CelestialBody body;
		private readonly Renderer scaledRenderer;
		public readonly Color color;

		public Vector3d cameraToBodyUnitVector;
		public double distanceFromCamera;

		private readonly double relativeRadiusSquared;
		private readonly double bodyRadiusSquared;

		internal BodyFlare(CelestialBody body, GameObject flare, Dictionary<CelestialBody, Color> bodyColors) : base(body.bodyName, flare, (bodyColors.ContainsKey(body)) ? bodyColors[body] : Color.white)
		{
			this.body = body;
			Renderer scaledRenderer = body.MapObject.transform.GetComponent<Renderer>();

			this.scaledRenderer = scaledRenderer;
			this.color = this.meshRenderer.material.color;
			this.relativeRadiusSquared = Math.Pow(body.Radius / FlightGlobals.Bodies[1].Radius, 2.0);
			this.bodyRadiusSquared = body.Radius * body.Radius;
			this.mesh.SetActive(DistantObjectSettings.DistantFlare.flaresEnabled);

			// LisiasT: Movint the body flares to layer 8, behing the Atmosphere.
			this.meshRenderer.gameObject.layer = 8;
		}

		~BodyFlare()
		{
			Log.dbg("BodyFlare {0} Destroy", (body != null) ? body.name : "(null bodyflare?)");
		}

		public void Update(Vector3d camPos, float camFOV)
		{
			// Update Body Flare
			Vector3d targetVectorToSun = FlightGlobals.Bodies[0].position - body.position;
			Vector3d targetVectorToCam = camPos - body.position;

			double targetSunRelAngle = Vector3d.Angle(targetVectorToSun, targetVectorToCam);

			cameraToBodyUnitVector = -targetVectorToCam.normalized;
			distanceFromCamera = targetVectorToCam.magnitude;

			double kerbinSMAOverBodyDist = kerbinSMA / targetVectorToSun.magnitude;
			double luminosity = kerbinSMAOverBodyDist * kerbinSMAOverBodyDist * relativeRadiusSquared;
			luminosity *= (0.5 + (32400.0 - targetSunRelAngle * targetSunRelAngle) / 64800.0);
			luminosity = (Math.Log10(luminosity) + 1.5) * (-2.0);

			// We need to clamp this value to remain < 5, since larger values cause a negative resizeVector.
			// This only appears to happen with some mod-generated worlds, but it's still a good practice
			// and not terribly expensive.
			float brightness = Math.Min(4.99f, (float)(luminosity + Math.Log10(distanceFromCamera / kerbinSMA)));

			//position, rotate, and scale mesh
			targetVectorToCam = ((MinFlareDistance + Math.Min(FlareDistanceRange, distanceFromCamera * bodyFlareDistanceScalar)) * targetVectorToCam.normalized);
			mesh.transform.position = camPos - targetVectorToCam;
			mesh.transform.LookAt(camPos);

			float resizeFactor = (-750.0f * (brightness - 5.0f) * (0.7f + .99f * camFOV) / 70.0f) * DistantObjectSettings.DistantFlare.flareSize;
			mesh.transform.localScale = new Vector3(resizeFactor, resizeFactor, resizeFactor);

			this.__sizeInDegrees = Math.Acos(Math.Sqrt(distanceFromCamera * distanceFromCamera - bodyRadiusSquared) / distanceFromCamera) * Mathf.Rad2Deg;

			// Disable the mesh if the scaledRenderer is enabled and visible.
			mesh.SetActive(!(scaledRenderer.enabled && scaledRenderer.isVisible));
		}
	}

	class VesselFlare:Flare
	{
		public override double sizeInDegrees => 5.0;

		public Vessel referenceShip;
		public readonly float luminosity;
		public float brightness;

		internal VesselFlare(Vessel referenceShip, GameObject flare) : base(referenceShip.vesselName, flare, Color.white)
		{
			this.referenceShip = referenceShip;
			this.luminosity = 5.0f + Mathf.Pow(referenceShip.GetTotalMass(), 1.25f);
			this.brightness = 0.0f;
		}

		~VesselFlare()
		{
			// Why is this never called?
			Log.dbg("VesselFlare {0} Destroy", (referenceShip != null) ? referenceShip.vesselName : "(null vessel?)");
		}

		public void Update(Vector3d camPos, float camFOV)
		{
			try
			{
				Vector3d targetVectorToCam = camPos - referenceShip.transform.position;
				float targetDist = (float)Vector3d.Distance(referenceShip.transform.position, camPos);
				bool activeSelf = this.mesh.activeSelf;
				if (targetDist > 750000.0f && activeSelf)
				{
					this.mesh.SetActive(false);
					activeSelf = false;
				}
				else if (targetDist < 750000.0f && !activeSelf)
				{
					this.mesh.SetActive(true);
					activeSelf = true;
				}

				if (activeSelf)
				{
					brightness = Mathf.Log10(luminosity) * (1.0f - Mathf.Pow(targetDist / 750000.0f, 1.25f));

					this.mesh.transform.position = camPos - targetDist * targetVectorToCam.normalized;
					this.mesh.transform.LookAt(camPos);
					float resizeFactor = (0.002f * targetDist * brightness * (0.7f + .99f * camFOV) / 70.0f) * DistantObjectSettings.DistantFlare.flareSize;

					this.mesh.transform.localScale = new Vector3(resizeFactor, resizeFactor, resizeFactor);
					Log.dbg("Resizing vessel flare {0} to {1} - brightness {2}, luminosity {3}", referenceShip.vesselName, resizeFactor, brightness, luminosity);
				}
			}
			catch
			{
				// If anything went whack, let's disable ourselves
				this.mesh.SetActive(false);
				referenceShip = null;
			}
		}
	}

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class FlareDraw:MonoBehaviour
	{
		private static FlareDraw INSTANCE = null;
		internal static FlareDraw Instance => INSTANCE;

		private const string MODEL = "DistantObject/Flare/model";

		enum FlareType
		{
			Celestial,
			Vessel,
			Debris
		}

		private readonly List<BodyFlare> bodyFlares = new List<BodyFlare>();
		private readonly Dictionary<Vessel, VesselFlare> vesselFlares = new Dictionary<Vessel, VesselFlare>();

		private static float camFOV;
		private Vector3d camPos;
		private float atmosphereFactor = 1.0f;
		private float dimFactor = 1.0f;

		// Track the variables relevant to determine whether the sun is
		// occluding a body flare.
		private double sunDistanceFromCamera = 1.0;
		private double sunSizeInDegrees = 1.0;
		private double sunRadiusSquared;
		private Vector3d cameraToSunUnitVector = Vector3d.zero;

		private static bool ExternalControl = false;

		private readonly List<Vessel.Situations> situations = new List<Vessel.Situations>();

		private string showNameString = null;
		private Transform showNameTransform = null;
		private Color showNameColor;

		// If something goes wrong (say, because another mod does something bad
		// that screws up vessels without us seeing the normal "vessel destroyed"
		// callback, we can see exceptions in Update.  If that happens, we use
		// the bigHammer to rebuild our vessel flare table outright.
		private bool bigHammer = false;
		private List<Vessel> deadVessels = new List<Vessel>();

		private GameObject flare;

#if SHOW_FIXEDUPDATE_TIMING
        private Stopwatch stopwatch = new Stopwatch();
#endif

		private Dictionary<CelestialBody, Color> bodyColors => __bodyColors ?? (__bodyColors = buildBodyColors());
		private static Dictionary<CelestialBody, Color> __bodyColors;
		private static Dictionary<CelestialBody, Color> buildBodyColors()
		{
			Dictionary<CelestialBody, Color> r = new Dictionary<CelestialBody, Color>();
			foreach (UrlDir.UrlConfig node in GameDatabase.Instance.GetConfigs("CelestialBodyColor"))
			{
				CelestialBody body = FlightGlobals.Bodies.Find(n => n.name == node.config.GetValue("name"));
				if (FlightGlobals.Bodies.Contains(body))
				{
					Color color = ConfigNode.ParseColor(node.config.GetValue("color"));
					color.r = 1.0f - (DistantObjectSettings.DistantFlare.flareSaturation * (1.0f - (color.r / 255.0f)));
					color.g = 1.0f - (DistantObjectSettings.DistantFlare.flareSaturation * (1.0f - (color.g / 255.0f)));
					color.b = 1.0f - (DistantObjectSettings.DistantFlare.flareSaturation * (1.0f - (color.b / 255.0f)));
					color.a = 1.0f;
					if (!r.ContainsKey(body))
					{
						r.Add(body, color);
					}
				}
			}
			return r;
		}

		//--------------------------------------------------------------------
		// AddVesselFlare
		// Add a new vessel flare to our library
		private void AddVesselFlare(Vessel referenceShip)
		{
			VesselFlare vesselFlare = new VesselFlare(referenceShip, this.flare);
			vesselFlares.Add(referenceShip, vesselFlare);
		}

		//private void ListChildren(PSystemBody body, int idx)
		//{
		//    StringBuilder sb = new StringBuilder();
		//    for(int i=0; i< idx; ++i) sb.Append("  ");
		//    sb.Append("Body ");
		//    sb.Append(body.celestialBody.name);
		//    Log.force(sb.ToString());
		//    for(int i=0; i<body.children.Count; ++i)
		//    {
		//        ListChildren(body.children[i], idx + 1);
		//    }
		//}

		//--------------------------------------------------------------------
		// GenerateBodyFlares
		// Iterate over the celestial bodies and generate flares for each of
		// them.  Add the flare info to the dictionary.
		private void GenerateBodyFlares()
		{
			Log.dbg("GenerateBodyFlares");
			// If Kerbin is parented to the Sun, set its SMA - otherwise iterate
			// through celestial bodies to locate which is parented to the Sun
			// and has Kerbin as a child. Set the highest parent's SMA to kerbinSMA.
			if (BodyFlare.kerbinSMA <= 0.0)
			{
				if (FlightGlobals.Bodies[1].referenceBody == FlightGlobals.Bodies[0])
				{
					BodyFlare.kerbinSMA = FlightGlobals.Bodies[1].orbit.semiMajorAxis;
				}
				else
				{
					foreach (CelestialBody current in FlightGlobals.Bodies)
					{
						if (current != FlightGlobals.Bodies[0])
						{
							if (current.referenceBody == FlightGlobals.Bodies[0] && current.HasChild(FlightGlobals.Bodies[1]))
							{
								BodyFlare.kerbinSMA = current.orbit.semiMajorAxis;
							}
						}
					}

					if (BodyFlare.kerbinSMA <= 0.0)
					{
						throw new Exception("Distant Object -- Unable to find Kerbin's relationship to Kerbol.");
					}
				}

				BodyFlare.kerbinRadius = FlightGlobals.Bodies[1].Radius;
			}
			bodyFlares.Clear();

			double largestSMA = 0.0;
			foreach (CelestialBody body in FlightGlobals.Bodies)
			{
				if (body != FlightGlobals.Bodies[0] && body?.MapObject != null)
				{
					largestSMA = Math.Max(largestSMA, body.orbit.semiMajorAxis);
					BodyFlare bf = new BodyFlare(body, this.flare, this.bodyColors);
					bodyFlares.Add(bf);
					Log.dbg("Body {0}:{1} added to bodyFlares.", body.bodyName, body.displayName);
				}
			}
			BodyFlare.bodyFlareDistanceScalar = BodyFlare.FlareDistanceRange / largestSMA;
		}

		//--------------------------------------------------------------------
		// GenerateVesselFlares
		// Iterate over the vessels, adding and removing flares as appropriate
		private void GenerateVesselFlares()
		{
#if SHOW_FIXEDUPDATE_TIMING
                stopwatch.Reset();
                stopwatch.Start();
#endif
			// See if there are vessels that need to be removed from our live
			// list
			foreach (KeyValuePair<Vessel, VesselFlare> v in vesselFlares)
			{
				if (v.Key.orbit.referenceBody != FlightGlobals.ActiveVessel.orbit.referenceBody || v.Key.loaded == true || !situations.Contains(v.Key.situation) || v.Value.referenceShip == null)
				{
					deadVessels.Add(v.Key);
				}
			}
#if SHOW_FIXEDUPDATE_TIMING
                long scanDead = stopwatch.ElapsedMilliseconds;
#endif

			for (int v = 0;v < deadVessels.Count;++v)
			{
				RemoveVesselFlare(deadVessels[v]);
			}
			deadVessels.Clear();
#if SHOW_FIXEDUPDATE_TIMING
                long clearDead = stopwatch.ElapsedMilliseconds;
#endif

			// See which vessels we should add
			for (int i = 0;i < FlightGlobals.Vessels.Count;++i)
			{
				Vessel vessel = FlightGlobals.Vessels[i];
				if (vessel.orbit.referenceBody == FlightGlobals.ActiveVessel.orbit.referenceBody && !vesselFlares.ContainsKey(vessel) && RenderableVesselType(vessel.vesselType) && !vessel.loaded && situations.Contains(vessel.situation))
				{
					AddVesselFlare(vessel);
				}
			}
#if SHOW_FIXEDUPDATE_TIMING
                long addNew = stopwatch.ElapsedMilliseconds;
                stopwatch.Stop();

                Log.force("GenerateVesselFlares net ms: scanDead = {0}, clearDead = {1}, addNew = {2} - {3} flares tracked", scanDead, clearDead, addNew, vesselFlares.Count));
#endif
		}

		//--------------------------------------------------------------------
		// CheckDraw
		// Checks if the given mesh should be drawn.
		private void CheckDraw(BodyFlare bodyFlre) => this.CheckDraw(bodyFlre, bodyFlre.body.transform.position, bodyFlre.body.referenceBody, FlareType.Celestial);
		private void CheckDraw(VesselFlare vesselFlare) => this.CheckDraw(vesselFlare, vesselFlare.mesh.transform.position, vesselFlare.referenceShip.mainBody, (vesselFlare.referenceShip.vesselType == VesselType.Debris) ? FlareType.Debris : FlareType.Vessel);
		private void CheckDraw(Flare flare, Vector3d position, CelestialBody referenceBody, FlareType flareType)
		{
			Vector3d targetVectorToSun = FlightGlobals.Bodies[0].position - position;
			Vector3d targetVectorToRef = referenceBody.position - position;
			double targetRelAngle = Vector3d.Angle(targetVectorToSun, targetVectorToRef);
			double targetDist = Vector3d.Distance(position, camPos);
			double targetSize = FlareType.Celestial == flareType
						? flare.sizeInDegrees
						: Math.Atan2(flare.sizeInDegrees, targetDist) * Mathf.Rad2Deg
					;

			double targetRefDist = Vector3d.Distance(position, referenceBody.position);
			double targetRefSize = Math.Acos(Math.Sqrt(Math.Pow(targetRefDist, 2.0) - Math.Pow(referenceBody.Radius, 2.0)) / targetRefDist) * Mathf.Rad2Deg;

			bool inShadow = false;
			if (referenceBody != FlightGlobals.Bodies[0] && targetRelAngle < targetRefSize)
			{
				inShadow = true;
			}

			bool isVisible;
			if (inShadow)
			{
				isVisible = false;
			}
			else
			{
				isVisible = true;

				// See if the sun obscures our target
				if (sunDistanceFromCamera < targetDist && sunSizeInDegrees > targetSize && Vector3d.Angle(cameraToSunUnitVector, position - camPos) < sunSizeInDegrees)
				{
					isVisible = false;
				}

				if (isVisible)
				{
					for (int i = 0;i < bodyFlares.Count;++i)
					{
						if (bodyFlares[i].body.bodyName != flare.mesh.name && bodyFlares[i].distanceFromCamera < targetDist && bodyFlares[i].sizeInDegrees > targetSize && Vector3d.Angle(bodyFlares[i].cameraToBodyUnitVector, position - camPos) < bodyFlares[i].sizeInDegrees)
						{
							isVisible = false;
							break;
						}
					}
				}
			}

			if (targetSize < (camFOV / 500.0f) && isVisible)
			{
				// Work in HSL space.  That allows us to do dimming of color
				// by adjusting the lightness value without any hue shifting.
				// We apply atmospheric dimming using alpha.  Although maybe
				// I don't need to - it could be done by dimming, too.
				float alpha = flare.hslColor.w;
				float dimming = 1.0f;
				alpha *= atmosphereFactor;
				dimming *= dimFactor;
				if (targetSize > (camFOV / 1000.0f))
				{
					dimming *= (float)(((camFOV / targetSize) / 500.0) - 1.0);
				}
				if (flareType == FlareType.Debris && DistantObjectSettings.DistantFlare.debrisBrightness < 1.0f)
				{
					dimming *= DistantObjectSettings.DistantFlare.debrisBrightness;
				}
				// Uncomment this to help with debugging
				//alpha = 1.0f;
				//dimming = 1.0f;
				flare.meshRenderer.material.color = ResourceUtilities.HSL2RGB(flare.hslColor.x, flare.hslColor.y, flare.hslColor.z * dimming, alpha);
				flare.mesh.SetActive(DistantObjectSettings.SkyboxBrightness.changeSkybox);
			}
			else
			{
				flare.mesh.SetActive(false);
			}
		}

		//--------------------------------------------------------------------
		// RenderableVesselType
		// Indicates whether the specified vessel type is one we will render
		private bool RenderableVesselType(VesselType vesselType)
		{
			return !(vesselType == VesselType.Flag || vesselType == VesselType.EVA || (vesselType == VesselType.Debris && DistantObjectSettings.DistantFlare.ignoreDebrisFlare));
		}

		//--------------------------------------------------------------------
		// UpdateVar()
		// Update atmosphereFactor and dimFactor
		private void UpdateVar()
		{
			Vector3d sunBodyAngle = (FlightGlobals.Bodies[0].position - camPos).normalized;
			double sunBodyDist = FlightGlobals.Bodies[0].GetAltitude(camPos) + FlightGlobals.Bodies[0].Radius;
			double sunBodySize = Math.Acos(Math.Sqrt(Math.Pow(sunBodyDist, 2.0) - Math.Pow(FlightGlobals.Bodies[0].Radius, 2.0)) / sunBodyDist) * Mathf.Rad2Deg;

			atmosphereFactor = 1.0f;

			if (FlightGlobals.currentMainBody != null && FlightGlobals.currentMainBody.atmosphere)
			{
				double camAltitude = FlightGlobals.currentMainBody.GetAltitude(camPos);
				double atmAltitude = FlightGlobals.currentMainBody.atmosphereDepth;
				double atmCurrentBrightness = (Vector3d.Distance(camPos, FlightGlobals.Bodies[0].position) - Vector3d.Distance(FlightGlobals.currentMainBody.position, FlightGlobals.Bodies[0].position)) / (FlightGlobals.currentMainBody.Radius);

				if (camAltitude > (atmAltitude / 2.0) || atmCurrentBrightness > 0.15)
				{
					atmosphereFactor = 1.0f;
				}
				else if (camAltitude < (atmAltitude / 10.0) && atmCurrentBrightness < 0.05)
				{
					atmosphereFactor = 0.0f;
				}
				else
				{
					if (camAltitude < (atmAltitude / 2.0) && camAltitude > (atmAltitude / 10.0) && atmCurrentBrightness < 0.15)
					{
						atmosphereFactor *= (float)((camAltitude - (atmAltitude / 10.0)) / (atmAltitude - (atmAltitude / 10.0)));
					}
					if (atmCurrentBrightness < 0.15 && atmCurrentBrightness > 0.05 && camAltitude < (atmAltitude / 2.0))
					{
						atmosphereFactor *= (float)((atmCurrentBrightness - 0.05) / (0.10));
					}
					if (atmosphereFactor > 1.0f)
					{
						atmosphereFactor = 1.0f;
					}
				}
				// atmDensityASL isn't an exact match for atmosphereMultiplier from KSP 0.90, I think, but it
				// provides a '1' for Kerbin (1.2, actually)
				float atmThickness = (float)Math.Min(Math.Sqrt(FlightGlobals.currentMainBody.atmDensityASL), 1);
				atmosphereFactor = (atmThickness) * (atmosphereFactor) + (1.0f - atmThickness);
			}

			float sunDimFactor = 1.0f;
			float skyboxDimFactor;
			if (DistantObjectSettings.SkyboxBrightness.changeSkybox)
			{
				// Apply fudge factors here so people who turn off the skybox don't turn off the flares, too.
				// And avoid a divide-by-zero.
				skyboxDimFactor = Mathf.Max(0.5f, GalaxyCubeControl.Instance.maxGalaxyColor.r / Mathf.Max(0.0078125f, DistantObjectSettings.SkyboxBrightness.maxBrightness));
			}
			else
			{
				skyboxDimFactor = 1.0f;
			}

			// This code applies a fudge factor to flare dimming based on the
			// angle between the camera and the sun.  We need to do this because
			// KSP's sun dimming effect is not applied to maxGalaxyColor, so we
			// really don't know how much dimming is being done.
			float angCamToSun = Vector3.Angle(FlightCamera.fetch.mainCamera.transform.forward, sunBodyAngle);
			if (angCamToSun < (camFOV * 0.5f))
			{
				bool isVisible = true;
				for (int i = 0;i < bodyFlares.Count;++i)
				{
					if (bodyFlares[i].distanceFromCamera < sunBodyDist && bodyFlares[i].sizeInDegrees > sunBodySize && Vector3d.Angle(bodyFlares[i].cameraToBodyUnitVector, FlightGlobals.Bodies[0].position - camPos) < bodyFlares[i].sizeInDegrees)
					{
						isVisible = false;
						break;
					}
				}
				if (isVisible)
				{
					// Apply an arbitrary minimum value - the (x^4) function
					// isn't right, but it does okay on its own.
					float sunDimming = Mathf.Max(0.2f, Mathf.Pow(angCamToSun / (camFOV * 0.5f), 4.0f));
					sunDimFactor *= sunDimming;
				}
			}
			dimFactor = DistantObjectSettings.DistantFlare.flareBrightness * Mathf.Min(skyboxDimFactor, sunDimFactor);
		}

		//--------------------------------------------------------------------
		// UpdateNameShown
		// Update the mousever name (if applicable)
		private void UpdateNameShown()
		{
			if (!DistantObjectSettings.DistantFlare.showNames) return;

			showNameTransform = null;
			{
				Ray mouseRay = FlightCamera.fetch.mainCamera.ScreenPointToRay(Input.mousePosition);

				// Detect CelestialBody mouseovers
				double bestRadius = -1.0;
				foreach (BodyFlare bodyFlare in bodyFlares) if (bodyFlare.body != FlightGlobals.ActiveVessel.mainBody)
				{
					if (bodyFlare.meshRenderer.material.color.a > 0.0f)
					{
						Vector3d vectorToBody = bodyFlare.body.position - mouseRay.origin;
						double mouseBodyAngle = Vector3d.Angle(vectorToBody, mouseRay.direction);
						if (mouseBodyAngle < 1.0)
							bestRadius = this.PrepareName(bodyFlare, bestRadius);
					}
				}

				if (showNameTransform == null)
				{
					// Detect Vessel mouseovers
					float bestBrightness = 0.01f; // min luminosity to show vessel name
					foreach (VesselFlare vesselFlare in vesselFlares.Values)
					{
						if (vesselFlare.mesh.activeSelf && vesselFlare.meshRenderer.material.color.a > 0.0f)
						{
							Vector3d vectorToVessel = vesselFlare.referenceShip.transform.position - mouseRay.origin;
							double mouseVesselAngle = Vector3d.Angle(vectorToVessel, mouseRay.direction);
							if (mouseVesselAngle < 1.0)
								bestBrightness = this.PrepareName(vesselFlare, bestBrightness);
						}
					}
				}
			}
		}

		private double PrepareName(BodyFlare bodyFlare, double bestRadius)
		{
			if (!bodyFlare.IsVisibleFrom(FlightCamera.fetch.mainCamera)) return bestRadius;
			if (bodyFlare.body.Radius > bestRadius)
			{
				double distance = Vector3d.Distance(FlightCamera.fetch.mainCamera.transform.position, bodyFlare.body.position);
				double angularSize = Mathf.Rad2Deg * bodyFlare.body.Radius / distance;
				if (angularSize < 0.2)
				{
					showNameTransform = bodyFlare.body.transform;
					showNameString = KSP.Localization.Localizer.Format("<<1>>", bodyFlare.body.bodyDisplayName);
					showNameColor = bodyFlare.color;
					return bodyFlare.body.Radius;
				}
			}
			return bestRadius;
		}

		private void PrepareName(BodyFlare bodyFlare)
		{
			if (!bodyFlare.IsOnFieldOfViewOf(FlightCamera.fetch.mainCamera)) return;
			showNameTransform = bodyFlare.body.transform;
			showNameString = KSP.Localization.Localizer.Format("<<1>>", bodyFlare.body.bodyDisplayName);
			showNameColor = bodyFlare.color;
		}

		private float PrepareName(VesselFlare vesselFlare, float bestBrightness)
		{
			if (!vesselFlare.IsVisibleFrom(FlightCamera.fetch.mainCamera)) return bestBrightness;
			float brightness = vesselFlare.brightness;
			if (brightness > bestBrightness)
			{
				showNameTransform = vesselFlare.referenceShip.transform;
				showNameString = vesselFlare.referenceShip.vesselName;
				showNameColor = Color.white;
				return brightness;
			}
			return bestBrightness;
		}

		private void PrepareName(VesselFlare vesselFlare)
		{
			if (!vesselFlare.IsOnFieldOfViewOf(FlightCamera.fetch.mainCamera)) return;
			showNameTransform = vesselFlare.referenceShip.transform;
			showNameString = vesselFlare.referenceShip.vesselName;
			showNameColor = Color.white;
		}

		//--------------------------------------------------------------------
		// Awake()
		// Load configs, set up the callback, 
		[UsedImplicitly]
		private void Awake()
		{
			INSTANCE = this;

			// DistantObject/Flare/model has extents of (0.5, 0.5, 0.0), a 1/2 meter wide square.
			this.flare = GameDatabase.Instance.GetModel(MODEL);
			Log.assert(() => null != this.flare, "Flare model {0} not found", MODEL);

			DistantObjectSettings.LoadConfig();

			Dictionary<string, Vessel.Situations> namedSituations = new Dictionary<string, Vessel.Situations> {
				{ Vessel.Situations.LANDED.ToString(), Vessel.Situations.LANDED},
				{ Vessel.Situations.SPLASHED.ToString(), Vessel.Situations.SPLASHED},
				{ Vessel.Situations.PRELAUNCH.ToString(), Vessel.Situations.PRELAUNCH},
				{ Vessel.Situations.FLYING.ToString(), Vessel.Situations.FLYING},
				{ Vessel.Situations.SUB_ORBITAL.ToString(), Vessel.Situations.SUB_ORBITAL},
				{ Vessel.Situations.ORBITING.ToString(), Vessel.Situations.ORBITING},
				{ Vessel.Situations.ESCAPING.ToString(), Vessel.Situations.ESCAPING},
				{ Vessel.Situations.DOCKED.ToString(), Vessel.Situations.DOCKED},
			};

			string[] situationStrings = DistantObjectSettings.DistantFlare.situations.Split(',');

			foreach (string sit in situationStrings)
			{
				if (namedSituations.ContainsKey(sit))
				{
					situations.Add(namedSituations[sit]);
				}
				else
				{
					Log.warn("Unable to find situation '{0}' in my known situations atlas", sit);
				}
			}

			sunRadiusSquared = FlightGlobals.Bodies[0].Radius * FlightGlobals.Bodies[0].Radius;
			GenerateBodyFlares();

			// Remove Vessels from our dictionaries just before they are destroyed.
			// After they are destroyed they are == null and this confuses Dictionary.
			GameEvents.onVesselWillDestroy.Add(RemoveVesselFlare);
		}

		[UsedImplicitly]
		private void Start()
		{
			DistantObjectSettings.Commit();
		}

		//--------------------------------------------------------------------
		// OnDestroy()
		// Clean up after ourselves.
		[UsedImplicitly]
		private void OnDestroy()
		{
			this.flare.DestroyGameObject(); this.flare = null;
			GameEvents.onVesselWillDestroy.Remove(RemoveVesselFlare);
			foreach (VesselFlare v in vesselFlares.Values)
				v.Destroy();
			vesselFlares.Clear();

			foreach (BodyFlare b in bodyFlares)
				b.Destroy();
			bodyFlares.Clear();

			INSTANCE = null;
		}

		//--------------------------------------------------------------------
		// RemoveVesselFlare
		// Removes a flare (either because a vessel was destroyed, or it's no
		// longer supposed to be part of the draw list).
		private void RemoveVesselFlare(Vessel v)
		{
			if (vesselFlares.ContainsKey(v))
			{
				vesselFlares[v].Destroy();
				vesselFlares.Remove(v);
			}
		}

		//--------------------------------------------------------------------
		// FixedUpdate
		// Update visible vessel list
		[UsedImplicitly]
		private void FixedUpdate()
		{
			if (MapView.MapIsEnabled) return;

			if (bigHammer)
			{
				foreach (VesselFlare v in vesselFlares.Values)
					v.Destroy();
				vesselFlares.Clear();
				bigHammer = false;
			}

			// MOARdV TODO: Make this callback-based instead of polling
			GenerateVesselFlares();
		}

		//--------------------------------------------------------------------
		// Update
		// Update flare positions and visibility
		[UsedImplicitly]
		private void Update()
		{
			showNameTransform = null;
			if (DistantObjectSettings.DistantFlare.flaresEnabled)
			{
				if (MapView.MapIsEnabled)
				{
					// Big Hammer for map view - don't draw any flares
					foreach (BodyFlare flare in bodyFlares)
					{
						flare.mesh.SetActive(false);
					}

					foreach (VesselFlare vesselFlare in vesselFlares.Values)
					{
						vesselFlare.mesh.SetActive(false);
					}
				}
				else
				{
#if SHOW_FIXEDUPDATE_TIMING
                stopwatch.Reset();
                stopwatch.Start();
#endif
					camPos = FlightCamera.fetch.mainCamera.transform.position;

					Vector3d targetVectorToCam = camPos - FlightGlobals.Bodies[0].position;

					cameraToSunUnitVector = -targetVectorToCam.normalized;
					sunDistanceFromCamera = targetVectorToCam.magnitude;
					sunSizeInDegrees = Math.Acos(Math.Sqrt(sunDistanceFromCamera * sunDistanceFromCamera - sunRadiusSquared) / sunDistanceFromCamera) * Mathf.Rad2Deg;

					if (!ExternalControl)
					{
						camFOV = FlightCamera.fetch.mainCamera.fieldOfView;
					}

					// Log.dbg("Update"); Really bad idea...

					foreach (BodyFlare flare in bodyFlares)
					{
						flare.Update(camPos, camFOV);

						if (flare.mesh.activeSelf)
							this.CheckDraw(flare);
					}
#if SHOW_FIXEDUPDATE_TIMING
                    long bodyCheckdraw = stopwatch.ElapsedMilliseconds;
#endif

					UpdateVar();
#if SHOW_FIXEDUPDATE_TIMING
                    long updateVar = stopwatch.ElapsedMilliseconds;
#endif

					foreach (VesselFlare vesselFlare in vesselFlares.Values)
					{
						try
						{
							vesselFlare.Update(camPos, camFOV);

							if (vesselFlare.mesh.activeSelf)
								this.CheckDraw(vesselFlare);
						}
						catch
						{
							// Something went drastically wrong.
							bigHammer = true;
						}
					}
#if SHOW_FIXEDUPDATE_TIMING
                    long vesselCheckdraw = stopwatch.ElapsedMilliseconds;
#endif

					UpdateNameShown();
#if SHOW_FIXEDUPDATE_TIMING
                    long updateName = stopwatch.ElapsedMilliseconds;
                    stopwatch.Stop();

                    Log.force("Update net ms: bodyCheckdraw = {0}, updateVar = {1}, vesselCheckdraw = {2}, updateName = {3}", bodyCheckdraw, updateVar, vesselCheckdraw,updateName));
#endif
				}
			}
		}

		private GUIStyle flyoverTextStyle = new GUIStyle();
		private Rect flyoverTextPosition = new Rect(0.0f, 0.0f, 100.0f, 20.0f);

		//--------------------------------------------------------------------
		// OnGUI
		// Draws flare names when enabled
		[UsedImplicitly]
		private void OnGUI()
		{
			if (MapView.MapIsEnabled || !(DistantObjectSettings.DistantFlare.flaresEnabled && DistantObjectSettings.DistantFlare.showNames)) return;

			if (Input.GetMouseButton(1) && Event.current.modifiers == EventModifiers.Alt)
			{
				foreach (BodyFlare bodyFlare in bodyFlares) if (bodyFlare.body != FlightGlobals.ActiveVessel.mainBody)
					{
						this.showNameTransform = null;
						this.PrepareName(bodyFlare);
						if (null != this.showNameTransform) this.ShowNameTransformPosition();
					}

				foreach (VesselFlare vesselFlare in vesselFlares.Values) if (vesselFlare.referenceShip != FlightGlobals.ActiveVessel)
					{
						this.showNameTransform = null;
						this.PrepareName(vesselFlare);
						if (null != this.showNameTransform) this.ShowNameTransformPosition();
					}

				this.showNameTransform = null;
			}
			else if (null != showNameTransform)
				this.ShowNameTransformPosition();
		}

		private void ShowNameTransformPosition()
		{
			Vector3 screenPos = FlightCamera.fetch.mainCamera.WorldToScreenPoint(showNameTransform.position);
			flyoverTextPosition.x = screenPos.x;
			flyoverTextPosition.y = Screen.height - screenPos.y - 20.0f;
			flyoverTextStyle.normal.textColor = showNameColor;
			GUI.Label(flyoverTextPosition, showNameString, flyoverTextStyle);
		}

		//--------------------------------------------------------------------
		// SetFOV
		// Provides an external plugin the opportunity to set the FoV.
		public static void SetFOV(float FOV)
		{
			if (ExternalControl)
			{
				camFOV = FOV;
			}
		}

		//--------------------------------------------------------------------
		// SetExternalFOVControl
		// Used to indicate whether an external plugin wants to control the
		// field of view.
		public static void SetExternalFOVControl(bool Control)
		{
			ExternalControl = Control;
		}

		internal void SetActiveTo(bool changeSkybox)
		{
			if (changeSkybox)
				this.Activate();
			else
				this.Deactivate();
		}

		private void Activate()
		{
			Log.trace("FlareDraw enabled");
			this.enabled = true;
		}

		private void Deactivate()
		{
			Log.trace("FlareDraw disabled");
			this.enabled = false;

			foreach (VesselFlare v in this.vesselFlares.Values)
				v.Destroy();
			this.vesselFlares.Clear();

			for (int i = 0;i < bodyFlares.Count;++i)
				bodyFlares[i].mesh.SetActive(false);
		}
	}
}
