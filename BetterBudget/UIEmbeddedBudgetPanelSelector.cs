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
    class UIEmbeddedBudgetPanelSelector : UIPanel
    {

        private BetterBudget2 _main;
        private UIPanel _containerSelected;
        private UIPanel _containerUnselected;
        private UISprite _spriteArrow;
        private UISprite _icon;
        private UIButton _quitButton;
        private UIEmbeddedBudgetPanel _panel;


        public void initialize(BetterBudget2 main, UIEmbeddedBudgetPanel panel, List<UIPanel> sliderList)
        {
            this._main = main;
            this._panel = panel;
            this.transform.parent = panel.parent.transform;
            relativePosition = new Vector3(panel.parent.width, 0);
            int containerHeight = (int) Math.Ceiling(_main._spriteDictionary.Count / 4.0) * 35 + 5;
            size = new Vector2(400, 155 + containerHeight);
            canFocus = true;
            isInteractive = true;
            isVisible = true;
            backgroundSprite = "MenuPanel2";
            color = new Color32(255,255,255,255);

            UILabel label = AddUIComponent<UILabel>();
            label.text = "Budget Editor";
            label.transform.parent = this.transform;
            label.relativePosition = new Vector3((width / 2) - ((label.text.Length / 2) * 8), 12);
            label.name = "Title";

            // icon
            _icon = AddUIComponent<UISprite>();
            _icon.relativePosition = new Vector3(2, 2);
            _icon.spriteName = "MoneyThumb";
            _icon.name = "Icon";
            _icon.size = new Vector2(40, 40);


            // Drag Handler
            UIDragHandle draghandler = AddUIComponent<UIDragHandle>();
            draghandler.relativePosition = new Vector3(0, 0);
            draghandler.transform.parent = this.transform;
            draghandler.target = this;
            draghandler.name = "Drag Handler";
            draghandler.size = new Vector2(this.width, 41);


            // Quit Button
            _quitButton = AddUIComponent<UIButton>();
            _quitButton.name = "Quit Button";
            _quitButton.normalBgSprite = "buttonclose";
            _quitButton.pressedBgSprite = "buttonclosehover";
            _quitButton.hoveredBgSprite = "buttonclosepressed";
            _quitButton.focusedBgSprite = "buttonclosehover";
            _quitButton.eventClick += closePanel;
            _quitButton.size = new Vector2(36, 36);
            _quitButton.relativePosition = new Vector3(width - 43, 2);

            // container Unselected
            _containerUnselected = AddUIComponent<UIPanel>();
            _containerUnselected.name = "Slider Container Unselected";
            _containerUnselected.backgroundSprite = "GenericPanel";
            _containerUnselected.relativePosition = new Vector3(30, 60);
            _containerUnselected.size = new Vector2(145, containerHeight);
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
            _containerSelected.size = new Vector2(145, containerHeight);
            _containerSelected.autoLayout = true;
            _containerSelected.autoSize = false;
            _containerSelected.autoLayoutStart = LayoutStart.TopLeft;
            _containerSelected.autoLayoutDirection = LayoutDirection.Horizontal;
            _containerSelected.autoLayoutPadding = new RectOffset(5, 0, 5, 0);
            _containerSelected.wrapLayout = true;

            // budget selected icons
            foreach (UIPanel sliderPanel in sliderList)
            {
                UISprite sprite = _containerSelected.AddUIComponent<UISprite>();
                sprite.name = sliderPanel.name;
                sprite.spriteName = _main._spriteDictionary[sliderPanel.name];
                sprite.size = new Vector2(30, 30);
                sprite.eventClick += toggleSprite;
                sprite.isInteractive = true;
            }

            // budget unselected icons
            foreach (KeyValuePair<String, String> entry in _main._spriteDictionary)
            {
                if (_containerSelected.Find<UISprite>(entry.Key) == null)
                {
                    UISprite sprite = _containerUnselected.AddUIComponent<UISprite>();
                    sprite.name = entry.Key;
                    sprite.spriteName = entry.Value;
                    sprite.size = new Vector2(30, 30);
                    sprite.eventClick += toggleSprite;
                    sprite.isInteractive = true;
                }
            }

            // apply changes
            UIButton button = AddUIComponent<UIButton>();
            button.name = "Apply Button";
            button.size = new Vector2(340, 30);
            button.relativePosition = new Vector3(30, 75 + containerHeight);
            button.normalBgSprite = "ButtonMenu";
            button.focusedBgSprite = "ButtonMenuFocused";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.text = "Apply";
            button.textScale = 1.3f;
            button.eventClick += applyChanges;


            // apply changes
            UIButton button2 = AddUIComponent<UIButton>();
            button2.name = "Create Custom Panel Button";
            button2.size = new Vector2(340, 30);
            button2.relativePosition = new Vector3(30, 115 + containerHeight);
            button2.normalBgSprite = "ButtonMenu";
            button2.focusedBgSprite = "ButtonMenuFocused";
            button2.hoveredBgSprite = "ButtonMenuHovered";
            button2.pressedBgSprite = "ButtonMenuPressed";
            button2.text = "Create Custom Panel";
            button2.textScale = 1.3f;
            button2.eventClick += createCustomPanel;
        }

        private void createCustomPanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            UICustomBudgetPanel test = _main.AddUIComponent<UICustomBudgetPanel>();
            test.initialize(_main, new BBCustomSaveFile());
            deletePanel();
        }

        private void applyChanges(UIComponent component, UIMouseEventParameter eventParam)
        {
            String[] panelList = new String[_containerSelected.childCount];
            int i = 0;
            foreach (UISprite sprite in _containerSelected.components)
            {
                panelList[i++] = sprite.name;
            }
            _panel.setSliderPanel(panelList);
            deletePanel();
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

        private void closePanel(UIComponent component, UIMouseEventParameter eventParam)
        {
            deletePanel();
        }

        private void deletePanel()
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _panel.enableEditButton();
            parent.RemoveUIComponent(this);
            GameObject.Destroy(this.gameObject);
        }
    }
}
