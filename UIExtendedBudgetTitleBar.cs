using System;

using ICities;
using ColossalFramework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BetterBudget
{
    class UIExtendedBudgetTitleBar : UIPanel
    {   
        private UIDragHandle _draghandler;
        private UISprite _icon;
        private UILabel _title;
        private UIButton _stickybutton;

        public UIExtendedBudgetPanel Parent { get; set; }

        /// <summary>
        /// Creates all objects. Call first.
        /// </summary>
        public void start()
        {
            // RoadOptionStraightPressed possible icon
            name = "TitleBar";
            setMode(Parent.mode); // isVisible
            canFocus = true;
            isInteractive = true;
            width = 385;
            height = 42;

            // Title
            bool customPanel = false;
            _title = AddUIComponent<UILabel>();
            _title.name = "Title Label";
            switch (Parent.name)
            {
                case "ExtendedPanelElectricity":
                    _title.localeID = "INFO_ELECTRICITY_TITLE"; break;
                case "ExtendedPanelWater":
                    _title.localeID = "INFO_WATER_WATERANDSEWAGE"; break;
                case "ExtendedPanelGarbage":
                    _title.localeID = "INFO_GARBAGE_TITLE"; break;
                case "ExtendedPanelHealth":
                    _title.localeID = "INFO_HEALTH_TITLE"; break;
                case "ExtendedPanelFireSafety":
                    _title.localeID = "INFO_FIRE_TITLE"; break;
                case "ExtendedPanelCrime":
                    _title.localeID = "INFO_CRIMERATE_TITLE"; break;
                case "ExtendedPanelEducation":
                    _title.localeID = "INFO_EDUCATION_TITLE"; break;
                case "ExtendedPanelParks":
                    _title.localeID = "INFO_ENTERTAINMENT_PARKS"; break;
                case "ExtendedPanelMonuments":
                    _title.localeID = "INFO_ENTERTAINMENT_BUILDINGS"; break;
                case "ExtendedPanelTransportBus":
                    _title.localeID = "INFO_PUBLICTRANSPORT_BUS"; break;
                case "ExtendedPanelTransportMetro":
                    _title.localeID = "INFO_PUBLICTRANSPORT_METRO"; break;
                case "ExtendedPanelTransportTrain":
                    _title.localeID = "INFO_PUBLICTRANSPORT_TRAIN"; break;
                case "ExtendedPanelTransportShip":
                    _title.localeID = "INFO_PUBLICTRANSPORT_SHIP"; break;
                case "ExtendedPanelTransportPlane":
                    _title.localeID = "INFO_PUBLICTRANSPORT_PLANE"; break;
                case "ExtendedPanelFreetime":
                    _title.localeID = "INFO_ENTERTAINMENT_TITLE"; break;
                case "ExtendedPanelTransport":
                    _title.localeID = "INFO_PUBLICTRANSPORT_TITLE"; break;
                default:
                    _title.text = Parent.name;
                    customPanel = true;
                    break;
            }

            String titletext = _title.text;
            if (!customPanel)
            {
                _title.localeID = "ECONOMY_BUDGET";
                titletext = titletext + " " + _title.text;
                _title.text = titletext;
            }
            _title.relativePosition = new Vector3(185-(_title.text.Length*4), 11);
            _title.textScale = 1.1f;

            // Drag Handler
            _draghandler = AddUIComponent<UIDragHandle>();
            _draghandler.name = "Draghandler";
            _draghandler.target = Parent;
            _draghandler.width = width;
            _draghandler.height = height;
            _draghandler.relativePosition = Vector3.zero;
            _draghandler.BringToFront();

            // Sticky Checkbox
            _stickybutton = AddUIComponent<UIButton>();
            _stickybutton.name = "Sticky Button";
            if (customPanel)
            {
                _stickybutton.normalBgSprite = "buttonclose";
                _stickybutton.pressedBgSprite = "buttonclosehover";
                _stickybutton.hoveredBgSprite = "buttonclosepressed";
                _stickybutton.focusedBgSprite = "buttonclosehover";
            }
            else
            {
                _stickybutton.normalBgSprite = "LocationMarkerActiveHovered";
                _stickybutton.pressedBgSprite = "LocationMarkerActivePressed";
                _stickybutton.hoveredBgSprite = "LocationMarkerActiveFocused";
                _stickybutton.focusedBgSprite = "LocationMarkerActivePressed";
            }
            _stickybutton.eventClick += toogleSticky;
            _stickybutton.size = new Vector2(36,36);
            _stickybutton.relativePosition = new Vector3(width-43,2);

            // Money Icon
            _icon = AddUIComponent<UISprite>();
            _icon.name = "Icon";
            _icon.relativePosition = new Vector3(4, 2);
            _icon.spriteName = "MoneyThumb";
            _icon.size = new Vector2(40, 40);
            _icon.eventClick += changeOpacity;
        }

        public void setMode(Mode mode)
        {
            if (mode != Mode.Default)
                isVisible = false;
            else
                isVisible = true;
        }

        /// <summary>
        /// Change the transparency of the object.
        /// </summary>
        /// <param name="component">Unused</param>
        /// <param name="eventParam">Unused</param>
        private void changeOpacity(UIComponent component, UIMouseEventParameter eventParam)
        {
            Parent.opacity  -= 0.15f;
            if (Parent.opacity < 0.4)
                Parent.opacity = 1f;
        }

        /// <summary>
        /// Sets the extended panel to sticky.
        /// </summary>
        /// <param name="component">Unused</param>
        /// <param name="eventParam">Unused</param>
        private void toogleSticky(UIComponent component, UIMouseEventParameter eventParam)
        {
            Parent.sticky = !Parent.sticky;
            updateButton();
        }

        /// <summary>
        /// Updates the button sprites based on if the panel is sticky or not. Closes the panel if there is no attached panel or the attached panel is closed.
        /// </summary>
        public void updateButton()
        {
            if (Parent.sticky)
            {
                _stickybutton.normalBgSprite = "buttonclose";
                _stickybutton.pressedBgSprite = "buttonclosehover";
                _stickybutton.hoveredBgSprite = "buttonclosepressed";
                _stickybutton.focusedBgSprite = "buttonclosehover";
            }
            else
            {
                _stickybutton.normalBgSprite = "LocationMarkerActiveHovered";
                _stickybutton.pressedBgSprite = "LocationMarkerActivePressed";
                _stickybutton.hoveredBgSprite = "LocationMarkerActiveFocused";
                _stickybutton.focusedBgSprite = "LocationMarkerActivePressed";
                if (Parent.attachedPanel == null || !Parent.attachedPanel.isVisible)
                    Parent.isVisible = false;
            }
        }
    }
}
