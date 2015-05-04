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
        public bool isCustom { get; set; }
        public BetterBudget Parent { get; set; }
        public Mode mode { get; set; }
        public bool isLeft { get; set; }

        /// <summary>
        /// Creates all objects based on the settings from last session.
        /// </summary>
        /// <param name="settings">All settings for this panel.</param>
        public void load(BBPanelSettings settings)
        {
            _settings = settings;

            // Settings
            canFocus = true;
            isInteractive = true;
            color = new Color32(255, 255, 255, 255);
            sticky = settings.sticky;
            isVisible = settings.sticky;
            opacity = settings.opacity;
            isLeft = settings.isLeft;
            isCustom = settings.isCustom;

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

            relativePosition = new Vector3(settings.x, settings.y);
        }

        /// <summary>
        /// Updates expenses if the mouse hovers over the panel.
        /// </summary>
        public override void Update()
        {
            if (this.containsMouse)
            {
                Parent.updateExpenses();
            }
            if (attachedPanel != null && isVisible)
            {
                if (Parent._buttonContainer.isVisible)
                {
                    if (mode == Mode.Default && relativePosition.x >= -10 && relativePosition.x <= 5 && relativePosition.y >= attachedPanel.absolutePosition.y + attachedPanel.height && relativePosition.y <= attachedPanel.absolutePosition.y + attachedPanel.height + 7)
                        relativePosition = new Vector3(81, relativePosition.y);
                    else if (mode != Mode.Default && relativePosition.x >= 4 && relativePosition.x <= 19 && relativePosition.y >= attachedPanel.absolutePosition.y + attachedPanel.height + 45 && relativePosition.y <= attachedPanel.absolutePosition.y + attachedPanel.height + 54)
                        relativePosition = new Vector3(95, relativePosition.y);
                }
                else if (!Parent._buttonContainer.isVisible)
                {
                    if (mode == Mode.Default && relativePosition.x >= 80 && relativePosition.x <= 85 && relativePosition.y >= attachedPanel.absolutePosition.y + attachedPanel.height && relativePosition.y <= attachedPanel.absolutePosition.y + attachedPanel.height + 7)
                        relativePosition = new Vector3(0, relativePosition.y);
                    else if (mode != Mode.Default && relativePosition.x >= 94 && relativePosition.x <= 99 && relativePosition.y >= attachedPanel.absolutePosition.y + attachedPanel.height + 45 && relativePosition.y <= attachedPanel.absolutePosition.y + attachedPanel.height + 54)
                        relativePosition = new Vector3(14, relativePosition.y);
                }
            }
        }

        /// <summary>
        /// Toggles through the modes and sets all objects.
        /// Called from BBSliderContainer object
        /// </summary>
        internal void toggleMode()
        {
            if (mode == Mode.Default)
                setMode(Mode.Slim);
            else if (mode == Mode.Slim)
                setMode(Mode.noSlider);
            else if (mode == Mode.noSlider)
                setMode(Mode.Default);
        }

        /// <summary>
        /// Sets the objects to their correct position, size, ect to the new mode.
        /// </summary>
        /// <param name="newMode">The mode the extended panel shall switch to.</param>
        /// <param name="updatePosition">Repositions the panel. Use false upon loading.</param>
        public void setMode(Mode newMode, bool updatePosition = true)
        {
            this.mode = newMode;
            _titlebar.setMode(mode);
            _sliderContainer.setMode(mode);

            if (mode == Mode.Slim)
            {
                // disable background
                backgroundSprite = null;
                // position extended panel
                if (updatePosition)
                {
                    if (isLeft)
                    {
                        this.relativePosition = new Vector3(this.relativePosition.x + 239, this.relativePosition.y + 47);
                    }
                    else
                    {
                        this.relativePosition = new Vector3(this.relativePosition.x + 14, this.relativePosition.y + 47);
                    }
                }
            }
            else if (mode == Mode.noSlider)
            {
                // disable background
                backgroundSprite = null;
            }
            else if (mode == Mode.Default)
            {
                // enable background
                backgroundSprite = "MenuPanel2";
                // position extended panel
                if (updatePosition)
                {
                    if (isLeft)
                    {
                        this.relativePosition = new Vector3(this.relativePosition.x - 239, this.relativePosition.y - 47);
                    }
                    else
                    {
                        this.relativePosition = new Vector3(this.relativePosition.x - 14, this.relativePosition.y - 47);
                    }
                }
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
            if (relativePosition.x == 0 && relativePosition.y == 0 && attachedPanel != null)
                relativePosition = new Vector3(attachedPanel.absolutePosition.x, attachedPanel.absolutePosition.y + attachedPanel.height + 5);
            if (!sticky)
                isVisible = visible;
            _titlebar.updateButton();
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
            _settings.isCustom = this.isCustom;
            _settings.mode = this.mode;
            _settings.isLeft = this.isLeft;
            return _settings;
        }
    }
}
