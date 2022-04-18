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
using KSPe.Annotations;

namespace DistantObject.MeshEngine
{
	public class Startup
	{
		[UsedImplicitly]
		private void Awake()
		{
			using (KSPe.Util.SystemTools.Assembly.Loader a = new KSPe.Util.SystemTools.Assembly.Loader<DistantObject.Startup>())
			{
				a.LoadAndStartup("MeshEngineStock");
				a.LoadAndStartup("MeshEngineTweakScale");
			}

			Database.Init();
			Contract.Module.Init();
		}

		[UsedImplicitly]
		private void Start()
		{
			Log.force("MeshEngine {0} ready.", Version.Text);
		}
	}
}
