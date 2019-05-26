using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

using ICities;
using ColossalFramework.UI;
using UnityEngine;
using System.Reflection;


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
        private int _expenseUpdateTimerCounter = 0;
        private UIPanel _expensePanel;
        private Vector3 _tempExpensePanelPosition;

        private Dictionary<String, BudgetItem> _budgetItems;
        private Dictionary<String, UIPanel> _originalBudgetPanels;
        public Dictionary<String, String> _spriteDictionary;

        private List<UIEmbeddedBudgetPanel> _embeddedBudgetPanelList;
        private List<UICustomBudgetPanel> _customBudgetPanelList;

        private Dictionary<String, ServiceInfo> _serviceInfos;

        private Dictionary<String, List<UIPanel>> _allBudgetPanels;

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
            _tempExpensePanelPosition = _expensePanel.absolutePosition;

            // Create dictionaries
            _budgetItems = new Dictionary<String, BudgetItem>();
            _originalBudgetPanels = new Dictionary<String, UIPanel>();
            _spriteDictionary = new Dictionary<String, String>();
            _serviceInfos = new Dictionary<String, ServiceInfo>();
            _allBudgetPanels = new Dictionary<String, List<UIPanel>>();

            // Find budget container with all slider panels
            _budgetPanel = view.FindUIComponent<UIPanel>("FullScreenContainer").Find<UIPanel>("EconomyPanel").Find<UITabContainer>("EconomyContainer").Find<UIPanel>("Budget");

            // Get all slider panels out of the budget container
            foreach (UIComponent servicesContainer in _budgetPanel.components)
            {
                foreach (UIComponent sliderPanel in servicesContainer.components)
                {

                    if (sliderPanel.GetComponents<BudgetItem>().Length == 0) { continue; }

                    UISprite originalSprite = sliderPanel.Find<UISprite>("Icon");

                    // saves the service sprites for the customizer
                    String spriteName;
                    if (originalSprite.spriteName.Contains("Disabled"))
                        spriteName = originalSprite.spriteName.Substring(0, originalSprite.spriteName.Length - 8);
                    else
                        spriteName = originalSprite.spriteName;

                    
                    // Add value changed event to update all other panels
                    UISlider sliderDay = sliderPanel.Find<UISlider>("DaySlider");
                    UISlider sliderNight = sliderPanel.Find<UISlider>("NightSlider");
                    sliderDay.eventValueChanged += copySliderValuesDay;
                    sliderNight.eventValueChanged += copySliderValuesNight;

                    // Add event that unlocks the components
                    sliderPanel.eventIsEnabledChanged += hitMilestone;


                    // save service names and sprites as well as the budget panels (to later remove the milestone event)
                    _spriteDictionary.Add(sliderPanel.name, spriteName);
                    _originalBudgetPanels.Add(sliderPanel.name, (UIPanel)sliderPanel);

                    // Add to budget panel list (to copy values on value changed from one another)
                    if (!_allBudgetPanels.ContainsKey(sliderPanel.name))
                    {
                        _allBudgetPanels.Add(sliderPanel.name, new List<UIPanel>());
                    }
                    _allBudgetPanels[sliderPanel.name].Add((UIPanel)sliderPanel);


                    // Add to original BudgetItem list to create copies of them
                    BudgetItem budgetItem = sliderPanel.GetComponents<BudgetItem>()[0];
                    _budgetItems.Add(sliderPanel.name, budgetItem);
                    
                    // Copy data binding values from BudgetItem (to be later used to initialize the copies)
                    BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                    ItemClass.Service service = (ItemClass.Service)(budgetItem.GetType().GetField("m_Service", bindFlags).GetValue(budgetItem));
                    ItemClass.SubService subService = (ItemClass.SubService)(budgetItem.GetType().GetField("m_SubService", bindFlags).GetValue(budgetItem));
                    Int32 budgetExpensePollIndex = (Int32)(budgetItem.GetType().GetField("m_BudgetExpensePollIndex", bindFlags).GetValue(budgetItem));
                    //Debug.Log(sliderPanel.name + " " + service + " " + subService + " " + budgetExpensePollIndex);
                    _serviceInfos.Add(sliderPanel.name, new ServiceInfo(sliderPanel.name, service, subService, budgetExpensePollIndex));

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
                }
            }

        }



        private void hitMilestone(UIComponent component, bool value)
        {
            if (!_allBudgetPanels.ContainsKey(component.name))
            {
                return;
            }
            
            foreach (UIPanel panel in _allBudgetPanels[component.name]) {
                panel.isEnabled = value;
                UISprite icon = panel.Find<UISprite>("Icon");
                if (value)
                {
                    if (icon.spriteName.Contains("Disabled"))
                    {
                        icon.spriteName = icon.spriteName.Substring(0, icon.spriteName.Length - 8);
                    }
                }
                else
                {
                    if (!icon.spriteName.Contains("Disabled"))
                    {
                        icon.spriteName = icon.spriteName + "Disabled";
                    }
                }
            }
        }



        private void copySliderValuesDay(UIComponent slider, float value)
        {
            foreach (UIPanel sliderPanel in _allBudgetPanels[slider.parent.name])
            {   
                //if (sliderPanel == slider)
                //if (sliderPanel.Equals(slider)) 
                if (sliderPanel.GetInstanceID() == slider.GetInstanceID())
                {
                    continue; // skip itself
                }

                UISlider sliderDay = sliderPanel.Find<UISlider>("DaySlider");
                sliderDay.value = value;
            }
        }



        private void copySliderValuesNight(UIComponent slider, float value)
        {
            foreach (UIPanel sliderPanel in _allBudgetPanels[slider.parent.name])
            {
                if (sliderPanel.GetInstanceID() == slider.GetInstanceID())
                {
                    continue; // skip itself
                }

                UISlider sliderNight = sliderPanel.Find<UISlider>("NightSlider");
                sliderNight.value = value; 
            }
        }



        public BudgetItem getBudgetCopy(String name)
        {
            foreach (KeyValuePair<String, BudgetItem> entry in _budgetItems)
            {
                if (entry.Value.name.Equals(name))
                {
                    BudgetItem budgetItemOriginal = entry.Value;
                    BudgetItem budgetItemCopy = InstanceManager.Instantiate(budgetItemOriginal);

                    _serviceInfos[name].initBudgetItem(budgetItemCopy);

                    UIPanel panelOriginal = (UIPanel)budgetItemOriginal.component;
                    UIPanel panelCopy = (UIPanel)budgetItemCopy.component;


                    UISlider sliderDay = panelCopy.Find<UISlider>("DaySlider");
                    UISlider sliderNight = panelCopy.Find<UISlider>("NightSlider");

                    sliderDay.eventValueChanged += copySliderValuesDay;
                    sliderNight.eventValueChanged += copySliderValuesNight;

                    _allBudgetPanels[name].Add(panelCopy);

                    return budgetItemCopy;
                }
            }
            return null;
        }

        public void removeBudgetCopy(UIPanel panel)
        {
            _allBudgetPanels[panel.name].Remove(panel);
            UISlider sliderDay = panel.Find<UISlider>("DaySlider");
            UISlider sliderNight = panel.Find<UISlider>("NightSlider");

            sliderDay.eventValueChanged -= copySliderValuesDay;
            sliderNight.eventValueChanged -= copySliderValuesNight;
        }



        public override void Update()
        {
            // update every 60 seconds
            if (_expenseUpdateActive)
                _expenseUpdateTimer -= 1;
            if (_budgetWindowManipulated)
            {
                _expensePanel.isVisible = false;
                if (_expenseUpdateTimerCounter > 0) {
                    _expenseUpdateTimer = 240;
                    _expenseUpdateTimerCounter -= 1;
                } else {
                    _expenseUpdateTimer = 1800;
                }
                _budgetWindowManipulated = false;
                _expensePanel.absolutePosition = _tempExpensePanelPosition;
            }
            else if (_expenseUpdateTimer <= 0 && !_expensePanel.isVisible)
            {
                _tempExpensePanelPosition = _expensePanel.absolutePosition;
                _expensePanel.absolutePosition = new Vector3(10000, 10000, _expensePanel.absolutePosition.z);
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


            foreach (KeyValuePair<String, List<UIPanel>> entry in _allBudgetPanels) {
                foreach (UIPanel panel in entry.Value) {
                    UISlider sliderDay = panel.Find<UISlider>("DaySlider");
                    UISlider sliderNight = panel.Find<UISlider>("NightSlider");

                    sliderDay.eventValueChanged -= copySliderValuesDay;
                    sliderNight.eventValueChanged -= copySliderValuesNight;
                }
            }

            //_allBudgetPanels[panel.name].Remove(panel);

            foreach (KeyValuePair<String, UIPanel> entry in _originalBudgetPanels)
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
            budgetSliderNameList.Add(new String[] {"Bus", "Metro", "Train", "Ship", "Plane", "Taxi", "Tram", "Monorail", "CableCar", "Tours"});
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

        internal void hoverOverPanelEnded()
        {
            _expenseUpdateTimerCounter = 5;
            if (_expenseUpdateTimer > 240 || _expenseUpdateTimer < 0)
            {
                _expenseUpdateTimer = 240;
            }
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
