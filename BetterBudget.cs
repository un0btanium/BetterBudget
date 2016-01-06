using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

using ICities;
using ColossalFramework.UI;
using UnityEngine;

namespace BetterBudget
{
    class BetterBudget : UIPanel
    {
        // path of save files
        private static string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BetterBudgetMod";
        private static string fileNameSettings = "\\BetterBudgetSettings4.xml";

        // UIView (main container for UI stuff)
        UIView view;

        // information button (top left)
        public UIPanel _buttonContainer;

        // Custom Panel Creator panel
        BBCustomPanelCreator _customPanelCreator;

        // container with original slider panels
        private UIComponent _budgetContainer;
        private UIComponent _budgetTransportContainer;
        
        // list of all mod panels
        private List<UIExtendedBudgetPanel> _containerPanels;

        // required to open and close budget/expense panel to update our custom sliders every now and then
        private bool _budgetWindowManipulated = false;
        private bool _expenseUpdateActive = true;
        private int _expenseUpdateTimer = 1;
        private UIPanel _expensePanel;

        


        /// <summary>
        /// Creates all panels on game start
        /// </summary>
        public override void Start()
        {
            // check and create standard save file
            checkForSaveFile();

            // load panels and settings
            BBSettings saveData = loadSettings();

            // load UI and attach the BetterBudget object to its origin (top left)
            view = UIView.GetAView();
            this.transform.parent = view.transform;
            relativePosition = new Vector3(0, 0);

            //UIView.FullScreenContainer.EconomyPanel isVisible
            _expensePanel = view.FindUIComponent("FullScreenContainer").Find<UIPanel>("EconomyPanel");

            // detect opening of button container
            _buttonContainer = view.FindUIComponent<UIPanel>("InfoViewsPanel");

            // create list for extended panels
            _containerPanels = new List<UIExtendedBudgetPanel>();

            // Set visible, so child objects/panels are shown.
            isVisible = true;
            isInteractive = false; // maybe set to true?

            // Search for Slider Containers
            _budgetContainer = view.FindUIComponent<UIComponent>("ServicesBudgetContainer");
            _budgetTransportContainer = view.FindUIComponent<UIComponent>("SubServicesBudgetContainer");

            // open custom panel creator
            _customPanelCreator = AddUIComponent<BBCustomPanelCreator>();
            _customPanelCreator._main = this;
            _customPanelCreator.start(saveData.customPanelCreatorSettings);

            // set automatic expense updating and load panel settings
            _expenseUpdateActive = saveData.expenseUpdateActive;
            List<BBPanelSettings> settings = saveData.panelSettings;

            // create extended panels based on the saved information (settings)
            foreach (BBPanelSettings data in settings) {
                createExtendedPanel(data);
            }

            //Debug.Log("Better Budget Mod loaded! Have fun :)");
        }

        /// <summary>
        /// Creates extended panel.
        /// </summary>
        /// <param name="data">The settings the panel gets created with.</param>
        public void createExtendedPanel(BBPanelSettings data)
        {
            if (data.isCustom && !data.sticky) // delete closed custom panels (stops panels from summing up into infinity)
                return;
            // create game object
            GameObject go = new GameObject(data.name);
            // attach new extended panel to the gameobject
            UIExtendedBudgetPanel panel = go.AddComponent<UIExtendedBudgetPanel>();
            // attach extended panel to better budget panel/container
            panel.transform.parent = this.transform;
            // set parent to BetterBudget
            panel.Parent = this;
            // insert save data
            panel.load(data);
            // add information panel which opens and closes the extended panel simultaneously/in-sync (optional)
            if (data.informationPanel != null)
            {
                UIPanel informationPanel = view.FindUIComponent<UIPanel>(data.informationPanel);
                if (informationPanel != null)
                {
                    informationPanel.eventVisibilityChanged += onVisibilityChangeBetterBudget;
                    panel.attachedPanel = informationPanel;
                }
            }
            // adds slider to the extended panel (optional - but panel is useless without them)
            foreach (String sliderName in data.slider)
            {
                UIPanel originalSlider = findUIPanel(sliderName);
                if (originalSlider != null)
                {
                    originalSlider.eventIsEnabledChanged += panel.hitMilestone;
                    panel.addSlider(originalSlider);
                }
            }
            // finish extended panel by add spacing panel (adds some more space on the bottom of the panel. i am bad at the layout stuff)
            panel.addSpacingPanel();
            panel.setMode(data.mode, false);
            _containerPanels.Add(panel);
        }


