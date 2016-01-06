using System;
using System.Collections.Generic;

using ICities;
using ColossalFramework.UI;
using ColossalFramework.DataBinding;
using UnityEngine;


namespace BetterBudget
{
    class BBSliderContainer : UIPanel
    {

        private List<UIPanel> sliders;
        private List<int> slimCloseTimer;
        private ComponentSettings cs;

        public UIExtendedBudgetPanel Parent { get; set; }

        /// <summary>
        /// Creates all objects. Call first.
        /// </summary>
        public void start()
        {
            sliders = new List<UIPanel>();
            slimCloseTimer = new List<int>();
            cs = new ComponentSettings();
            name = "Slider Container";
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(14, 0, 0, 5);
            autoFitChildrenHorizontally = true;
            autoFitChildrenVertically = true;
        }

        /// <summary>
        /// Toggles through all modes.
        /// </summary>
        /// <param name="component">Unused</param>
        /// <param name="eventParam">Unused</param>
        private void toggleMode(UIComponent component, UIMouseEventParameter eventParam)
        {
            Parent.toggleMode();
        }

        /// <summary>
        /// Sets position, size, ect. of all objects depending on the mode the extended panel is in.
        /// </summary>
        /// <param name="mode">The extended panel mode</param>
        public void setMode(Mode mode)
        {
            cs = cs.getComponentSettings(mode, Parent.isLeft);

            foreach (UIPanel panel in sliders)
            {
                UIComponent sliderDay = panel.Find("DaySlider");
                UIComponent sliderNight = panel.Find("NightSlider");
                UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");
                UILabel total = panel.Find<UILabel>("Total");
                UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
                UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
                UIComponent icon = panel.Find("Icon");
                UIButton buttonPlusDay = panel.Find<UIButton>("Budget Plus One Button Day");
                UIButton buttonMinusDay = panel.Find<UIButton>("Budget Minus One Button Day");
                UIButton buttonPlusNight = panel.Find<UIButton>("Budget Plus One Button Night");
                UIButton buttonMinusNight = panel.Find<UIButton>("Budget Minus One Button Night");

                panel.width = cs.panelWidth;

                sliderDay.size = new Vector2(210, sliderDay.height);
                sliderDay.relativePosition = new Vector3(cs.sliderX, sliderDay.relativePosition.y);
                sliderDay.isVisible = cs.sliderIsVisible;
                sliderNight.size = new Vector2(210, sliderNight.height);
                sliderNight.relativePosition = new Vector3(cs.sliderX, sliderNight.relativePosition.y);
                sliderNight.isVisible = cs.sliderIsVisible;

                // background slider sprite has to be scaled extra since After Dark update
                sliderBackground.size = new Vector2(sliderDay.size.x, sliderBackground.height);
                sliderBackground.relativePosition = new Vector3(cs.sliderX, sliderBackground.relativePosition.y);
                sliderBackground.isVisible = cs.sliderIsVisible;

                total.relativePosition = new Vector3(cs.totalX, cs.totalY);
                total.size = new Vector2(cs.totalWidth, cs.totalHeight);
                total.isVisible = cs.totalIsVisible;

                percentageDay.relativePosition = new Vector3(cs.percentageDayX, cs.percentageDayY);
                percentageDay.size = new Vector2(cs.percentageWidth, cs.percentageHeight);
                percentageDay.isVisible = cs.percentageIsVisible;
                percentageNight.relativePosition = new Vector3(cs.percentageNightX, cs.percentageNightY);
                percentageNight.size = new Vector2(cs.percentageWidth, cs.percentageHeight);
                percentageNight.isVisible = cs.percentageIsVisible;

                icon.relativePosition = new Vector3(cs.iconX, icon.relativePosition.y);

                buttonPlusDay.relativePosition = new Vector3(cs.buttonPlusDayX, cs.buttonPlusDayY);
                buttonPlusDay.isVisible = cs.buttonPlusIsVisible;
                buttonPlusNight.relativePosition = new Vector3(cs.buttonPlusNightX, cs.buttonPlusNightY);
                buttonPlusNight.isVisible = cs.buttonPlusIsVisible;

                buttonMinusDay.relativePosition = new Vector3(cs.buttonMinusDayX, cs.buttonMinusDayY);
                buttonMinusDay.isVisible = cs.buttonMinusIsVisible;
                buttonMinusNight.relativePosition = new Vector3(cs.buttonMinusNightX, cs.buttonMinusNightY);
                buttonMinusNight.isVisible = cs.buttonMinusIsVisible;
            }

            autoLayoutPadding = new RectOffset(cs.autolayoutpadding[0], cs.autolayoutpadding[1], cs.autolayoutpadding[2], cs.autolayoutpadding[3]);
            if (mode == Mode.Slim)
            {
                for (int i = 0; i < slimCloseTimer.Count; i++)
                {
                    slimCloseTimer[i] = -1;
                }
                if (Parent.isLeft)
                {
                    autoLayoutStart = LayoutStart.BottomRight;
                }
            }
            else
            {
                autoLayoutStart = LayoutStart.TopLeft;
            }
        }



