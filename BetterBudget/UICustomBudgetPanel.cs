using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ICities;
using ColossalFramework.UI;
using UnityEngine;

namespace BetterBudget
{
    class UICustomBudgetPanel : UIPanel
    {
        BetterBudget2 _main;

        private Boolean isEditEnabled;
        private Mode mode;
        public BBCustomSaveFile settings;

        UISprite icon;
        UIDragHandle draghandler;
        UILabel title;
        UIButton quitButton;
        //UIResizeHandle resizehandler;

        private List<UIPanel> _sliderList;

        public void initialize(BetterBudget2 main, BBCustomSaveFile settings)
        {
            this._main = main;
            _main.addCustomPanel(this);
            this.settings = settings;
            this.relativePosition = new Vector3(settings.x, settings.y);
            this.isEditEnabled = true;
            this.name = "Custom Budget Panel";
            this.size = new Vector2(settings.width, 46);
            this.mode = Mode.Default;
            this.opacity = settings.opacity;
            _sliderList = new List<UIPanel>();
            backgroundSprite = "MenuPanel2";

            // title
            title = AddUIComponent<UILabel>();
            title.text = "Custom Budget Panel";
            title.relativePosition = new Vector3((width / 2) - ((title.text.Length / 2) * 8), 12);
            title.name = "Title";


            // Drag Handler
            draghandler = AddUIComponent<UIDragHandle>();
            draghandler.relativePosition = new Vector3(0, 0);
            draghandler.target = this;
            draghandler.name = "Drag Handler";
            draghandler.size = new Vector2(this.width, 41);

            // Resize Handler
            //resizehandler = AddUIComponent<UIResizeHandle>();
            //resizehandler.edges = UIResizeHandle.ResizeEdge.Right;

            // Quit Button
            quitButton = AddUIComponent<UIButton>();
            quitButton.name = "Quit Button";
            quitButton.normalBgSprite = "buttonclose";
            quitButton.pressedBgSprite = "buttonclosehover";
            quitButton.hoveredBgSprite = "buttonclosepressed";
            quitButton.focusedBgSprite = "buttonclosehover";
            quitButton.eventClick += deleteCustomPanel;
            quitButton.size = new Vector2(36, 36);
            quitButton.relativePosition = new Vector3(width - 43, 2);

            // icon
            icon = AddUIComponent<UISprite>();
            icon.relativePosition = new Vector3(2, 2);
            icon.spriteName = "MoneyThumb";
            icon.name = "Icon";
            icon.size = new Vector2(40, 40);
            icon.eventClicked += openSelectorPanel;
            icon.BringToFront();

            if (settings.budgetSliderNameList.Count > 0)
            {
                setSliderPanel(settings.budgetSliderNameList.ToArray());

                // required on load
                while (mode != settings.mode)
                {
                    toggleMode();
                }
            }

        }

        private void deleteCustomPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            _main.removeCustomPanel(this);
            _main.RemoveUIComponent(this);
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            GameObject.Destroy(this.gameObject);
        }

