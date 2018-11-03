using System;
using System.Collections.Generic;

using ICities;
using ColossalFramework.UI;
using UnityEngine;


namespace BetterBudget
{
    class UIEmbeddedBudgetPanel : UIPanel
    {

        private BetterBudget2 _main;
        private UIPanel _infoViewPanel;

        private List<bool> _sliderIsOpenList;
        private List<UIPanel> _sliderList;

        public BBEmbeddedSaveFile settings;

        private bool isEditEnabled;

        private bool isPublicTransportInfoViewPanelAndDidNotApplyFix;

        public void initialize(BetterBudget2 main, UIPanel infoViewPanel)
        {
            this._main = main;
            this._infoViewPanel = infoViewPanel;
            this.anchor = UIAnchorStyle.Bottom | UIAnchorStyle.Left | UIAnchorStyle.Right;
            this.transform.parent = _infoViewPanel.transform;
            this.relativePosition = new Vector3(8, _infoViewPanel.height + 2);
            this.size = new Vector2(_infoViewPanel.width - 16, 0); // size
            this._sliderList = new List<UIPanel>();
            this._sliderIsOpenList = new List<bool>();
            this.isEditEnabled = true;
            settings = new BBEmbeddedSaveFile();
            settings.infoViewPanelName = infoViewPanel.name;

            this.isPublicTransportInfoViewPanelAndDidNotApplyFix = false;
            if (infoViewPanel.name.Equals("(Library) PublicTransportInfoViewPanel"))
                this.isPublicTransportInfoViewPanelAndDidNotApplyFix = true;
            

            UISprite icon = _infoViewPanel.Find("Caption").Find<UISprite>("Icon");
            icon.eventClick += openSelectorPanel;
            icon.BringToFront();
        }

        private void openSelectorPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (isEditEnabled)
            {
                _infoViewPanel.AddUIComponent<UIEmbeddedBudgetPanelSelector>().initialize(_main, this, _sliderList);
                isEditEnabled = false;
            }
        }


        public void setSliderPanel(String[] sliderPanels)
        {
            clearSliderPanel();
            settings.budgetSliderNameList.Clear();

            int numberOfSliders = 0;

            foreach (String sliderName in sliderPanels)
            {
                UIPanel originalSlider = _main.getSliderPanel(sliderName);
                if (originalSlider != null)
                {
                    // DEBUG
                    //foreach (UIComponent component in originalSlider.components)
                    //{
                    //    Debug.Log(component.name, component);
                    //}

                    numberOfSliders++;
                    
                    UIPanel panel = InstanceManager.Instantiate(originalSlider);
                    //UIPanel panel = Instantiate<UIPanel>(originalSlider);
                    
                    panel.name = panel.name.Substring(0, panel.name.Length - 7); // delete ' (Copy)' mark
                    AttachUIComponent(panel.gameObject);

                    UIComponent sliderDay = panel.Find<UISlider>("DaySlider");
                    UIComponent sliderNight = panel.Find<UISlider>("NightSlider");
                    UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");
                    UILabel total = panel.Find<UILabel>("Total");
                    UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
                    UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
                    UISprite icon = panel.Find<UISprite>("Icon");

                    panel.transform.parent = this.transform;
                    panel.relativePosition = new Vector3(0, _sliderList.Count * 50);
                    panel.size = new Vector2(width, panel.height);

                    icon.relativePosition = new Vector3(1, icon.relativePosition.y);

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

                    _sliderList.Add(panel);
                    settings.budgetSliderNameList.Add(sliderName);
                }
            }

            if (numberOfSliders > 0)
            {
                changeInfoViewPanelHeight(_infoViewPanel.height + numberOfSliders * 50 + 10);
            }

            this.height += numberOfSliders * 50;
        }



