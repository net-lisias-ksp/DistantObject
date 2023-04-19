/*
		This file is part of Distant Object Enhancement /L
			© 2021-2023 LisiasT
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

namespace DistantObject.MeshEngine.Stock
{
	public class BlackList:DistantObject.MeshEngine.Contract.Module.IBlackList
	{
		private static readonly string[] BLACKLIST = { "LaunchClamp", "CModuleLinkedMesh" };

		public BlackList()
		{
		}

		List<string> Contract.Module.IBlackList.Get()
		{
			return new List<string>(BLACKLIST);
		}
	}
}
