using Engine.Logging;
using System;
using System.Collections.Generic;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Engine.Utility;

namespace Engine.UI
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class UIController : MonoBehaviour
    {
        internal PanelRenderer panelRenderer;

        private void Awake()
        {

            panelRenderer = gameObject.GetComponent<PanelRenderer>();
            panelRenderer.postUxmlReload = panelLoaded;
            panelRenderer.RecreateUIFromUxml();
        }

        private IEnumerable<UnityEngine.Object> panelLoaded()
        {
            onPanelLoaded();
            return null;
        }

        public PanelRenderer GetPanelRenderer()
        {
            return panelRenderer;
        }

        public abstract void onPanelLoaded();

        //Show/Hide panel
        public void setPanelVisibility(bool visible)
        {
            panelRenderer.visualTree.visible = visible;
        }

        //Select an element from the tree
        public U getElement<U>(String name) where U : VisualElement
        {
            U element = panelRenderer.visualTree.Query<U>(name);
            if (element == null)
                Log.LogError("UI Controller called for element '" + name + "' which does not exist in '"+panelRenderer.uxml.name+"' UXML.");

            return element;
        }

        //Register text field to always have an uppercase letter for string
        public void RegisterFirstLetterUpperCaseFormat(TextField field)
        {
            field.RegisterValueChangedCallback(TextChangedForceFormatUsername);
        }

        //Register text field as a password (with support of placeholders)
        public void RegisterPlaceholderWithPasswordFormat(TextField field)
        {
            field.RegisterValueChangedCallback(TextChangedForceFormatPassword);
        }

        //Register text field to use placeholders (Requires tooltip field to be set to the placeholder text)
        public void RegisterPlaceholderTextField(TextField field)
        {
            field.SetValueWithoutNotify(field.tooltip);
            field.userData = true;
            if (field.ClassListContains("placeholder") == false)
                field.AddToClassList("placeholder");
            field.RegisterCallback<FocusInEvent>(placeholderGainedFocusEvent);
            field.RegisterCallback<FocusOutEvent>(placeholderLostFocusEvent);
        }

        public string getPlaceholderTextFieldRealValue(TextField field)
        {
            if ((bool)field.userData)
                return "";
            else
                return field.value;
        }

        private void placeholderGainedFocusEvent(FocusInEvent focusEvent)
        {
            TextField field = (TextField)focusEvent.target;
            if ((bool)field.userData)
            {
                field.userData = false;
                if (field.ClassListContains("placeholder"))
                    field.RemoveFromClassList("placeholder");
                field.SetValueWithoutNotify("");
            }
        }
        private void placeholderLostFocusEvent(FocusOutEvent focusEvent)
        {

            TextField field = (TextField)focusEvent.target;
            if (field.value == null || field.value.Length == 0)
            {
                field.SetValueWithoutNotify(field.tooltip);
                field.userData = true;
                if (field.ClassListContains("placeholder") == false)
                    field.AddToClassList("placeholder");
            }
        }

        private void TextChangedForceFormatUsername(ChangeEvent<string> changeEvent)
        {
            string textValue = changeEvent.newValue;
            ((TextField)changeEvent.target).SetValueWithoutNotify(StringFormatter.FirstCharToUpper(textValue));
        }

        private void TextChangedForceFormatPassword(ChangeEvent<string> changeEvent)
        {
            string textValue = changeEvent.newValue;
            TextField field = (TextField)changeEvent.target;
            if (textValue != null && textValue.Length > 0)
            {
                field.isPasswordField = true;
            }
            else
            {
                field.isPasswordField = false;
            }
        }
    }
}