using System;

using ICities;
using ColossalFramework.UI;
using UnityEngine;

namespace BetterBudget
{
    // Author: unobtanium
    public class BBModLoader : LoadingExtensionBase
    {
        private LoadMode _mode;
        private BetterBudget2 _betterBudgetPanel;

        /// <summary>
        /// Loads the mod if in gameplay mode and not in editor.
        /// </summary>
        /// <param name="mode"></param>


        public override void OnLevelLoaded(LoadMode mode)
        {
            _mode = mode;
            // only loads up in gameplay mode
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            // ask for UI information
            UIView view = UIView.GetAView();

            // create container to hold the mod objects
            GameObject goBetterBudget = new GameObject("BetterBudgetMod");
            _betterBudgetPanel = goBetterBudget.AddComponent<BetterBudget2>();
        }

        /// <summary>
        /// Unloads the mod on game exit.
        /// </summary>
        public override void OnLevelUnloading()
        {
            // only unloads in gameplay mode
            if ((_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame))
                return;

            if (_betterBudgetPanel != null)
            {
                _betterBudgetPanel.unload();
                GameObject.Destroy(_betterBudgetPanel.gameObject);
            }
        }
    }
}