        /// <summary>
        /// Loads last session's playermade changes and settings.
        /// </summary>
        private BBSettings loadSettings()
        {
            if (!File.Exists(filePath + fileNameSettings))
                return null;

            BBSettings settings;
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(BBSettings));
                reader = new StreamReader(filePath + fileNameSettings);
                settings = (BBSettings) serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return settings;
        }

        /// <summary>
        /// Saves playermade changes and settings.
        /// </summary>
        private void saveSettings()
        {
            System.IO.Directory.CreateDirectory(filePath);
            if (File.Exists(filePath + fileNameSettings))
                File.Delete(filePath + fileNameSettings);

            // request data to save
            BBSettings settings = new BBSettings();
            settings.expenseUpdateActive = _expenseUpdateActive;
            settings.customPanelCreatorSettings = _customPanelCreator.getSettings();
            foreach (UIExtendedBudgetPanel panel in _containerPanels)
            {
                settings.panelSettings.Add(panel.getSettings());
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
        /// Called upon hovering over an extended panel, making the main budget panel visible and allows the expense value to update.
        /// </summary>
        /// <param name="component">Extended Panel</param>
        /// <param name="eventParam"></param>
        internal void updateExpenses()
        {
            _expenseUpdateTimer = 0;
        }

        /// <summary>
        /// Makes main budget panel visible to allow expense values to update.
        /// The one minute cooldown prevents framerate drops.
        /// Complicated because it has to detect if the player opened the budget panel and budgets are getting updated allready.
        /// </summary>
        public override void Update()
        {
            // update every 60 seconds
            if (_expenseUpdateActive)
                _expenseUpdateTimer -= 1;
            if (_budgetWindowManipulated)
            {
                _expensePanel.isVisible = false;
                _expenseUpdateTimer = 3600;
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
        
        /// <summary>
        /// Based on visiblity of the information panel in the top left, moves the extended panels.
        /// Prevents overlapping of the panels with the information panel.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="visible">button container opened or closed</param>
        private void onVisibilityChangeBetterBudget(UIComponent component, bool visible)
        {
            closeAllWindows();
            for (int i = 0; i < _containerPanels.Count; i++)
            {
                if (_containerPanels[i].attachedPanel == component)
                    _containerPanels[i].setVisibility(visible);
            }
            
        }


        /// <summary>
        /// Closes all extended panels if they are not sticky.
        /// </summary>
        private void closeAllWindows()
        {
            for (int i = 0; i < _containerPanels.Count; i++ )
            {
                _containerPanels[i].setVisibility(false);
            }
        }

        /// <summary>
        /// Searchs for the panel with the budget slider in it.
        /// </summary>
        /// <param name="name">The panel name.</param>
        /// <returns></returns>
        private UIPanel findUIPanel(string name)
        {
            if (name.Equals("Bus") || name.Equals("Metro") || name.Equals("Train") || name.Equals("Ship") || name.Equals("Plane"))
            {
                return  _budgetTransportContainer.Find<UIPanel>(name);
            }
            else
            {
                return _budgetContainer.Find<UIPanel>(name);
            }
        }


        /// <summary>
        /// Deletes all method calls and game objects created.
        /// </summary>
        public void unload()
        {
            // save settings
            saveSettings();

            for (int i = 0; i < _containerPanels.Count; i++)
            {
                foreach (String sliderName in _containerPanels[i].getSettings().slider)
                {
                    UIPanel originalSlider = findUIPanel(sliderName);
                    if (originalSlider != null)
                    {
                        originalSlider.eventIsEnabledChanged -= _containerPanels[i].hitMilestone;
                        _containerPanels[i].addSlider(originalSlider);
                    }
                }
                if (_containerPanels[i].attachedPanel != null)
                    _containerPanels[i].attachedPanel.eventVisibilityChanged -= onVisibilityChangeBetterBudget;
                // delete all created Game Objects
                GameObject.Destroy(_containerPanels[i].gameObject);
            }
        }


        /// <summary>
        /// Checks if the save file with all settings exists. If not it creates the standard save file.
        /// </summary>
        private void checkForSaveFile()
        {
            if (File.Exists(filePath + fileNameSettings)) // everything is ok, savefile from last session exists
                return;
            System.IO.Directory.CreateDirectory(filePath); // create folder

            // create save file with standard extended panels
            string[] extendedPanelName = {
                                         "ExtendedPanelElectricity",
                                         "ExtendedPanelWater",
                                         "ExtendedPanelGarbage",
                                         "ExtendedPanelHealth",
                                         "ExtendedPanelFireSafety",
                                         "ExtendedPanelCrime",
                                         "ExtendedPanelEducation",
                                         "ExtendedPanelParks",
                                         "ExtendedPanelFreetime",
                                         "ExtendedPanelTransport"
                                };
            string[] informationPanelName = { 
                                               "(Library) ElectricityInfoViewPanel",
                                               "(Library) WaterInfoViewPanel",
                                               "(Library) GarbageInfoViewPanel",
                                               "(Library) HealthInfoViewPanel",
                                               "(Library) FireSafetyInfoViewPanel",
                                               "(Library) CrimeInfoViewPanel",
                                               "(Library) EducationInfoViewPanel",
                                               "(Library) HappinessInfoViewPanel",
                                               "(Library) EntertainmentInfoViewPanel",
                                               "(Library) PublicTransportInfoViewPanel"
                                            };
            string[][] sliderPanelTemplate = {
                                               new String[] {"Electricity"},
                                               new String[] {"WaterAndSewage"},
                                               new String[] {"Garbage"},
                                               new String[] {"Healthcare"},
                                               new String[] {"FireDepartment"},
                                               new String[] {"Police"},
                                               new String[] {"Education"},
                                               new String[] {"Beautification"},
                                               new String[] {"Beautification", "Monuments"},
                                               new String[] {"Bus","Metro","Train","Ship","Plane"}
                                           };

            List<BBPanelSettings> settings = new List<BBPanelSettings>();
            for (int i = 0; i < extendedPanelName.Length; i++)
            {
                BBPanelSettings panelSettings = new BBPanelSettings();
                panelSettings.name = extendedPanelName[i];
                panelSettings.slider = sliderPanelTemplate[i];
                panelSettings.informationPanel = informationPanelName[i];

                panelSettings.x = 0;
                panelSettings.y = 0;
                panelSettings.opacity = 1f;
                panelSettings.sticky = false;
                panelSettings.mode = Mode.Default;
                panelSettings.isCustom = false;
                panelSettings.isLeft = false;
                settings.Add(panelSettings);
            }

            BBPanelSettings customPanelSettings = new BBPanelSettings();
            customPanelSettings.x = 80;
            customPanelSettings.y = 80;

            BBSettings BBsettings = new BBSettings();
            BBsettings.panelSettings = settings;
            BBsettings.expenseUpdateActive = true;
            BBsettings.customPanelCreatorSettings = customPanelSettings;


            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(BBSettings));
                writer = new StreamWriter(filePath + fileNameSettings, false);
                serializer.Serialize(writer, BBsettings);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
    }

    public enum Mode
    {
        Default,
        Slim,
        noSlider
    }

}