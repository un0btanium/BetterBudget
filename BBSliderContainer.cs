using System;
using System.Collections.Generic;

using ICities;
using ColossalFramework.UI;
using UnityEngine;


namespace BetterBudget
{
    class BBSliderContainer : UIPanel
    {

        private UIPanel[] sliders;
        private int[] slimCloseTimer;

        public UIExtendedBudgetPanel Parent { get; set; }

        /// <summary>
        /// Creates all objects. Call first.
        /// </summary>
        public void start()
        {
            sliders = new UIPanel[0];
            slimCloseTimer = new int[0];
            name = "Slider Container";
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(14, 0, 0, 5);
            autoFitChildrenHorizontally = true;
            autoFitChildrenVertically = true;
        }

        /// <summary>
        /// Adds a copy of the slider panel object to the extended panel.
        /// </summary>
        /// <param name="originalSliderObject">The original slider panel to add.</param>
        public void addSlider(UIPanel originalSliderObject)
        {
            UIPanel sliderObject = duplicateUIPanel(originalSliderObject);
            sliderObject.transform.parent = this.transform;
            AttachUIComponent(sliderObject.gameObject);
            sliders = addElement<UIPanel>(sliderObject, sliders);
            slimCloseTimer = addElement<int>(0, slimCloseTimer);
        }

        /// <summary>
        /// Adds a spacing panel for layout purposes. Call after you added all slider objects.
        /// </summary>
        public void addSpacingPanel()
        {
            GameObject goSpacingPanel = new GameObject("SpacingPanel");
            UIPanel spacingPanel = goSpacingPanel.AddComponent<UIPanel>();
            spacingPanel.transform.parent = this.transform;
            spacingPanel.size = new Vector2(1, 7);
            AttachUIComponent(goSpacingPanel);
        }

        /// <summary>
        /// Duplicates an existing slider budget panel and changes UI.
        /// </summary>
        /// <param name="original">Original slider budget panel</param>
        /// <returns>A copy of the original slider object.</returns>
        private UIPanel duplicateUIPanel(UIPanel original)
        {
            UIPanel panel = InstanceManager.Instantiate(original);
            panel.width = 355;
            panel.name = panel.name.Substring(0, panel.name.Length - 7); // delete ' (Copy)' mark
            panel.eventMouseEnter += onPanelEnter;
            UIComponent icon = panel.Find("Icon");
            icon.relativePosition = new Vector3(3, icon.relativePosition.y);
            icon.eventClick += toggleSlimMode;
            icon.isInteractive = true;
            UILabel total = panel.Find<UILabel>("Total");
            total.relativePosition = new Vector3(265, 10);
            total.size = new Vector2(80, 18);
            total.textScale = 0.85f;
            UIComponent slider = panel.Find("Slider");
            slider.width = 210;
            slider.relativePosition = new Vector3(45, 10);
            UILabel percentage = panel.Find<UILabel>("Percentage");
            percentage.relativePosition = new Vector3(265, 10);
            percentage.size = new Vector2(80, 18);
            percentage.textScale = 0.9f;
            percentage.isVisible = false;
            return panel;
        }











        public void enableSlimMode(UIComponent component = null)
        {
            autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            for (int i = 0; i < slimCloseTimer.Length; i++)
            {
                slimCloseTimer[i] = 30;
            }
            foreach (UIPanel panel in sliders)
            {
                UILabel total = panel.Find<UILabel>("Total");
                UIComponent slider = panel.Find("Slider");
                UILabel percentage = panel.Find<UILabel>("Percentage");
                if (component!= null && component.name.Equals(panel.name)) // the slider panel the user clicked on
                {
                    slider.isVisible = true;
                    percentage.isVisible = true;
                    total.isVisible = false;
                    panel.width = 355;
                    total.relativePosition = new Vector3(265, 10);
                    percentage.relativePosition = new Vector3(265, 10);
                }
                else // every other slider panel
                {
                    slider.isVisible = false;
                    percentage.isVisible = false;
                    total.isVisible = true;
                    panel.width = 130;
                    percentage.relativePosition = new Vector3(45, 10);
                    total.relativePosition = new Vector3(45, 10);
                }
            }
        }

        public void disableSlimMode()
        {
            autoLayoutPadding = new RectOffset(14, 0, 0, 5);
            foreach (UIPanel panel in sliders)
            {
                panel.width = 355;
                UILabel total = panel.Find<UILabel>("Total");
                total.relativePosition = new Vector3(265, 10);
                UIComponent slider = panel.Find("Slider");
                slider.isVisible = true;
                UILabel percentage = panel.Find<UILabel>("Percentage");
                percentage.relativePosition = new Vector3(265, 10);
                percentage.isVisible = true;
            }
        }

        private void toggleSlimMode(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.Parent.slim = !this.Parent.slim;
            this.Parent.updateSlim();
            if (this.Parent.slim)
                enableSlimMode(component.parent);
            else
                disableSlimMode();
        }

        public void setSlimMode(bool enabled)
        {
            if (enabled)
            {
                this.Parent.updateSlim();
                enableSlimMode();
            }
        }

        /// <summary>
        /// Switches to percentage value and toggles on slim mode.
        /// </summary>
        /// <param name="component">Slider object</param>
        /// <param name="eventParam"></param>
        private void onPanelEnter(UIComponent component, UIMouseEventParameter eventParam)
        {
            UILabel total = component.Find<UILabel>("Total");
            total.isVisible = false;
            UILabel percentage = component.Find<UILabel>("Percentage");
            percentage.isVisible = true;
            if (Parent.slim)
            {
                for (int i = 0; i < sliders.Length; i++)
                {
                    if (sliders[i].name.Equals(component.name))
                    {
                        if (Parent.slim)
                            slimCloseTimer[i] = 30;
                        else
                            slimCloseTimer[i] = 1;
                        break;
                    }
                }
                component.width = 355;
                UIComponent slider = component.Find("Slider");
                slider.isVisible = true;
                percentage.relativePosition = new Vector3(265, 10);
                total.relativePosition = new Vector3(265, 10);
            }
        }

        private void closePanel(UIPanel panel)
        {
            UILabel total = panel.Find<UILabel>("Total");
            total.isVisible = true;
            UILabel percentage = panel.Find<UILabel>("Percentage");
            percentage.isVisible = false;
            if (Parent.slim)
            {
                panel.width = 130;
                percentage.relativePosition = new Vector3(45, 10);
                total.relativePosition = new Vector3(45, 10);
                UIComponent slider = panel.Find("Slider");
                slider.isVisible = false;
            }
        }



        public override void Update()
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (sliders[i].containsMouse)
                {
                    if (Parent.slim)
                        slimCloseTimer[i] = 30;
                    else
                        slimCloseTimer[i] = 1;
                    break;
                }
            }

            for(int i = 0; i < slimCloseTimer.Length; i++)
            {
                if (slimCloseTimer[i] > 0)
                    slimCloseTimer[i] -= 1;
                else if (slimCloseTimer[i] == 0)
                {
                    closePanel(sliders[i]);
                    slimCloseTimer[i] -= 1;
                }
            }
        }


        private T[] addElement<T>(T element, T[] array)
        {
            T[] tempArray = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                tempArray[i] = array[i];
            }
            tempArray[array.Length] = element;
            return tempArray;
        }

    }
}
