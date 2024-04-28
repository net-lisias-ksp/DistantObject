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
using KSPe.Annotations;
using UnityEngine;

namespace DistantObject
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class Startup:MonoBehaviour
	{
		[UsedImplicitly]
		private void Awake()
		{
			try
			{
				KSPe.Util.Compatibility.Check<Startup>();
				KSPe.Util.Installation.Check<Startup>();
				GameEvents.onGameSceneSwitchRequested.Add(OnGameSceneSwitchRequested);
			}
			catch (KSPe.Util.InstallmentException e)
			{
				Log.error(e.ToShortMessage());
				KSPe.Common.Dialogs.ShowStopperAlertBox.Show(e);
			}
		}

		[UsedImplicitly]
		private void Start()
		{
			Log.force("Version {0}", Version.Text);
		}

		private void OnGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> data)
		{
			GameEvents.onGameSceneSwitchRequested.Remove(OnGameSceneSwitchRequested);
			using (KSPe.Util.SystemTools.Assembly.Loader a = new KSPe.Util.SystemTools.Assembly.Loader<Startup>())
			{
				a.LoadAndStartup("MeshEngine");
			}
		}
	}
}
