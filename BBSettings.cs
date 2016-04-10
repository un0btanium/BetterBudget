using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterBudget
{
    /// <summary>
    /// Contains all mod settings to load them next session.
    /// </summary>
    public class BBSettings
    {
        public List<BBEmbeddedSaveFile> embeddedPanelSettings;
        public List<BBCustomSaveFile> customPanelSettings;
        public bool expanseUpdateActive;

        public BBSettings()
        {
            embeddedPanelSettings = new List<BBEmbeddedSaveFile>();
            customPanelSettings = new List<BBCustomSaveFile>();
            expanseUpdateActive = true;
        }
        /*
        public List<BBPanelSettings> panelSettings = new List<BBPanelSettings>();
        public bool expenseUpdateActive = true;
        public BBPanelSettings customPanelCreatorSettings;
         */
    }
}