        /// <summary>
        /// Decreases the percentage value of the budget.
        /// </summary>
        /// <param name="component">The clicked button.</param>
        /// <param name="eventParam">Unused</param>
        private void adjustBudgetMinusDay(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIComponent panel = component.parent;
            UISlider slider = panel.Find<UISlider>("DaySlider");
            if (slider.value - 5 > 50)
                slider.value -= 5;
            else
                slider.value = 50;
            UILabel percentage = panel.Find<UILabel>("DayPercentage");
            percentage.text = "" + slider.value;
        }

        /// <summary>
        /// Increases tthe percentage value of the budget.
        /// </summary>
        /// <param name="component">The clicked button.</param>
        /// <param name="eventParam">Unused</param>
        private void adjustBudgetPlusDay(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIComponent panel = component.parent;
            UISlider slider = panel.Find<UISlider>("DaySlider");
            if (slider.value + 5 < 150)
                slider.value += 5;
            else
                slider.value = 150;
            UILabel percentage = panel.Find<UILabel>("DayPercentage");
            percentage.text = "" + slider.value;
        }

        /// <summary>
        /// Decreases the percentage value of the budget.
        /// </summary>
        /// <param name="component">The clicked button.</param>
        /// <param name="eventParam">Unused</param>
        private void adjustBudgetMinusNight(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIComponent panel = component.parent;
            UISlider slider = panel.Find<UISlider>("NightSlider");
            if (slider.value - 5 > 50)
                slider.value -= 5;
            else
                slider.value = 50;
            UILabel percentage = panel.Find<UILabel>("NightPercentage");
            percentage.text = "" + slider.value;
        }

        /// <summary>
        /// Increases tthe percentage value of the budget.
        /// </summary>
        /// <param name="component">The clicked button.</param>
        /// <param name="eventParam">Unused</param>
        private void adjustBudgetPlusNight(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIComponent panel = component.parent;
            UISlider slider = panel.Find<UISlider>("NightSlider");
            if (slider.value + 5 < 150)
                slider.value += 5;
            else
                slider.value = 150;
            UILabel percentage = panel.Find<UILabel>("NightPercentage");
            percentage.text = "" + slider.value;
        }



        /// <summary>
        /// Adds a copy of the slider panel object to the extended panel.
        /// </summary>
        /// <param name="originalSliderObject">The original slider panel to add.</param>
        public void addSlider(UIPanel originalSliderObject)
        {
            UIPanel sliderObject = duplicateUIPanel(originalSliderObject);
            //sliderObject.transform.parent = this.transform;
            AttachUIComponent(sliderObject.gameObject);
            sliders.Add(sliderObject);
            slimCloseTimer.Add(-1);
        }

        /// <summary>
        /// Adds a spacing panel for layout purposes. Call after you added all slider objects.
        /// </summary>
        public void addSpacingPanel()
        {
            GameObject goSpacingPanel = new GameObject("SpacingPanel");
            UIPanel spacingPanel = goSpacingPanel.AddComponent<UIPanel>();
            spacingPanel.transform.parent = this.transform;
            spacingPanel.size = new Vector2(1, 7);
            AttachUIComponent(goSpacingPanel);
        }
        



