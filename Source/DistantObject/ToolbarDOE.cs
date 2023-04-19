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

namespace DistantObject
{
    partial class SettingsGui : MonoBehaviour
    {
        private static IButton buttonDOSettings = null;

        private void toolbarButton()
        {
            Log.dbg("Drawing toolbar icon...");
            buttonDOSettings = ToolbarManager.Instance.add("test", "buttonDOSettings");
            buttonDOSettings.Visibility = new GameScenesVisibility(GameScenes.SPACECENTER, GameScenes.FLIGHT);
            if (activated)
            {
                buttonDOSettings.TexturePath = "DistantObject/Icons/toolbar_enabled";
            }
            else
            {
                buttonDOSettings.TexturePath = "DistantObject/Icons/toolbar_disabled";
            }
            buttonDOSettings.ToolTip = "Distant Object Enhancement Settings";
            buttonDOSettings.OnClick += (e) => Toggle();
            buttonDOSettings.OnClick += (e) => ToggleIcon();
        }

        private void ToggleIcon()
        {
            if (buttonDOSettings != null)
            {
                if (activated)
                {
                    buttonDOSettings.TexturePath = "DistantObject/Icons/toolbar_enabled";
                }
                else
                {
                    buttonDOSettings.TexturePath = "DistantObject/Icons/toolbar_disabled";
                }
            }

            if (appLauncherButton != null)
            {
                if (activated)
                {
                    Texture2D iconTexture = null;
                    if (GameDatabase.Instance.ExistsTexture("DistantObject/Icons/toolbar_enabled_38"))
                    {
                        iconTexture = GameDatabase.Instance.GetTexture("DistantObject/Icons/toolbar_enabled_38", false);
                    }
                    if (iconTexture != null)
                    {
                        appLauncherButton.SetTexture(iconTexture);
                    }
                }
                else
                {
                    Texture2D iconTexture = null;
                    if (GameDatabase.Instance.ExistsTexture("DistantObject/Icons/toolbar_disabled_38"))
                    {
                        iconTexture = GameDatabase.Instance.GetTexture("DistantObject/Icons/toolbar_disabled_38", false);
                    }
                    if (iconTexture != null)
                    {
                        appLauncherButton.SetTexture(iconTexture);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            Log.trace("OnDestroy - {0}", this.GetInstanceID());

            if (buttonDOSettings != null)
            {
                buttonDOSettings.Destroy();
                buttonDOSettings = null;
            }
            GameEvents.onGUIApplicationLauncherReady.Remove(AddAppLauncherButton);
            GameEvents.onGUIApplicationLauncherDestroyed.Remove(RemoveAppLauncherButton);
            RemoveAppLauncherButton();
        }
    }
}
