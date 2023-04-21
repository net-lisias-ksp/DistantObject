﻿/*
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
using IO = KSPe.IO;

namespace DistantObject
{
	internal class Globals
	{
		static internal readonly string DistantObject = "Distant Object Enhancement v" + Version.Number;
		internal const string SETTINGS_NAME = "Settings";
		internal const string SETTINGS_FILE = SETTINGS_NAME + ".cfg";
		static internal readonly string SETTINGS_DEFAULTS = "DistantObject";
		static internal readonly string REFERENCE_CONFIG_PATHNAME = IO.Hierarchy<Startup>.GAMEDATA.Solve("PluginData", SETTINGS_FILE);
		static internal readonly string CONFIG_DIRECTORY = IO.Hierarchy<Startup>.PLUGINDATA.Solve();
		static internal readonly string CONFIG_PATHNAME = IO.Hierarchy<Startup>.PLUGINDATA.Solve(SETTINGS_FILE);
	}
}