        /// <summary>
        /// Duplicates an existing slider budget panel and changes UI.
        /// </summary>
        /// <param name="original">Original slider budget panel</param>
        /// <returns>A copy of the original slider object.</returns>
        private UIPanel duplicateUIPanel(UIPanel original)
        {
            UIPanel panel = InstanceManager.Instantiate(original);
            

            // NEW
            panel.width = cs.panelWidth;
            panel.name = panel.name.Substring(0, panel.name.Length - 7); // delete ' (Copy)' mark
            UIComponent sliderDay = panel.Find("DaySlider");
            sliderDay.size = new Vector2(210, sliderDay.height);
            sliderDay.relativePosition = new Vector3(cs.sliderX, sliderDay.relativePosition.y);
            sliderDay.isVisible = cs.sliderIsVisible;
            UIComponent sliderNight = panel.Find("NightSlider");
            sliderNight.size = new Vector2(210, sliderNight.height);
            sliderNight.relativePosition = new Vector3(cs.sliderX, sliderNight.relativePosition.y);
            sliderNight.isVisible = cs.sliderIsVisible;
            UIComponent sliderBackground = panel.Find("SliderBackground");
            sliderBackground.size = new Vector2(sliderDay.size.x, sliderBackground.height);
            sliderBackground.relativePosition = new Vector3(cs.sliderX, sliderBackground.relativePosition.y);
            sliderBackground.isVisible = cs.sliderIsVisible;
            UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
            percentageDay.relativePosition = new Vector3(cs.percentageDayX, cs.percentageDayY);
            percentageDay.size = new Vector2(cs.percentageWidth, cs.percentageHeight);
            percentageDay.textScale = 0.9f;
            percentageDay.isVisible = cs.percentageIsVisible;
            UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
            percentageNight.relativePosition = new Vector3(cs.percentageNightX, cs.percentageNightY);
            percentageNight.size = new Vector2(cs.percentageWidth, cs.percentageHeight);
            percentageNight.textScale = 0.9f;
            percentageNight.isVisible = cs.percentageIsVisible;
            UILabel total = panel.Find<UILabel>("Total");
            total.relativePosition = new Vector3(cs.totalX, cs.totalY);
            total.size = new Vector2(cs.totalWidth, cs.totalHeight);
            total.textScale = 0.85f;
            total.isVisible = cs.totalIsVisible;
            UIComponent icon = panel.Find("Icon");
            icon.relativePosition = new Vector3(cs.iconX, icon.relativePosition.y);
            icon.eventClick += toggleMode;
            icon.isInteractive = true;
            UIButton buttonMinusDay = panel.AddUIComponent<UIButton>();
            buttonMinusDay.name = "Budget Minus One Button Day";
            buttonMinusDay.size = new Vector2(18, 18);
            buttonMinusDay.relativePosition = new Vector3(cs.buttonMinusDayX, cs.buttonMinusDayY);
            buttonMinusDay.normalBgSprite = "ButtonMenu";
            buttonMinusDay.focusedBgSprite = "ButtonMenuFocused";
            buttonMinusDay.hoveredBgSprite = "ButtonMenuHovered";
            buttonMinusDay.pressedBgSprite = "ButtonMenuPressed";
            buttonMinusDay.text = "-";
            buttonMinusDay.textScale = 1.2f;
            buttonMinusDay.textHorizontalAlignment = UIHorizontalAlignment.Center;
            buttonMinusDay.textVerticalAlignment = UIVerticalAlignment.Middle;
            buttonMinusDay.textColor = new Color32(255, 0, 0, 255);
            buttonMinusDay.eventClick += adjustBudgetMinusDay;
            buttonMinusDay.isVisible = cs.buttonMinusIsVisible;
            UIButton buttonPlusDay = panel.AddUIComponent<UIButton>();
            buttonPlusDay.name = "Budget Plus One Button Day";
            buttonPlusDay.size = new Vector2(18, 18);
            buttonPlusDay.relativePosition = new Vector3(cs.buttonPlusDayX, cs.buttonPlusDayY);
            buttonPlusDay.normalBgSprite = "ButtonMenu";
            buttonPlusDay.focusedBgSprite = "ButtonMenuFocused";
            buttonPlusDay.hoveredBgSprite = "ButtonMenuHovered";
            buttonPlusDay.pressedBgSprite = "ButtonMenuPressed";
            buttonPlusDay.text = "+";
            buttonPlusDay.textColor = new Color32(0, 255, 0, 255);
            buttonPlusDay.textScale = 1.2f;
            buttonPlusDay.textHorizontalAlignment = UIHorizontalAlignment.Center;
            buttonPlusDay.textVerticalAlignment = UIVerticalAlignment.Middle;
            buttonPlusDay.eventClick += adjustBudgetPlusDay;
            buttonPlusDay.isVisible = cs.buttonPlusIsVisible;
            UIButton buttonMinusNight = panel.AddUIComponent<UIButton>();
            buttonMinusNight.name = "Budget Minus One Button Night";
            buttonMinusNight.size = new Vector2(18, 18);
            buttonMinusNight.relativePosition = new Vector3(cs.buttonMinusDayX, cs.buttonMinusDayY);
            buttonMinusNight.normalBgSprite = "ButtonMenu";
            buttonMinusNight.focusedBgSprite = "ButtonMenuFocused";
            buttonMinusNight.hoveredBgSprite = "ButtonMenuHovered";
            buttonMinusNight.pressedBgSprite = "ButtonMenuPressed";
            buttonMinusNight.text = "-";
            buttonMinusNight.textScale = 1.2f;
            buttonMinusNight.textHorizontalAlignment = UIHorizontalAlignment.Center;
            buttonMinusNight.textVerticalAlignment = UIVerticalAlignment.Middle;
            buttonMinusNight.textColor = new Color32(255, 0, 0, 255);
            buttonMinusNight.eventClick += adjustBudgetMinusNight;
            buttonMinusNight.isVisible = cs.buttonMinusIsVisible;
            UIButton buttonPlusNight = panel.AddUIComponent<UIButton>();
            buttonPlusNight.name = "Budget Plus One Button Night";
            buttonPlusNight.size = new Vector2(18, 18);
            buttonPlusNight.relativePosition = new Vector3(cs.buttonPlusDayX, cs.buttonPlusDayY);
            buttonPlusNight.normalBgSprite = "ButtonMenu";
            buttonPlusNight.focusedBgSprite = "ButtonMenuFocused";
            buttonPlusNight.hoveredBgSprite = "ButtonMenuHovered";
            buttonPlusNight.pressedBgSprite = "ButtonMenuPressed";
            buttonPlusNight.text = "+";
            buttonPlusNight.textColor = new Color32(0, 255, 0, 255);
            buttonPlusNight.textScale = 1.2f;
            buttonPlusNight.textHorizontalAlignment = UIHorizontalAlignment.Center;
            buttonPlusNight.textVerticalAlignment = UIVerticalAlignment.Middle;
            buttonPlusNight.eventClick += adjustBudgetPlusNight;
            buttonPlusNight.isVisible = cs.buttonPlusIsVisible;
            return panel;

        }



