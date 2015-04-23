using System;

using ICities;
using ColossalFramework.UI;
using UnityEngine;



namespace BetterBudget
{
    class UIExtendedBudgetPanel : UIPanel
    {
        private UIExtendedBudgetTitleBar _titlebar;
        private BBSliderContainer _sliderContainer;
        private BBPanelSettings _settings;

        public UIPanel attachedPanel { get; set; }
        public bool sticky { get; set; }
        public bool slim { get; set; }
        public bool isCustom { get; set; }
        public BetterBudget Parent { get; set; }

        /// <summary>
        /// Creates all objects based on the settings from last session.
        /// </summary>
        /// <param name="settings">All settings for this panel.</param>
        public void loadSettings(BBPanelSettings settings)
        {
            _settings = settings;

            relativePosition = new Vector3(settings.x, settings.y);
            canFocus = true;
            isInteractive = true;
            color = new Color32(255, 255, 255, 255);

            // Layout
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(0, 0, 0, 5);
            autoLayout = true;
            autoSize = true;
            autoFitChildrenHorizontally = true;
            autoFitChildrenVertically = true;

            // Titlebar
            _titlebar = AddUIComponent<UIExtendedBudgetTitleBar>();
            _titlebar.Parent = this;
            _titlebar.start();

            // sliderContainer
            _sliderContainer = AddUIComponent<BBSliderContainer>();
            _sliderContainer.Parent = this;
            _sliderContainer.start();

            // Settings
            slim = settings.slim;
            sticky = settings.sticky;
            updateSlim(false);
            if (slim)
                _sliderContainer.enableSlimMode();
            isVisible = settings.sticky;
            opacity = settings.opacity;
            _titlebar.updateButton();
        }

        public override void Update()
        {
            if (this.containsMouse)
            {
                Parent.updateExpenses();
            }
        }

        /// <summary>
        /// If the player hits a milestone and unlocks more buildings, the slider has to enabled. And vise versa (disable - which is not possible without cheating).
        /// </summary>
        /// <param name="component">The slider panel to change-</param>
        /// <param name="value">Set the comonent to enabled or disabled.</param>
        public void hitMilestone(UIComponent component, bool value)
        {
            UIPanel slider = _sliderContainer.Find<UIPanel>(component.name);
            slider.isEnabled = value;
            UISprite sliderSprite = slider.Find<UISprite>("Icon");
            if (value)
                sliderSprite.spriteName = sliderSprite.spriteName.Substring(0, sliderSprite.spriteName.Length - 8);
            else
                sliderSprite.spriteName = sliderSprite.spriteName + "Disabled";
        }


        
        /// <summary>
        /// Adds a slider to the panel. A copy of the slider will be created internally.
        /// </summary>
        /// <param name="sliderObject">The original slider panel.</param>
        public void addSlider(UIPanel sliderObject)
        {
            _sliderContainer.addSlider(sliderObject);
        }

        /// <summary>
        /// Adds a small invisible panel to help the layout look fancy.
        /// Only add after all sliders got added.
        /// </summary>
        public void addSpacingPanel()
        {
            _sliderContainer.addSpacingPanel();
        }

        /// <summary>
        /// Set if the player is able to see the extended panel. Only works if the panel is not sticky.
        /// </summary>
        /// <param name="visible">If the panel is visible or invisble.</param>
        public void setVisibility(Boolean visible)
        {
            if (!sticky)
                isVisible = visible;
            else if (visible)
                isVisible = true;
            _titlebar.updateButton();
        }
        
        /// <summary>
        /// Used to open and position the panel if opened by the attached information panel.
        /// </summary>
        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            if (!sticky && attachedPanel != null)
                relativePosition = new Vector3(attachedPanel.absolutePosition.x, attachedPanel.absolutePosition.y + attachedPanel.height + 5);
            if (!sticky && attachedPanel != null  && slim)
            {
                _sliderContainer.disableSlimMode();
                slim = false;
                updateSlim();
                relativePosition = new Vector3(attachedPanel.absolutePosition.x, attachedPanel.absolutePosition.y + attachedPanel.height + 5);
            }
        }

        /// <summary>
        /// Sets the extended panel into Slim Mode.
        /// </summary>
        /// <param name="updatePosition">Slim mode enabled or disabled. Default set to true.</param>
        public void updateSlim(bool updatePosition = true)
        {
            if (slim) // Slim Mode ON
            {
                // disable titlebar
                _titlebar.enableSlimMode();

                // disable background and titlebar
                backgroundSprite = null;

                // position extended panel
                if (updatePosition)
                    this.relativePosition = new Vector3(this.relativePosition.x + 14, this.relativePosition.y + 47);

            }
            else // Slim Mode OFF
            {
                // enable titlebar
                _titlebar.disableSlimMode();

                // enable background and titlebar
                backgroundSprite = "MenuPanel2";

                // position extended panel
                if (updatePosition)
                    this.relativePosition = new Vector3(this.relativePosition.x - 14, this.relativePosition.y - 47);

            }
        }

        /// <summary>
        /// Puts all important settings of the extended panel together.
        /// </summary>
        /// <returns>The save object.</returns>
        public BBPanelSettings getSettings()
        {
            _settings.name = this.name;
            _settings.x = this.relativePosition.x;
            _settings.y = this.relativePosition.y;
            _settings.opacity = this.opacity;
            _settings.sticky = this.sticky;
            _settings.slim = this.slim;
            _settings.isCustom = this.isCustom;
            return _settings;
        }


    }
}
