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
    class BBCustomPanelCreator : UIPanel
    {
        private bool isOpen;
        public BetterBudget _main { get; set; }
        private UIPanel _containerSelected;
        private UIPanel _containerUnselected;
        private UISprite _spriteArrow;
        private UITextField _textfield;
        private UISprite _icon;
        private string[] spriteIcon = { "InfoIconElectricity", "InfoIconWater", "InfoIconGarbage", "ToolbarIconHealthcare", "InfoIconFireSafety", "ToolbarIconPolice", "InfoIconEducation", "ToolbarIconBeautification", "ToolbarIconMonuments", "SubBarPublicTransportBus", "SubBarPublicTransportTrain", "SubBarPublicTransportMetro", "SubBarPublicTransportShip", "SubBarPublicTransportPlane" };
        private string[] serviceSlider = { "Electricity",
                                         "WaterAndSewage",
                                         "Garbage",
                                         "Healthcare",
                                         "FireDepartment",
                                         "Police",
                                         "Education",
                                         "Beautification",
                                         "Monuments",
                                         "Bus",
                                         "Metro",
                                         "Train",
                                         "Ship",
                                         "Plane"
                                       }; 

        /// <summary>
        /// Creates all objects based on the given settings. Call this first.
        /// </summary>
        /// <param name="settings">The settings (only uses x and y position).</param>
        public void start(BBPanelSettings settings)
        {
            relativePosition = new Vector3(settings.x, settings.y);
            canFocus = true;
            isInteractive = true;
            isVisible = true;
            backgroundSprite = "MenuPanel2";
            color = new Color32(255, 255, 255, 255);
            size = new Vector2(400,310);
            name = "Better Budget - Custom Panel Creator";

            
            UILabel label = AddUIComponent<UILabel>();
            label.text = "Custom Panel Creator";
            label.transform.parent = this.transform;
            label.relativePosition = new Vector3((width/2) - ((label.text.Length/2)*8), 12);
            label.name = "Title Label";
            UIDragHandle draghandler = AddUIComponent<UIDragHandle>();
            draghandler.relativePosition = new Vector3(0,0);
            draghandler.transform.parent = this.transform;
            draghandler.target = this;
            draghandler.name = "Drag Handler";
            // icon
            _icon = AddUIComponent<UISprite>();
            _icon.relativePosition = new Vector3(2, 2);
            _icon.spriteName = "MoneyThumb";
            _icon.size = new Vector2(40, 40);
            _icon.eventClick += togglePanel;
            _icon.BringToFront();

            // container Unselected
            _containerUnselected = AddUIComponent<UIPanel>();
            _containerUnselected.name = "Slider Container Unselected";
            _containerUnselected.backgroundSprite = "GenericPanel";
            _containerUnselected.relativePosition = new Vector3(30, 60);
            _containerUnselected.size = new Vector2(145, 145);
            _containerUnselected.autoLayout = true;
            _containerUnselected.autoSize = false;
            _containerUnselected.autoLayoutStart = LayoutStart.TopLeft;
            _containerUnselected.autoLayoutDirection = LayoutDirection.Horizontal;
            _containerUnselected.autoLayoutPadding = new RectOffset(5, 0, 5, 0);
            _containerUnselected.wrapLayout = true;

            // Arrow
            _spriteArrow = AddUIComponent<UISprite>();
            _spriteArrow.name = "Arrow Sprite";
            _spriteArrow.spriteName = "ArrowRight";
            _spriteArrow.relativePosition = new Vector3(163, 100);
            _spriteArrow.size = new Vector3(64, 64);

            // container Selected
            _containerSelected = AddUIComponent<UIPanel>();
            _containerSelected.name = "Slider Container Selected";
            _containerSelected.backgroundSprite = "GenericPanel";
            _containerSelected.relativePosition = new Vector3(225, 60);
            _containerSelected.size = new Vector2(145, 145);
            _containerSelected.autoLayout = true;
            _containerSelected.autoSize = false;
            _containerSelected.autoLayoutStart = LayoutStart.TopLeft;
            _containerSelected.autoLayoutDirection = LayoutDirection.Horizontal;
            _containerSelected.autoLayoutPadding = new RectOffset(5, 0, 5, 0);
            _containerSelected.wrapLayout = true;

            // budget icons
            for(int i = 0; i < spriteIcon.Length; i++)
            {
                UISprite sprite = _containerUnselected.AddUIComponent<UISprite>();
                sprite.name = serviceSlider[i];
                sprite.spriteName = spriteIcon[i];
                sprite.size = new Vector2(30, 30);
                sprite.eventClick += toggleSprite;
                sprite.isInteractive = true;
            }

            // text field
            _textfield = AddUIComponent<UITextField>();
            _textfield.name = "Text Field Name";
            _textfield.size = new Vector2(340,25);
            _textfield.relativePosition = new Vector3(30, 225);
            _textfield.text = "Panel Name";
            _textfield.enabled = true;
            _textfield.builtinKeyNavigation = true;
            _textfield.isInteractive = true;
            _textfield.readOnly = false;
            _textfield.submitOnFocusLost = true;
            _textfield.horizontalAlignment = UIHorizontalAlignment.Left;
            _textfield.selectionSprite = "EmptySprite";
            _textfield.selectionBackgroundColor = new Color32(0, 171, 234, 255);
            _textfield.normalBgSprite = "TextFieldPanel";
            _textfield.textColor = new Color32(174, 197, 211, 255);
            _textfield.disabledTextColor = new Color32(254, 254, 254, 255);
            _textfield.textScale = 1.3f;
            _textfield.opacity = 1;
            _textfield.color = new Color32(58, 88, 104, 255);
            _textfield.disabledColor = new Color32(254, 254, 254, 255);

            // create button
            UIButton button = AddUIComponent<UIButton>();
            button.name = "Create Button";
            button.size = new Vector2(340, 30);
            button.relativePosition = new Vector3(30, 265);
            button.normalBgSprite = "ButtonMenu";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.text = "Create Custom Panel";
            button.textScale = 1.3f;
            button.eventClick += createPanel;

            isOpen = false;
            updateState();
        }

        /// <summary>
        /// Creates the custom panel based on the selected sliders.
        /// </summary>
        /// <param name="component">Unused</param>
        /// <param name="eventParam">Unused</param>
        private void createPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            BBPanelSettings settings = new BBPanelSettings();
            settings.name = _textfield.text;
            settings.x = 400;
            settings.y = 200;
            settings.opacity = 1;
            settings.sticky = true;
            settings.slim = false;

            if (_containerSelected.childCount == 0)
                return;


            string[] sliderNames = new string[_containerSelected.childCount];
            for (int i = 0; i < _containerSelected.childCount; i++)
            {
                sliderNames[i] = _containerSelected.components[i].name;
            }

            settings.slider = sliderNames;
            _main.createExtendedPanel(settings, true);
        }

        /// <summary>
        /// Switch the clicked budget icon to the selected or unselected box.
        /// </summary>
        /// <param name="component">The clicked budget icon.</param>
        /// <param name="eventParam">Unused</param>
        private void toggleSprite(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (_containerUnselected.Find<UISprite>(component.name) != null)
            {
                _containerSelected.AttachUIComponent(component.gameObject);
                _containerUnselected.RemoveUIComponent(component);
                _spriteArrow.spriteName = "ArrowRightFocused";
                _spriteArrow.relativePosition = new Vector3(163, 100);
            }
            else if (_containerSelected.Find<UISprite>(component.name) != null)
            {
                _containerUnselected.AttachUIComponent(component.gameObject);
                _containerSelected.RemoveUIComponent(component);
                _spriteArrow.spriteName = "ArrowLeftFocused";
                _spriteArrow.relativePosition = new Vector3(175, 100);
            }
        }

        /// <summary>
        /// Opens and closes the panel. If closed only the icon is visible to open it again.
        /// </summary>
        /// <param name="component">Unused</param>
        /// <param name="eventParam">Unused</param>
        private void togglePanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            isOpen = !isOpen;
            updateState();
        }

        /// <summary>
        /// Opens and closes the panel. If closed only the icon is visible to open it again.
        /// </summary>
        private void updateState() {
            if (isOpen)
            {
                foreach (UIComponent component in this.components)
                {
                    component.isVisible = true;
                }
                backgroundSprite = "MenuPanel2";
                size = new Vector2(400, 310);
            }
            else
            {
                foreach (UIComponent component in this.components)
                {
                    component.isVisible = false;
                }
                _icon.isVisible = true;
                backgroundSprite = null;
                size = new Vector2(40, 40);
            }
        }

        /// <summary>
        /// Saves all important settings for next session.
        /// </summary>
        /// <returns>Settings.</returns>
        public BBPanelSettings getSettings() {
            BBPanelSettings settings = new BBPanelSettings();
            settings.x = this.relativePosition.x;
            settings.y = this.relativePosition.y;
            return settings;
        }

    }
}
