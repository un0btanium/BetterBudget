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

        private List<UIPanel> _sliderList;
        private List<bool> _sliderIsOpen;

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
            this._sliderIsOpen = new List<bool>();
            this.isEditEnabled = true;
            settings = new BBEmbeddedSaveFile();
            settings.infoViewPanelName = infoViewPanel.name;

            this.isPublicTransportInfoViewPanelAndDidNotApplyFix = false;
            if (infoViewPanel.name.Equals("(Library) PublicTransportInfoViewPanel"))
            {
                this.isPublicTransportInfoViewPanelAndDidNotApplyFix = true;
            }

            //if (infoViewPanel.name.Equals("(Library) WaterInfoViewPanel"))
            //{
            //    this.relativePosition = new Vector3(16, _infoViewPanel.height + 2);
            //}

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

            List<BudgetItem> budgetItems = _main.getBudgetCopies(sliderPanels);

            int heightPerBudget = 46;
            int additionalPadding = 10;
            if (budgetItems.Count > 4)
            {
                heightPerBudget = 36;
                additionalPadding = 20;
            }

            foreach (BudgetItem budgetItem in budgetItems)
            {
                UIPanel panel = (UIPanel) budgetItem.component;
                AttachUIComponent(budgetItem.gameObject);

                UIComponent sliderDay = panel.Find<UISlider>("DaySlider");
                UIComponent sliderNight = panel.Find<UISlider>("NightSlider");
                UIComponent sliderBackground = panel.Find<UISlicedSprite>("SliderBackground");
                UILabel total = panel.Find<UILabel>("Total");
                UILabel percentageDay = panel.Find<UILabel>("DayPercentage");
                UILabel percentageNight = panel.Find<UILabel>("NightPercentage");
                UISprite icon = panel.Find<UISprite>("Icon");

                panel.transform.parent = this.transform;
                panel.relativePosition = new Vector3(0, this._sliderList.Count * heightPerBudget);
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

                this._sliderList.Add(panel);
                this._sliderIsOpen.Add(false);
                settings.budgetSliderNameList.Add(budgetItem.name);
            }

            if (budgetItems.Count > 0)
            {
                changeInfoViewPanelHeight(_infoViewPanel.height + (budgetItems.Count * heightPerBudget) + additionalPadding);
                this.height += budgetItems.Count * heightPerBudget;
            }
        }



        private void clearSliderPanel()
        {
            if (_sliderList.Count == 0)
                return;

            int heightPerBudget = 46;
            int additionalPadding = 10;
            if (_sliderList.Count > 4)
            {
                heightPerBudget = 36;
                additionalPadding = 20;
            }

            changeInfoViewPanelHeight(_infoViewPanel.height - (_sliderList.Count * heightPerBudget) - additionalPadding);
            this.height -= _sliderList.Count * heightPerBudget;

            for (int i = 0; i < _sliderList.Count; i++)
            {
                _main.removeBudgetCopy(_sliderList[i]);
                GameObject.Destroy(_sliderList[i].gameObject);
            }
            _sliderList.Clear();
            _sliderIsOpen.Clear();
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
            _main.hoverOverPanelEnded();

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
            int i = 0;
            foreach (UIPanel panel in _sliderList)
            {
                if (panel.containsMouse)
                {

                    if (_sliderIsOpen[i] == false)
                    {
                        onPanelEnter(panel);
                        _sliderIsOpen[i] = true;
                    }

                }
                else
                {

                    if (_sliderIsOpen[i] == true)
                    {
                        onPanelLeave(panel);
                        _sliderIsOpen[i] = false;
                    }

                }

                i++;
            }
        }



        public void enableEditButton()
        {
            isEditEnabled = true;
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
