using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

using ICities;
using ColossalFramework.UI;
using UnityEngine;


namespace BetterBudget
{
    class BetterBudget2 : UIPanel {

        // path of save files
        private static string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BetterBudgetMod";
        private static string fileNameSettings = "\\BetterBudgetSettings6.xml";

        // UIView (main container for UI stuff)
        public UIView view;

        // budget container with all budget slider panels
        private UIPanel _budgetPanel;

        // required to open and close budget/expense panel to update our custom sliders every now and then
        private bool _budgetWindowManipulated = false;
        private bool _expenseUpdateActive = true;
        private int _expenseUpdateTimer = 1;
        private UIPanel _expensePanel;

        private Dictionary<String, UIPanel> _sliderList;
        public Dictionary<String, String> _spriteDictionary;

        private List<UIEmbeddedBudgetPanel> _embeddedBudgetPanelList;
        private List<UICustomBudgetPanel> _customBudgetPanelList;

        

        public override void Start()
        {
            //base.Start();

            // load UI and attach the BetterBudget object to its origin (top left)
            view = UIView.GetAView();
            this.transform.parent = view.transform;
            relativePosition = new Vector3(0, 0);
            size = new Vector2(0, 0);
            this.name = "BetterBudgetMod";

            isVisible = true;
            isInteractive = false;

            //UIView.FullScreenContainer.EconomyPanel isVisible
            _expensePanel = view.FindUIComponent("FullScreenContainer").Find<UIPanel>("EconomyPanel");
            _expensePanel.absolutePosition = new Vector3(_expensePanel.absolutePosition.x, 5000, _expensePanel.absolutePosition.y); // fix/workaround for economy budget window flickering

            // Create list for slider panels
            _sliderList = new Dictionary<String, UIPanel>();
            _spriteDictionary = new Dictionary<String, String>();

            // Find budget container with all slider panels
            _budgetPanel = view.FindUIComponent<UIPanel>("FullScreenContainer").Find<UIPanel>("EconomyPanel").Find<UITabContainer>("EconomyContainer").Find<UIPanel>("Budget");

            // Get all slider panels out of the budget container
            foreach (UIComponent servicesContainer in _budgetPanel.components)
            {
                foreach (UIComponent sliderPanel in servicesContainer.components)
                {
                    // DEBUG
                    //foreach (UIComponent component in sliderPanel.components)
                    //{
                    //    Debug.Log(component.name, component);
                    //}

                    String spriteName;
                    UISprite originalSprite = sliderPanel.Find<UISprite>("Icon");

                    if (originalSprite.spriteName.Contains("Disabled"))
                        spriteName = originalSprite.spriteName.Substring(0, originalSprite.spriteName.Length - 8);
                    else
                        spriteName = originalSprite.spriteName;

                    _spriteDictionary.Add(sliderPanel.name, spriteName);
                    _sliderList.Add(sliderPanel.name, (UIPanel) sliderPanel);

                    sliderPanel.eventIsEnabledChanged += hitMilestone;
                }
            }

            // Get all service panels (info views)
            List<UIPanel> infoViewPanelList = new List<UIPanel>();
            _embeddedBudgetPanelList = new List<UIEmbeddedBudgetPanel>();

            foreach (UIPanel panel in view.GetComponentsInChildren<UIPanel>())  
            {
                if (panel.name.Contains("(Library)") && panel.name.Contains("InfoViewPanel"))
                {
                    infoViewPanelList.Add(panel);
                }
            }

            // create mod panels and add them to the service info view panels
            foreach (UIPanel infoViewPanel in infoViewPanelList) {
                UIEmbeddedBudgetPanel embeddedPanel = infoViewPanel.AddUIComponent<UIEmbeddedBudgetPanel>();
                embeddedPanel.initialize(this, infoViewPanel);
                _embeddedBudgetPanelList.Add(embeddedPanel);
            }

            // load settings (complex to be expansion viable)
            BBSettings settings = loadSettings();
            if (settings.embeddedPanelSettings.Count > 0)
            {
                Dictionary<String, List<String>> map = new Dictionary<String, List<String>>();
                foreach(BBEmbeddedSaveFile savefile in settings.embeddedPanelSettings) {
                    map.Add(savefile.infoViewPanelName, savefile.budgetSliderNameList);
                }

                foreach (UIEmbeddedBudgetPanel panel in _embeddedBudgetPanelList)
                {
                    String panelName = panel.getInfoViewPanel().name;
                    if (map.ContainsKey(panelName)) {
                        if (map[panelName].Count > 0)
                        {
                            panel.setSliderPanel(map[panelName].ToArray());
                        }
                        panel.settings.budgetSliderNameList = map[panelName]; // ensures that settings are saved across sessions even if some expansions and their budgets may be disabled
                    }
                }
            }

            _customBudgetPanelList = new List<UICustomBudgetPanel>();

            if (settings.customPanelSettings.Count > 0)
            {
                foreach (BBCustomSaveFile savefile in settings.customPanelSettings)
                {
                    UICustomBudgetPanel panel = AddUIComponent<UICustomBudgetPanel>();
                    panel.initialize(this, savefile);
                    panel.settings.budgetSliderNameList = savefile.budgetSliderNameList; // ensures that settings are saved across sessions even if some expansions and their budgets may be disabled
                }
            }

        }

        private void hitMilestone(UIComponent component, bool value)
        {
            foreach (UIEmbeddedBudgetPanel panel in _embeddedBudgetPanelList)
                panel.hitMilestone(component, value);
            foreach (UICustomBudgetPanel panel in _customBudgetPanelList)
                panel.hitMilestone(component, value);
        }

