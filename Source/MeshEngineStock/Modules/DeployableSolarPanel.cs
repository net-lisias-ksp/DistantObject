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
using UnityEngine;

namespace DistantObject.MeshEngine.Stock.Module
{
	public class DeployableSolarPanel : DistantObject.MeshEngine.Contract.Module.Interface
	{
		private const string MODULE_NAME = "ModuleDeployableSolarPanel";
		public DeployableSolarPanel()
		{
		}

		string Contract.Module.Interface.GetImplementedModuleName()
		{
			return MODULE_NAME;
		}

		GameObject Contract.Module.Interface.Render(GameObject mesh, ProtoPartSnapshot part, AvailablePart avPart)
		{
			ProtoPartModuleSnapshot solarPanel = part.modules.Find(n => n.moduleName == MODULE_NAME);
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
					Animation anim = mesh.GetComponentInChildren<Animation>();
					//copy the animation over to the new part!
					anim.AddClip(animClip, animName);
					anim[animName].enabled = true;
					anim[animName].normalizedTime = 1f;
				}
			}
			return mesh;
		}
	}
}