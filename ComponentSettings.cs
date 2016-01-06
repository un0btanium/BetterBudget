using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterBudget
{
    /// <summary>
    /// Contains all required data to position all objects respective to the panels mode (Default, Slim, noSlider) and orientation (left or right)
    /// </summary>
    class ComponentSettings
    {
        public int panelWidth;
        public int[] autolayoutpadding;

        public int sliderX;
        public bool sliderIsVisible;

        public int totalX;
        public int totalY;
        public int totalWidth;
        public int totalHeight;
        public bool totalIsVisible;

        public int percentageDayX;
        public int percentageDayY;
        public int percentageNightX;
        public int percentageNightY;
        public int percentageWidth;
        public int percentageHeight;
        public bool percentageIsVisible;

        public int iconX;

        public int buttonPlusDayX;
        public int buttonPlusDayY;
        public int buttonPlusNightX;
        public int buttonPlusNightY;
        public bool buttonPlusIsVisible;

        public int buttonMinusDayX;
        public int buttonMinusDayY;
        public int buttonMinusNightX;
        public int buttonMinusNightY;
        public bool buttonMinusIsVisible;

        /// <summary>
        /// Get all required data to position all objects respective to the panels mode (Default, Slim, noSlider) and orientation (left or right)
        /// </summary>
        /// <param name="mode">The mode the panel currently is in.</param>
        /// <param name="isLeft">The orientation the panel currently is in.</param>
        /// <returns>Returns itself with the updated data.</returns>
        public ComponentSettings getComponentSettings(Mode mode, bool isLeft)
        {
            if (mode == Mode.Default)
            {
                if (isLeft)
                {
                    panelWidth = 355;
                    autolayoutpadding = new int[] { 14, 0, 0, 5 };

                    sliderX = 100;
                    sliderIsVisible = true;

                    totalX = 10;
                    totalY = 13;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageDayX = 10;
                    percentageDayY = 13;
                    percentageNightX = 51;
                    percentageNightY = 13;
                    percentageWidth = 39;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 315;

                    buttonPlusDayX = 155;
                    buttonPlusDayY = 13;
                    buttonPlusNightX = 155;
                    buttonPlusNightY = 13;
                    buttonPlusIsVisible = false;

                    buttonMinusDayX = 45;
                    buttonMinusDayY = 13;
                    buttonMinusNightX = 45;
                    buttonMinusNightY = 13;
                    buttonMinusIsVisible = false;
                }
                else
                {
                    panelWidth = 355;
                    autolayoutpadding = new int[] { 14, 0, 0, 5 };

                    sliderX = 45;
                    sliderIsVisible = true;

                    totalX = 265;
                    totalY = 13;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageDayX = 265;
                    percentageDayY = 13;
                    percentageNightX = 306;
                    percentageNightY = 13;
                    percentageWidth = 39;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 3;

                    buttonPlusDayX = 155;
                    buttonPlusDayY = 13;
                    buttonPlusNightX = 155;
                    buttonPlusNightY = 13;
                    buttonPlusIsVisible = false;

                    buttonMinusDayX = 45;
                    buttonMinusDayY = 13;
                    buttonMinusNightX = 45;
                    buttonMinusNightY = 13;
                    buttonMinusIsVisible = false;
                }
                
            }
            else if (mode == Mode.Slim)
            {
                if (isLeft)
                {
                    panelWidth = 130;
                    autolayoutpadding = new int[] { 0, 0, 0, 0 };

                    sliderX = 100;
                    sliderIsVisible = false;

                    totalX = 6;
                    totalY = 13;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageDayX = 6;
                    percentageDayY = 13;
                    percentageNightX = 47;
                    percentageNightY = 13;
                    percentageWidth = 39;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 90;

                    buttonPlusDayX = 155;
                    buttonPlusDayY = 13;
                    buttonPlusNightX = 155;
                    buttonPlusNightY = 13;
                    buttonPlusIsVisible = false;

                    buttonMinusDayX = 45;
                    buttonMinusDayY = 13;
                    buttonMinusNightX = 45;
                    buttonMinusNightY = 13;
                    buttonMinusIsVisible = false;
                }
                else
                {
                    panelWidth = 130;
                    autolayoutpadding = new int[] { 0, 0, 0, 0 };

                    sliderX = 45;
                    sliderIsVisible = false;

                    totalX = 45;
                    totalY = 13;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageDayX = 45;
                    percentageDayY = 13;
                    percentageNightX = 86;
                    percentageNightY = 13;
                    percentageWidth = 39;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 3;

                    buttonPlusDayX = 155;
                    buttonPlusDayY = 13;
                    buttonPlusNightX = 155;
                    buttonPlusNightY = 13;
                    buttonPlusIsVisible = false;

                    buttonMinusDayX = 45;
                    buttonMinusDayY = 13;
                    buttonMinusNightX = 45;
                    buttonMinusNightY = 13;
                    buttonMinusIsVisible = false;
                }
            }
            else if (mode == Mode.noSlider)
            {
                if (isLeft)
                {
                    panelWidth = 130;
                    autolayoutpadding = new int[] { 0, 0, 0, 0 };

                    sliderX = 45;
                    sliderIsVisible = false;

                    totalX = 6;
                    totalY = 13;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageDayX = 6;
                    percentageDayY = 5;
                    percentageNightX = 6; // TDOO: new different layout 
                    percentageNightY = 23;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 90;

                    buttonPlusDayX = 6;
                    buttonPlusDayY = 5;
                    buttonPlusNightX = 6; // TDOO: new different layout 
                    buttonPlusNightY = 23;
                    buttonPlusIsVisible = false;
                    
                    buttonMinusDayX = 69;
                    buttonMinusDayY = 5;
                    buttonMinusNightX = 69; // TDOO: new different layout 
                    buttonMinusNightY = 23;
                    buttonMinusIsVisible = false;
                }
                else
                {
                    panelWidth = 130;
                    autolayoutpadding = new int[] { 0, 0, 0, 0 };

                    sliderX = 45;
                    sliderIsVisible = false;

                    totalX = 45;
                    totalY = 13;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageDayX = 45;
                    percentageDayY = 5;
                    percentageNightX = 45; // TDOO: new different layout
                    percentageNightY = 23;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 3;

                    buttonPlusDayX = 107;
                    buttonPlusDayY = 5;
                    buttonPlusNightX = 107; // TDOO: new different layout
                    buttonPlusNightY = 23;
                    buttonPlusIsVisible = false;

                    buttonMinusDayX = 45;
                    buttonMinusDayY = 5;
                    buttonMinusNightX = 45; // TDOO: new different layout
                    buttonMinusNightY = 23;
                    buttonMinusIsVisible = false;
                }
            }
            return this;
        }
    }
}