        /// <summary>
        /// If the mouse enters the slider, this method shows percentages and hides the total value.
        /// Displays the slider in slim mode.
        /// </summary>
        /// <param name="panel">The slider object the player hovers over</param>
        private void onPanelEnter(UIComponent panel)
        {
            UILabel total = panel.Find<UILabel>("Total");
            total.isVisible = false;
            UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
            UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
            percentageDay.isVisible = true;
            percentageNight.isVisible = true;
            UISlider sliderDay = panel.Find<UISlider>("DaySlider");
            UISlider sliderNight = panel.Find<UISlider>("NightSlider");
            percentageDay.text = "" + sliderDay.value;
            percentageNight.text = "" + sliderNight.value;
            UIComponent sliderBackground = panel.Find("SliderBackground");
            if (Parent.mode == Mode.Slim)
            {
                panel.width = 355;
                sliderDay.isVisible = true;
                sliderNight.isVisible = true;
                sliderBackground.isVisible = true;
                sliderDay.size = new Vector2(210, sliderDay.height); // fixes a bug (slider too long otherwise)
                sliderNight.size = new Vector2(210, sliderNight.height); // fixes a bug (slider too long otherwise)
                sliderBackground.size = new Vector2(210, sliderBackground.height); // fixes a bug (slider too long otherwise)
                
                if (Parent.isLeft)
                {
                    UISprite icon = panel.Find<UISprite>("Icon");
                    icon.relativePosition = new Vector3(315, icon.relativePosition.y);
                    percentageDay.relativePosition = new Vector3(10, cs.percentageDayY);
                    percentageNight.relativePosition = new Vector3(51, cs.percentageNightY); // TODO: validate correctness of the x coordination
                    total.relativePosition = new Vector3(10, cs.totalY);
                }
                else
                {
                    percentageDay.relativePosition = new Vector3(265, cs.percentageDayY);
                    percentageNight.relativePosition = new Vector3(306, cs.percentageNightY); // TODO: validate correctness of the x coordination
                    total.relativePosition = new Vector3(265, cs.totalY);
                }
            }
            else if (Parent.mode == Mode.noSlider)
            {
                UIButton buttonPlusDay = panel.Find<UIButton>("Budget Plus One Button Day");
                UIButton buttonMinusDay = panel.Find<UIButton>("Budget Minus One Button Day");
                UIButton buttonPlusNight = panel.Find<UIButton>("Budget Plus One Button Night");
                UIButton buttonMinusNight = panel.Find<UIButton>("Budget Minus One Button Night");
                buttonPlusDay.isVisible = true;
                buttonMinusDay.isVisible = true;
                buttonPlusNight.isVisible = true;
                buttonMinusNight.isVisible = true;
            }
        }

