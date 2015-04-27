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
                UIComponent slider = panel.Find("Slider");
                UILabel total = panel.Find<UILabel>("Total");
                UILabel percentage = panel.Find<UILabel>("Percentage");
                UIComponent icon = panel.Find("Icon");
                UIButton buttonPlus = panel.Find<UIButton>("Budget Plus One Button");
                UIButton buttonMinus = panel.Find<UIButton>("Budget Minus One Button");

                panel.width = cs.panelWidth;

                slider.size = new Vector2(210,slider.height);
                slider.relativePosition = new Vector3(cs.sliderX, slider.relativePosition.y);
                slider.isVisible = cs.sliderIsVisible;

                total.relativePosition = new Vector3(cs.totalX, cs.totalY);
                total.size = new Vector2(cs.totalWidth, cs.totalHeight);
                total.isVisible = cs.totalIsVisible;

                percentage.relativePosition = new Vector3(cs.percentageX, cs.percentageY);
                percentage.size = new Vector2(cs.percentageWidth, cs.percentageHeight);
                percentage.isVisible = cs.percentageIsVisible;

                icon.relativePosition = new Vector3(cs.iconX, icon.relativePosition.y);

                buttonPlus.relativePosition = new Vector3(cs.buttonPlusX, cs.buttonPlusY);
                buttonPlus.isVisible = cs.buttonPlusIsVisible;

                buttonMinus.relativePosition = new Vector3(cs.buttonMinusX, cs.buttonMinusY);
                buttonMinus.isVisible = cs.buttonMinusIsVisible;
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
        private void adjustBudgetMinus(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIComponent panel = component.parent;
            UISlider slider = panel.Find<UISlider>("Slider");
            slider.value -= 1;
            UILabel percentage = panel.Find<UILabel>("Percentage");
            percentage.text = "" + slider.value;
        }

        /// <summary>
        /// Increases tthe percentage value of the budget.
        /// </summary>
        /// <param name="component">The clicked button.</param>
        /// <param name="eventParam">Unused</param>
        private void adjustBudgetPlus(UIComponent component, UIMouseEventParameter eventParam)
        {
            UIComponent panel = component.parent;
            UISlider slider = panel.Find<UISlider>("Slider");
            slider.value += 1;
            UILabel percentage = panel.Find<UILabel>("Percentage");
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
            
            panel.width = cs.panelWidth;
            panel.name = panel.name.Substring(0, panel.name.Length - 7); // delete ' (Copy)' mark
            //panel.eventMouseEnter += onPanelEnter;
            UIComponent slider = panel.Find("Slider");
            slider.size = new Vector2(210, slider.height);
            slider.relativePosition = new Vector3(cs.sliderX, slider.relativePosition.y);
            slider.isVisible = cs.sliderIsVisible;
            UILabel percentage = panel.Find<UILabel>("Percentage");
            percentage.relativePosition = new Vector3(cs.percentageX, cs.percentageY);
            percentage.size = new Vector2(cs.percentageWidth, cs.percentageHeight);
            percentage.textScale = 0.9f;
            percentage.isVisible = cs.percentageIsVisible;
            UILabel total = panel.Find<UILabel>("Total");
            total.relativePosition = new Vector3(cs.totalX, cs.totalY);
            total.size = new Vector2(cs.totalWidth, cs.totalHeight);
            total.textScale = 0.85f;
            total.isVisible = cs.totalIsVisible;
            UIComponent icon = panel.Find("Icon");
            icon.relativePosition = new Vector3(cs.iconX, icon.relativePosition.y);
            icon.eventClick += toggleMode;
            icon.isInteractive = true;
            UIButton buttonMinus = panel.AddUIComponent<UIButton>();
            buttonMinus.name = "Budget Minus One Button";
            buttonMinus.size = new Vector2(18, 18);
            buttonMinus.relativePosition = new Vector3(cs.buttonMinusX, cs.buttonMinusY);
            buttonMinus.normalBgSprite = "ButtonMenu";
            buttonMinus.focusedBgSprite = "ButtonMenuFocused";
            buttonMinus.hoveredBgSprite = "ButtonMenuHovered";
            buttonMinus.pressedBgSprite = "ButtonMenuPressed";
            buttonMinus.text = "-";
            buttonMinus.textScale = 1.2f;
            buttonMinus.textHorizontalAlignment = UIHorizontalAlignment.Center;
            buttonMinus.textVerticalAlignment = UIVerticalAlignment.Middle;
            buttonMinus.textColor = new Color32(255,0,0,255);
            buttonMinus.eventClick += adjustBudgetMinus;
            buttonMinus.isVisible = cs.buttonMinusIsVisible;
            UIButton buttonPlus = panel.AddUIComponent<UIButton>();
            buttonPlus.name = "Budget Plus One Button";
            buttonPlus.size = new Vector2(18, 18);
            buttonPlus.relativePosition = new Vector3(cs.buttonPlusX, cs.buttonPlusY);
            buttonPlus.normalBgSprite = "ButtonMenu";
            buttonPlus.focusedBgSprite = "ButtonMenuFocused";
            buttonPlus.hoveredBgSprite = "ButtonMenuHovered";
            buttonPlus.pressedBgSprite = "ButtonMenuPressed";
            buttonPlus.text = "+";
            buttonPlus.textColor = new Color32(0, 255, 0, 255);
            buttonPlus.textScale = 1.2f;
            buttonPlus.textHorizontalAlignment = UIHorizontalAlignment.Center;
            buttonPlus.textVerticalAlignment = UIVerticalAlignment.Middle;
            buttonPlus.eventClick += adjustBudgetPlus;
            buttonPlus.isVisible = cs.buttonPlusIsVisible;
            return panel;
        }


        /// <summary>
        /// If the mouse enters the slider, this method shows percentage and hides total value.
        /// Displays the slider in slim mode.
        /// </summary>
        /// <param name="panel">The slider object the player hovers over</param>
        private void onPanelEnter(UIComponent panel)
        {
            UILabel total = panel.Find<UILabel>("Total");
            total.isVisible = false;
            UILabel percentage = panel.Find<UILabel>("Percentage");
            percentage.isVisible = true;
            UISlider slider = panel.Find<UISlider>("Slider");
            percentage.text = "" + slider.value;
            if (Parent.mode == Mode.Slim)
            {
                panel.width = 355;
                slider.isVisible = true;
                slider.size = new Vector2(210, slider.height); // fixes a bug (slider too long otherwise)
                if (Parent.isLeft)
                {
                    UISprite icon = panel.Find<UISprite>("Icon");
                    icon.relativePosition = new Vector3(315, icon.relativePosition.y);
                    percentage.relativePosition = new Vector3(10, cs.percentageY);
                    total.relativePosition = new Vector3(10, cs.totalY);
                }
                else
                {
                    percentage.relativePosition = new Vector3(265, cs.percentageY);
                    total.relativePosition = new Vector3(265, cs.totalY);
                }
            }
            else if (Parent.mode == Mode.noSlider)
            {
                UIButton buttonPlus = panel.Find<UIButton>("Budget Plus One Button");
                UIButton buttonMinus = panel.Find<UIButton>("Budget Minus One Button");
                buttonPlus.isVisible = true;
                buttonMinus.isVisible = true;
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
            UILabel percentage = panel.Find<UILabel>("Percentage");
            percentage.isVisible = false;
            if (Parent.mode == Mode.Slim)
            {
                panel.width = 130;
                UIComponent slider = panel.Find("Slider");
                slider.isVisible = false;
                slider.size = new Vector2(210, slider.height);
                if (Parent.isLeft)
                {
                    UISprite icon = panel.Find<UISprite>("Icon");
                    icon.relativePosition = new Vector3(cs.iconX, icon.relativePosition.y);
                    percentage.relativePosition = new Vector3(cs.percentageX, cs.percentageY);
                    total.relativePosition = new Vector3(cs.totalX, cs.totalY);
                }
                else
                {
                    percentage.relativePosition = new Vector3(45, cs.percentageY);
                    total.relativePosition = new Vector3(45, cs.totalY);
                }
            }
            else if (Parent.mode == Mode.noSlider)
            {
                UIButton buttonPlus = panel.Find<UIButton>("Budget Plus One Button");
                UIButton buttonMinus = panel.Find<UIButton>("Budget Minus One Button");
                buttonPlus.isVisible = false;
                buttonMinus.isVisible = false;
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
