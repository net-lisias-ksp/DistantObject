/*
		This file is part of Distant Object Enhancement /L
			© 2021 LisiasT
			© 2019-2021 TheDarkBadger
			© 2014-2019 MOARdV
			© 2014 Rubber Ducky
*/

using UnityEngine;

namespace DistantObject
{
    public partial class SettingsGui : MonoBehaviour
    {
        public static IButton buttonDOSettings = null;

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
