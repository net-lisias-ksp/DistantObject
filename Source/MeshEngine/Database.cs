/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
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

namespace DistantObject.MeshEngine
{
	public static class Database
	{
		internal static class PartModelDB
		{ 
			private static readonly Dictionary<string, List<string>> DB = new Dictionary<string, List<string>>();

			internal static void Add(string partName, string modelName)
			{
				if (!DB.ContainsKey(partName)) DB.Add(partName, new List<string>());
				DB[partName].Add(modelName);
			}

			internal static bool ContainsKey(string partName)
			{
				return DB.ContainsKey(partName);
			}

			private static readonly List<string> EMPTY = new List<string>();
			internal static IEnumerable<string> Get(string partName)
			{
				if (DB.ContainsKey(partName)) return DB[partName];
				return EMPTY;
			}

			internal static void Remove(List<string> dejects)
			{
				if (0 == dejects.Count) return;
				foreach (string s in dejects)
					DB.Remove(s);
			}
		}

		internal static void Init()
		{
			bool sawErrors = false;
			foreach (UrlDir.UrlConfig urlConfig in GameDatabase.Instance.GetConfigs("PART"))
			{
				ConfigNode cfgNode = urlConfig.config;
				if (cfgNode.HasValue("name"))
				{
					string partName = cfgNode.GetValue("name");

					// There's no point on tryint to render the Prebuilt parts. Their meshes are not available.
					if (partName.StartsWith("kerbalEVA")) continue;
					if (partName == "flag") continue;

					string url = urlConfig.parent.url.Substring(0, urlConfig.parent.url.LastIndexOf("/"));
					if (cfgNode.HasValue("mesh"))
					{
						string modelName = cfgNode.GetValue("mesh");
						modelName = System.IO.Path.GetFileNameWithoutExtension(modelName);
						sawErrors = AddModelToPart(partName, url + "/" + modelName);
					}
					else if (cfgNode.HasNode("MODEL"))
					{
						ConfigNode[] cna = cfgNode.GetNodes("MODEL");
						foreach (ConfigNode cn in cna)
						{ 
							string modelName = cn?.GetValue("model");
							sawErrors = AddModelToPart(partName, modelName);
						}
					}
					else
					{
						Log.error("Could not find a model for part {0}.  Part will not render for VesselDraw.", partName);
						sawErrors = true;
					}
				}
				else
				{
					Log.error("Could not find ConfigNode for part {0}.  Part will not render for VesselDraw.", urlConfig.name);
					sawErrors = true;
				}
			}

			Log.dbg("VesselDraw initialized");
			if (sawErrors) Log.error("Some parts do not have ConfigNode entries in the game database.  Some distant vessels will be missing pieces.");
		}

		private static bool AddModelToPart(string partName, string modelPath)
		{	// TODO: Find the right place to initialise this thing, so we don't need to check sanityon the drawing phase!
			//
			// The problem is that by callig this on some Awake or Start a lot of parts that would work on the
			// MeshEngine Update bork on the spot - and I just don't understand why.
			//
			// I need to find why **some** GameDatabase.Instance.GetModel borks on Awake/Start and works on Update!

			//if (null != GameDatabase.Instance.GetModel(modelPath))
			//{ 
				Log.trace("Addint {0} {1}", partName, modelPath);
				PartModelDB.Add(partName, modelPath);
				return false;
			//}
			//Log.error("Could not find the mesh for the model {0} from part {1}.  Part will not render for VesselDraw.", modelPath, partName);
			//return true;
		}
	}
}