        /// <summary>
        /// If the mouse leaves the slider, this method shows total and hides percentage value.
        /// </summary>
        /// <param name="panel">The slider object the player hovers over</param>
        private void closePanel(UIPanel panel)
        {
            UILabel total = panel.Find<UILabel>("Total");
            total.isVisible = true;
            UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
            UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
            percentageDay.isVisible = false;
            percentageNight.isVisible = false;
            if (Parent.mode == Mode.Slim)
            {
                panel.width = 130;
                UIComponent sliderDay = panel.Find("DaySlider");
                UIComponent sliderNight = panel.Find("NightSlider");
                UIComponent sliderBackground = panel.Find("SliderBackground");
                sliderDay.isVisible = false;
                sliderNight.isVisible = false;
                sliderBackground.isVisible = false;
                sliderDay.size = new Vector2(210, sliderDay.height);
                sliderNight.size = new Vector2(210, sliderNight.height);
                sliderBackground.size = new Vector2(210, sliderBackground.height);
                if (Parent.isLeft)
                {
                    UISprite icon = panel.Find<UISprite>("Icon");
                    icon.relativePosition = new Vector3(cs.iconX, icon.relativePosition.y);
                    percentageDay.relativePosition = new Vector3(cs.percentageDayX, cs.percentageDayY);
                    percentageNight.relativePosition = new Vector3(cs.percentageNightX, cs.percentageNightY);
                    total.relativePosition = new Vector3(cs.totalX, cs.totalY);
                }
                else
                {
                    percentageDay.relativePosition = new Vector3(45, cs.percentageDayY);
                    percentageNight.relativePosition = new Vector3(86, cs.percentageNightY);
                    total.relativePosition = new Vector3(45, cs.totalY);
                }
            }
            else if (Parent.mode == Mode.noSlider)
            {
                UIButton buttonPlusDay = panel.Find<UIButton>("Budget Plus One Button Day");
                UIButton buttonMinusDay = panel.Find<UIButton>("Budget Minus One Button Day");
                UIButton buttonPlusNight = panel.Find<UIButton>("Budget Plus One Button Night");
                UIButton buttonMinusNight = panel.Find<UIButton>("Budget Minus One Button Night");
                buttonPlusDay.isVisible = false;
                buttonMinusDay.isVisible = false;
                buttonPlusNight.isVisible = false;
                buttonMinusNight.isVisible = false;
            }
        }


        /// <summary>
        /// Opens and closes the sliders upon hovering over them.
        /// </summary>
        public override void Update()
        {
            for (int i = 0; i < sliders.Count; i++)
            {
                if (sliders[i].containsMouse && slimCloseTimer[i] <= 1)
                {
                    onPanelEnter(sliders[i]);
                    slimCloseTimer[i] = 30;
                } 
                else if (slimCloseTimer[i] == 0)
                    closePanel(sliders[i]);
                if (slimCloseTimer[i] >= 0)
                    slimCloseTimer[i] -= 1;
            }
        }
    }
}
