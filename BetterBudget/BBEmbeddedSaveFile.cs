using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterBudget
{
    public class BBEmbeddedSaveFile
    {
        public String infoViewPanelName;
        public List<String> budgetSliderNameList;

        public BBEmbeddedSaveFile()
        {
            infoViewPanelName = "";
            budgetSliderNameList = new List<String>();
        }
    }
}