        public UIPanel getSliderPanel(String name)
        {
            foreach(KeyValuePair<String, UIPanel> entry in _sliderList) {
                if (entry.Value.name.Equals(name))
                    return entry.Value;
            }
            return null;
        }

        public override void Update()
        {
            // update every 60 seconds
            if (_expenseUpdateActive)
                _expenseUpdateTimer -= 1;
            if (_budgetWindowManipulated)
            {
                _expensePanel.isVisible = false;
                _expenseUpdateTimer = 1800;
                _budgetWindowManipulated = false;
            }
            else if (_expenseUpdateTimer <= 0 && !_expensePanel.isVisible)
            {
                _expensePanel.isVisible = true;
                _budgetWindowManipulated = true;
            }

            // toggle on/off
            if (Input.GetKeyDown(KeyCode.B) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
            {
                _expenseUpdateActive = !_expenseUpdateActive;
                _expenseUpdateTimer = 60;
            }
            base.Update();
        }

        public void unload()
        {
            saveSettings();

            foreach (KeyValuePair<String, UIPanel> entry in _sliderList)
            {
                entry.Value.eventIsEnabledChanged -= hitMilestone;
            }

            foreach (UIEmbeddedBudgetPanel panel in _embeddedBudgetPanelList)
            {
                GameObject.Destroy(panel.gameObject);
            } 
            
            foreach (UICustomBudgetPanel panel in _customBudgetPanelList)
            {
                GameObject.Destroy(panel.gameObject);
            }
        }

        /// <summary>
        /// Saves playermade changes and settings.
        /// </summary>
        public void saveSettings()
        {
            System.IO.Directory.CreateDirectory(filePath);
            if (File.Exists(filePath + fileNameSettings))
                File.Delete(filePath + fileNameSettings);

            BBSettings settings = new BBSettings();
            settings.expanseUpdateActive = _expenseUpdateActive;
            foreach (UIEmbeddedBudgetPanel panel in _embeddedBudgetPanelList)
            {
                BBEmbeddedSaveFile savefile = panel.getSettings();
                settings.embeddedPanelSettings.Add(savefile);
            }
            foreach (UICustomBudgetPanel panel in _customBudgetPanelList)
            {
                BBCustomSaveFile savefile = panel.getSettings();
                settings.customPanelSettings.Add(savefile);
            }


            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(BBSettings));
                writer = new StreamWriter(filePath + fileNameSettings, false);
                serializer.Serialize(writer, settings);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Loads last session's playermade changes and settings.
        /// </summary>
        private BBSettings loadSettings()
        {
            if (!File.Exists(filePath + fileNameSettings))
                createSaveFile();

            BBSettings settings;

            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(BBSettings));
                reader = new StreamReader(filePath + fileNameSettings);
                settings = (BBSettings)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            _expenseUpdateActive = settings.expanseUpdateActive;
            return settings;
        }

        private void createSaveFile()
        {
            System.IO.Directory.CreateDirectory(filePath); // create folder

            BBSettings settings = new BBSettings();
            settings.expanseUpdateActive = true;
            
            List<String> infoViewPanelNameList = new List<string>();
            List<String[]> budgetSliderNameList = new List<String[]>();

            infoViewPanelNameList.Add("(Library) HealthInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Healthcare"});
            infoViewPanelNameList.Add("(Library) OutsideConnectionsInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) CrimeInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Police"});
            infoViewPanelNameList.Add("(Library) PopulationInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) PollutionInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) NoisePollutionInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) WindInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) LevelsInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) TrafficInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) LandValueInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) NaturalResourcesInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) PublicTransportInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Bus", "Metro", "Train", "Ship", "Plane", "Taxi", "Tram"});
            infoViewPanelNameList.Add("(Library) ElectricityInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Electricity"});
            infoViewPanelNameList.Add("(Library) HappinessInfoViewPanel");
            budgetSliderNameList.Add(new String[] {});
            infoViewPanelNameList.Add("(Library) EducationInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Education"});
            infoViewPanelNameList.Add("(Library) WaterInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"WaterAndSewage"});
            infoViewPanelNameList.Add("(Library) HeatingInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Electricity"});
            infoViewPanelNameList.Add("(Library) GarbageInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Garbage"});
            infoViewPanelNameList.Add("(Library) FireSafetyInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"FireDepartment"});
            infoViewPanelNameList.Add("(Library) EntertainmentInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"Beautification", "Monuments"});
            infoViewPanelNameList.Add("(Library) RoadMaintenanceInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"RoadMaintenance"});
            infoViewPanelNameList.Add("(Library) RoadSnowInfoViewPanel");
            budgetSliderNameList.Add(new String[] {"RoadMaintenance"});

            for (int i = 0; i < infoViewPanelNameList.Count; i++)
            {
                BBEmbeddedSaveFile savefile = new BBEmbeddedSaveFile();
                savefile.infoViewPanelName = infoViewPanelNameList[i];
                List<String> sliderNameList = new List<String>();
                foreach (String name in budgetSliderNameList[i])
                {
                    sliderNameList.Add(name);
                }
                savefile.budgetSliderNameList = sliderNameList;
                settings.embeddedPanelSettings.Add(savefile);
            }

            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(BBSettings));
                writer = new StreamWriter(filePath + fileNameSettings, false);
                serializer.Serialize(writer, settings);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        internal void addCustomPanel(UICustomBudgetPanel panel)
        {
            _customBudgetPanelList.Add(panel);
        }

        internal void removeCustomPanel(UICustomBudgetPanel panel)
        {
            _customBudgetPanelList.Remove(panel);
        }
    }

    public enum Mode
    {
        Default,
        SliderOnly,
        Slim,
        PlusMinus
    }
}
