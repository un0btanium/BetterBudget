using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICities;
using ColossalFramework.UI;
using UnityEngine;

namespace BetterBudget
{
    /// <summary>
    /// Used to save data of an extended panel. Used on mod load to create the extended panel.
    /// </summary>
    public class BBPanelSettings
    {
        public String name { get; set; }
        public String[] slider { get; set; }
        public String informationPanel { get; set; }

        public float x { get; set; }
        public float y { get; set; }
        public float opacity { get; set; }
        public bool sticky { get; set; }
        public bool slim { get; set; }
        public bool isCustom { get; set; }
    }
}