        private void clearSliderPanel()
        {
            if (_sliderList.Count == 0)
                return;
            changeInfoViewPanelHeight(_infoViewPanel.height - _sliderList.Count * 50 - 10);
            this.height -= _sliderList.Count * 50;

            for (int i = 0; i < _sliderList.Count; i++)
            {
                GameObject.Destroy(_sliderList[i].gameObject);
            }
            _sliderList.Clear();
        }

        private void onPanelEnter(UIPanel panel)
        {
            UILabel total = panel.Find<UILabel>("Total");
            UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
            UILabel percentageNight = panel.Find<UILabel>("NightPercentage");

            total.isVisible = false;
            percentageDay.isVisible = true;
            percentageNight.isVisible = true;
        }

        private void onPanelLeave(UIPanel panel)
        {
            UILabel total = panel.Find<UILabel>("Total");
            UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
            UILabel percentageNight = panel.Find<UILabel>("NightPercentage");

            total.isVisible = true;
            percentageDay.isVisible = false;
            percentageNight.isVisible = false;
        }

        private void changeInfoViewPanelHeight(float newHeight) {
            if (newHeight == 0)
                return;

            UIComponent[] list = _infoViewPanel.GetComponentsInChildren<UIComponent>();
            float[] X = new float[list.Length];
            float[] Y = new float[list.Length];
            float[] Height = new float[list.Length];
            float[] Width = new float[list.Length];
            for (int i = 1; i < list.Length; i++)
            {
                X[i] = list[i].relativePosition.x;
                Y[i] = list[i].relativePosition.y;
                Width[i] = list[i].width;
                Height[i] = list[i].height;
            }

            _infoViewPanel.height = newHeight;

            for (int i = 1; i < list.Length; i++)
            {
                list[i].relativePosition = new Vector3(X[i], Y[i]);
                list[i].size = new Vector2(Width[i], Height[i]);
            }
        }


        public override void Update()
        {
            if (!isVisible)
                return;

            // fix for panel height because of invisible public transport entries (which occurs when expansions are not installed/enabled) (execute only once)
            if (isPublicTransportInfoViewPanelAndDidNotApplyFix)
            {
                int invisibleEntries = 0;
                foreach (UIComponent panel in _infoViewPanel.Find<UIPanel>("TransportContainer").components)
                {
                    if (!panel.isVisible)
                    {
                        invisibleEntries++;
                    }
                }
                //Debug.Log("TransportContainer Entries: " + _infoViewPanel.Find<UIPanel>("TransportContainer").components.Count + " Invisible: " + invisibleEntries);
                _infoViewPanel.height -= 23 * invisibleEntries;

                invisibleEntries = 0;
                foreach (UIPanel panel in _infoViewPanel.Find<UIPanel>("Legend").Find<UIPanel>("Panel").components)
                {
                    if (!panel.isVisible)
                    {
                        invisibleEntries++;
                    }
                }

                //Debug.Log("Legend Entries: " + _infoViewPanel.Find<UIPanel>("Legend").Find<UIPanel>("Panel").components.Count + " Invisible: " + invisibleEntries);
                _infoViewPanel.height -= 18 * invisibleEntries;
                isPublicTransportInfoViewPanelAndDidNotApplyFix = false;
            }

            // Normal Update Routine
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

        public void enableEditButton()
        {
            isEditEnabled = true;
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
                if (sliderSprite.spriteName.Length - 8 > 0)
                    sliderSprite.spriteName = sliderSprite.spriteName.Substring(0, sliderSprite.spriteName.Length - 8);
            else
                sliderSprite.spriteName = sliderSprite.spriteName + "Disabled";
        }

        
        public BBEmbeddedSaveFile getSettings()
        {
            UISprite icon = _infoViewPanel.Find("Caption").Find<UISprite>("Icon");
            icon.eventClick -= openSelectorPanel;
            if (_sliderList.Count > 0 )
                changeInfoViewPanelHeight(_infoViewPanel.height - _sliderList.Count * 50 - 10);
            return settings;
        }

        public UIPanel getInfoViewPanel()
        {
            return _infoViewPanel;
        }
    }
}
