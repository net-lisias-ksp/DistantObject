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

namespace DistantObject.Debug
{
	internal static class DarkenSky
	{
		private static readonly Dummy DUMMY = new Dummy();
		private static Interface __instance = null;
		internal static Interface Instance { get => __instance??DUMMY; private set => __instance = value; }

		internal interface Interface
		{
			double targetColorScalar { get; set; }
			bool Show { get; set; }
		}

		private class Dummy:Interface
		{
			double Interface.targetColorScalar { get => this.targetColorScalar; set => this.targetColorScalar = value; }
			bool Interface.Show { get => false; set { bool r = value; } }

			private double targetColorScalar = 0;
		}

		[KSPAddon(KSPAddon.Startup.Flight, false)]
		internal class Implementation:MonoBehaviour, Interface
		{
			public static readonly float WIDTH = 250;
			public static readonly float HEIGHT = 100;

			private readonly int WindowID;
			private Rect windowPos;

			double Interface.targetColorScalar { get => this.targetColorScalar; set => this.targetColorScalar = value; }
			bool Interface.Show {
				get => this.showWindow;
				set
				{
					this.enabled = this.showWindow = value;
				}
			}

			private double targetColorScalar = 0;
			private bool showWindow = false;

			private Implementation() : base()
			{
				this.WindowID = KSPe.UI.UID.Get();
				this.windowPos = new Rect(0, 0, WIDTH, HEIGHT);
			}

			~Implementation()
			{
				KSPe.UI.UID.Release(this.WindowID);
			}

			private void Awake()
			{
				Log.dbg("DistantObject.Debug.DarkenSky.Awake()");
				Instance = this;
				this.enabled = this.showWindow = this.enabled = Globals.DEBUG;
			}

			private void OnDestroy()
			{
				Log.dbg("DistantObject.Debug.DarkenSky.OnDestroy()");
				this.enabled = Globals.DEBUG;
				Instance = null;
			}

			private void OnGUI()
			{
				if (!this.showWindow) return;
				this.windowPos = GUILayout.Window(this.WindowID, this.windowPos, mainGUI, Globals.DistantObject + " Debug", GUILayout.Width(WIDTH), GUILayout.Height(HEIGHT));
			}

			private void mainGUI(int windowID)
			{
				GUILayout.BeginVertical("Skybox Dimming", new GUIStyle(GUI.skin.window));
				{
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					GUILayout.Label("targetColorScalar");
					GUILayout.Label(string.Format("{0:0.000000}", targetColorScalar));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUI.DragWindow();
			}
		}
	}
}