        private void openSelectorPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (isEditEnabled)
            {
                this.AddUIComponent<UICustomBudgetPanelSelector>().initialize(_main, this, _sliderList);
                isEditEnabled = false;
            }
        }

        public void enableEditButton()
        {
            isEditEnabled = true;
        }


        private void toggleMode(UIComponent component, UIMouseEventParameter eventParam)
        {
            toggleMode();
        }

        private void toggleMode() {
            if (!isEditEnabled)
                return;
            if (mode == Mode.Default)
            {
                width = settings.width;
                mode = Mode.SliderOnly;
                title.isVisible = false;
                draghandler.isVisible = false;
                //resizehandler.isVisible = false;
                quitButton.isVisible = false;
                icon.isVisible = false;
                backgroundSprite = "";
            }
            else if (mode == Mode.SliderOnly)
            {
                width = 144;
                mode = Mode.Slim;

                foreach (UIPanel panel in _sliderList)
                {
                    panel.width = 140;

                    UIComponent sliderDay = panel.Find("DaySlider");
                    UIComponent sliderNight = panel.Find("NightSlider");
                    UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");

                    sliderDay.isVisible = false;
                    sliderNight.isVisible = false;
                    sliderBackground.isVisible = false;
                }
            }
            else if (mode == Mode.Slim)
            {
                width = 144;
                mode = Mode.PlusMinus;
                foreach (UIPanel panel in _sliderList)
                {
                    panel.width = panel.parent.width - 8;

                    UIComponent sliderDay = panel.Find("DaySlider");
                    UIComponent sliderNight = panel.Find("NightSlider");
                    UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");
                    UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
                    UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
                    UIButton buttonPlusDay = panel.Find<UIButton>("Budget Plus Button Day");
                    UIButton buttonMinusDay = panel.Find<UIButton>("Budget Minus Button Day");
                    UIButton buttonPlusNight = panel.Find<UIButton>("Budget Plus Button Night");
                    UIButton buttonMinusNight = panel.Find<UIButton>("Budget Minus Button Night");

                    sliderDay.isVisible = false;
                    sliderNight.isVisible = false;
                    sliderBackground.isVisible = false;

                    percentageDay.relativePosition = new Vector3(45, 5);
                    percentageDay.size = new Vector2(90, 18);
                    percentageNight.relativePosition = new Vector3(45, 23);
                    percentageNight.size = new Vector2(90, 18);

                    buttonPlusDay.isVisible = true;
                    buttonMinusDay.isVisible = true;
                    buttonPlusNight.isVisible = true;
                    buttonMinusNight.isVisible = true;
                }

            }
            else if (mode == Mode.PlusMinus)
            {
                width = settings.width;
                mode = Mode.Default;
                title.isVisible = true;
                draghandler.isVisible = true;
                //resizehandler.isVisible = true;
                quitButton.isVisible = true;
                icon.isVisible = true;
                backgroundSprite = "MenuPanel2";


                foreach (UIPanel panel in _sliderList)
                {
                    panel.width = panel.parent.width-6;

                    UIComponent sliderDay = panel.Find("DaySlider");
                    UIComponent sliderNight = panel.Find("NightSlider");
                    UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");
                    UILabel total = panel.Find<UILabel>("Total");
                    UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
                    UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
                    UIButton buttonPlusDay = panel.Find<UIButton>("Budget Plus Button Day");
                    UIButton buttonMinusDay = panel.Find<UIButton>("Budget Minus Button Day");
                    UIButton buttonPlusNight = panel.Find<UIButton>("Budget Plus Button Night");
                    UIButton buttonMinusNight = panel.Find<UIButton>("Budget Minus Button Night");

                    sliderDay.isVisible = true;
                    sliderNight.isVisible = true;
                    sliderBackground.isVisible = true;

                    percentageDay.relativePosition = new Vector3(panel.width - 92, total.relativePosition.y);
                    percentageDay.size = new Vector2(45, total.height);
                    percentageNight.relativePosition = new Vector3(panel.width - 47, total.relativePosition.y);
                    percentageNight.size = new Vector2(45, total.height);

                    buttonPlusDay.isVisible = false;
                    buttonMinusDay.isVisible = false;
                    buttonPlusNight.isVisible = false;
                    buttonMinusNight.isVisible = false;
                }
            }
        }

        public override void Update()
        {
            if (!isVisible)
                return;
            foreach (UIPanel panel in _sliderList)
            {
                if (panel.containsMouse)
                {
                    onPanelEnter(panel);
                }
                else
                {
                    onPanelLeave(panel);
                }

            }
        }

        private void onPanelEnter(UIPanel panel)
        {
            UILabel total = panel.Find<UILabel>("Total");
            UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
            UILabel percentageNight = panel.Find<UILabel>("NightPercentage");

            total.isVisible = false;
            percentageDay.isVisible = true;
            percentageNight.isVisible = true;

            if (mode == Mode.Slim)
            {
                width = settings.width;
                panel.width = panel.parent.width - 6;

                UIComponent sliderDay = panel.Find("DaySlider");
                UIComponent sliderNight = panel.Find("NightSlider");
                UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");

                sliderDay.isVisible = true;
                sliderNight.isVisible = true;
                sliderBackground.isVisible = true;
            }
            else if (mode == Mode.PlusMinus)
            {
                panel.width = 140;
                UIButton buttonPlusDay = panel.Find<UIButton>("Budget Plus Button Day");
                UIButton buttonMinusDay = panel.Find<UIButton>("Budget Minus Button Day");
                UIButton buttonPlusNight = panel.Find<UIButton>("Budget Plus Button Night");
                UIButton buttonMinusNight = panel.Find<UIButton>("Budget Minus Button Night");

                buttonPlusDay.isVisible = true;
                buttonPlusNight.isVisible = true;
                buttonMinusDay.isVisible = true;
                buttonMinusNight.isVisible = true;

                percentageDay.relativePosition = new Vector3(45, 5);
                percentageDay.size = new Vector2(90, 18);
                percentageNight.relativePosition = new Vector3(45, 23);
                percentageNight.size = new Vector2(90, 18);
            }
        }

        private void onPanelLeave(UIPanel panel)
        {

            UILabel total = panel.Find<UILabel>("Total");
            UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
            UILabel percentageNight = panel.Find<UILabel>("NightPercentage");

            total.isVisible = true;
            percentageDay.isVisible = false;
            percentageNight.isVisible = false;
            
            if (mode == Mode.Slim)
            {
                width = 144;
                panel.width = 140;

                UIComponent sliderDay = panel.Find("DaySlider");
                UIComponent sliderNight = panel.Find("NightSlider");
                UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");

                sliderDay.isVisible = false;
                sliderNight.isVisible = false;
                sliderBackground.isVisible = false;
            }
            else if (mode == Mode.PlusMinus)
            {
                panel.width = 140;
                UIButton buttonPlusDay = panel.Find<UIButton>("Budget Plus Button Day");
                UIButton buttonMinusDay = panel.Find<UIButton>("Budget Minus Button Day");
                UIButton buttonPlusNight = panel.Find<UIButton>("Budget Plus Button Night");
                UIButton buttonMinusNight = panel.Find<UIButton>("Budget Minus Button Night");

                buttonPlusDay.isVisible = false;
                buttonPlusNight.isVisible = false;
                buttonMinusDay.isVisible = false;
                buttonMinusNight.isVisible = false;

                percentageDay.relativePosition = new Vector3(panel.width - 92, total.relativePosition.y);
                percentageDay.size = new Vector2(45, total.height);
                percentageNight.relativePosition = new Vector3(panel.width - 47, total.relativePosition.y);
                percentageNight.size = new Vector2(45, total.height);
            }
        }

        private void clearSliderPanel()
        {
            if (_sliderList.Count == 0)
                return;
            this.height = 41;

            for (int i = 0; i < _sliderList.Count; i++)
            {
                GameObject.Destroy(_sliderList[i].gameObject);
            }
            _sliderList.Clear();
        }

        public void setSliderPanel(String[] sliderPanels)
        {
            clearSliderPanel();

            settings.budgetSliderNameList.Clear();

            foreach (String sliderName in sliderPanels)
            {
                UIPanel originalSlider = _main.getSliderPanel(sliderName);
                if (originalSlider != null)
                {
                    settings.budgetSliderNameList.Add(sliderName);
                    UIPanel panel = InstanceManager.Instantiate(originalSlider);
                    panel.name = panel.name.Substring(0, panel.name.Length - 7); // delete ' (Copy)' mark
                    AttachUIComponent(panel.gameObject);
                    UIComponent sliderDay = panel.Find("DaySlider");
                    UIComponent sliderNight = panel.Find("NightSlider");
                    UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");
                    UILabel total = panel.Find<UILabel>("Total");
                    UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
                    UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
                    UISprite icon = panel.Find<UISprite>("Icon");

                    panel.transform.parent = this.transform;
                    panel.relativePosition = new Vector3(2, _sliderList.Count * 50 + 46);
                    panel.size = new Vector2(width - 6, panel.height - 2);

                    sliderBackground.relativePosition = new Vector3(50, sliderBackground.relativePosition.y);
                    sliderBackground.size = new Vector2(panel.width - 98 - 50, sliderBackground.height);
                    sliderDay.relativePosition = new Vector3(50, sliderDay.relativePosition.y);
                    sliderDay.size = new Vector2(panel.width - 98 - 50, sliderDay.height);
                    sliderNight.relativePosition = new Vector3(50, sliderNight.relativePosition.y);
                    sliderNight.size = new Vector2(panel.width - 98 - 50, sliderNight.height);

                    total.relativePosition = new Vector3(panel.width - 92, total.relativePosition.y);
                    total.size = new Vector2(90, total.height);
                    percentageDay.relativePosition = new Vector3(panel.width - 92, total.relativePosition.y);
                    percentageDay.size = new Vector2(45, total.height);
                    percentageNight.relativePosition = new Vector3(panel.width - 47, total.relativePosition.y);
                    percentageNight.size = new Vector2(45, total.height);

                    UIButton buttonPlusDay = panel.AddUIComponent<UIButton>();
                    UIButton buttonMinusDay = panel.AddUIComponent<UIButton>();
                    UIButton buttonPlusNight = panel.AddUIComponent<UIButton>();
                    UIButton buttonMinusNight = panel.AddUIComponent<UIButton>();

                    buttonPlusDay.name = "Budget Plus Button Day";
                    buttonPlusDay.size = new Vector2(18, 18);
                    buttonPlusDay.relativePosition = new Vector3(117, 5);
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
                    buttonPlusDay.isVisible = false;

                    buttonPlusNight.name = "Budget Plus Button Night";
                    buttonPlusNight.size = new Vector2(18, 18);
                    buttonPlusNight.relativePosition = new Vector3(117, 23);
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
                    buttonPlusNight.isVisible = false;

                    buttonMinusDay.name = "Budget Minus Button Day";
                    buttonMinusDay.size = new Vector2(18, 18);
                    buttonMinusDay.relativePosition = new Vector3(45, 5);
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
                    buttonMinusDay.isVisible = false;

                    buttonMinusNight.name = "Budget Minus Button Night";
                    buttonMinusNight.size = new Vector2(18, 18);
                    buttonMinusNight.relativePosition = new Vector3(45, 23);
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
                    buttonMinusNight.isVisible = false; ;


                    icon.relativePosition = new Vector3(1, icon.relativePosition.y);
                    icon.isInteractive = true;
                    icon.eventClick += toggleMode;
                    icon.BringToFront();

                    _sliderList.Add(panel);
                }
            }
            this.height = _sliderList.Count * 50 + 46;
        }


        /// <summary>
        /// If the player hits a milestone and unlocks more buildings, the slider has to enabled. And vise versa (disable - which is not possible without cheating).
        /// </summary>
        /// <param name="component">The slider panel to change-</param>
        /// <param name="value">Set the comonent to enabled or disabled.</param>
        public void hitMilestone(UIComponent component, bool value)
        {
            UIPanel slider = Find<UIPanel>(component.name);
            if (slider == null)
                return;
            slider.isEnabled = value;
            UISprite sliderSprite = slider.Find<UISprite>("Icon");
            if (value)
                sliderSprite.spriteName = sliderSprite.spriteName.Substring(0, sliderSprite.spriteName.Length - 8);
            else
                sliderSprite.spriteName = sliderSprite.spriteName + "Disabled";
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


        internal BBCustomSaveFile getSettings()
        {
            settings.x = relativePosition.x;
            settings.y = relativePosition.y;
            settings.mode = mode;
            return settings;
        }
    }
}
