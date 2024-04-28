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
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DistantObject.MeshEngine.Contract
{
	public static class Module
	{
		public interface IBlackList
		{
			List<string> Get();
		}

		public interface Interface
		{
			string GetImplementedModuleName();
			GameObject Render(GameObject go, ProtoPartSnapshot part, AvailablePart avPart);
		}

		private static readonly HashSet<string> BLACKLIST = new HashSet<string>();
		private static readonly Dictionary<string, Interface> MAP = new Dictionary<string, Interface>();
		internal static void Init()
		{
			foreach (Type type in KSPe.Util.SystemTools.Type.Search.By(typeof(DistantObject.MeshEngine.Contract.Module.IBlackList)))
			{
				ConstructorInfo ctor = type.GetConstructor(new Type[] { });
				IBlackList i = (IBlackList) ctor.Invoke(new object[] { });
				foreach (string s in i.Get())
					BLACKLIST.Add(s);
			}

			foreach (Type type in KSPe.Util.SystemTools.Type.Search.By(typeof(DistantObject.MeshEngine.Contract.Module.Interface)))
			{
				ConstructorInfo ctor = type.GetConstructor(new Type[] { });
				Interface i = (Interface) ctor.Invoke(new object[] { });
				MAP.Add(i.GetImplementedModuleName(), i);
			}
		}

		internal static bool IsBlackListed(ProtoPartSnapshot part)
		{
			foreach (string moduleName in BLACKLIST)
				if (part.modules.Find(n => n.moduleName == moduleName) != null)
					return true;
			return false;
		}

		internal static GameObject Render(GameObject mesh, ProtoPartSnapshot part, AvailablePart avPart, ProtoPartModuleSnapshot module)
		{
			if (!MAP.ContainsKey(module.moduleName)) return mesh;
			return MAP[module.moduleName].Render(mesh, part, avPart);
		}
	}
}
