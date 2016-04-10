using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetterBudget
{
    public class BBCustomSaveFile
    {
        public List<String> budgetSliderNameList;
        public float x;
        public float y;
        public float width;
        public Mode mode;
        public float opacity;

        public BBCustomSaveFile()
        {
            budgetSliderNameList = new List<String>();
            x = 500;
            y = 500;
            width = 350;
            mode = Mode.Default;
            opacity = 1f;
        }
    }
}
