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

        public int percentageX;
        public int percentageY;
        public int percentageWidth;
        public int percentageHeight;
        public bool percentageIsVisible;

        public int iconX;

        public int buttonPlusX;
        public int buttonPlusY;
        public bool buttonPlusIsVisible;

        public int buttonMinusX;
        public int buttonMinusY;
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
                    totalY = 10;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageX = 10;
                    percentageY = 10;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 315;

                    buttonPlusX = 155;
                    buttonPlusY = 10;
                    buttonPlusIsVisible = false;

                    buttonMinusX = 45;
                    buttonMinusY = 10;
                    buttonMinusIsVisible = false;
                }
                else
                {
                    panelWidth = 355;
                    autolayoutpadding = new int[] { 14, 0, 0, 5 };

                    sliderX = 45;
                    sliderIsVisible = true;

                    totalX = 265;
                    totalY = 10;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageX = 265;
                    percentageY = 10;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 3;

                    buttonPlusX = 155;
                    buttonPlusY = 10;
                    buttonPlusIsVisible = false;

                    buttonMinusX = 45;
                    buttonMinusY = 10;
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
                    totalY = 10;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageX = 6;
                    percentageY = 10;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 90;

                    buttonPlusX = 155;
                    buttonPlusY = 10;
                    buttonPlusIsVisible = false;

                    buttonMinusX = 45;
                    buttonMinusY = 10;
                    buttonMinusIsVisible = false;
                }
                else
                {
                    panelWidth = 130;
                    autolayoutpadding = new int[] { 0, 0, 0, 0 };

                    sliderX = 45;
                    sliderIsVisible = false;

                    totalX = 45;
                    totalY = 10;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageX = 45;
                    percentageY = 10;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 3;

                    buttonPlusX = 155;
                    buttonPlusY = 10;
                    buttonPlusIsVisible = false;

                    buttonMinusX = 45;
                    buttonMinusY = 10;
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
                    totalY = 10;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageX = 6;
                    percentageY = 10;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 90;

                    buttonPlusX = 6;
                    buttonPlusY = 10;
                    buttonPlusIsVisible = false;

                    buttonMinusX = 69;
                    buttonMinusY = 10;
                    buttonMinusIsVisible = false;
                }
                else
                {
                    panelWidth = 130;
                    autolayoutpadding = new int[] { 0, 0, 0, 0 };

                    sliderX = 45;
                    sliderIsVisible = false;

                    totalX = 45;
                    totalY = 10;
                    totalWidth = 80;
                    totalHeight = 18;
                    totalIsVisible = true;

                    percentageX = 45;
                    percentageY = 10;
                    percentageWidth = 80;
                    percentageHeight = 18;
                    percentageIsVisible = false;

                    iconX = 3;

                    buttonPlusX = 107;
                    buttonPlusY = 10;
                    buttonPlusIsVisible = false;

                    buttonMinusX = 45;
                    buttonMinusY = 10;
                    buttonMinusIsVisible = false;
                }
            }
            return this;
        }
    }
}
