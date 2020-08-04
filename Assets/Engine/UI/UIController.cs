using Engine.Logging;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Engine.Utility;

namespace Engine.UI
{
    /// <summary>
    /// UI Panel base class
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIController : MonoBehaviour
    {
        /// <summary>
        /// Internal panel renderer
        /// </summary>
        internal UIDocument uiDocument;

        /// <summary>
        /// Called on awake
        /// </summary>
        private void Awake()
        {
            uiDocument = gameObject.GetComponent<UIDocument>();
            uiDocumentLoaded();
           // uiDocument.postUxmlReload = uiDocumentLoaded;
           // uiDocument.RecreateUIFromUxml();
        }

        /// <summary>
        /// Intermediate method on panel loaded
        /// </summary>
        /// <returns></returns>
        private IEnumerable<UnityEngine.Object> uiDocumentLoaded()
        {
            onUIDocumentLoaded();
            return null;
        }

        /// <summary>
        /// Returns the panel renderer
        /// </summary>
        /// <returns></returns>
        public UIDocument GetUIDocument()
        {
            return uiDocument;
        }

        /// <summary>
        /// Called when the panel is done loading
        /// </summary>
        public abstract void onUIDocumentLoaded();

        /// <summary>
        /// Show/Hide panel
        /// </summary>
        /// <param name="visible">If the panel should be made visible</param>
        public void SetDocumentVisibility(bool visible)
        {
         //   uiDocument.visualTreeAsset.visible = visible;
        }

        /// <summary>
        /// Select an element from the tree
        /// </summary>
        /// <typeparam name="U">Visual Element Type</typeparam>
        /// <param name="name">Name of the element</param>
        /// <returns></returns>
        public U getElement<U>(String name) where U : VisualElement
        {
            
              U element = uiDocument.rootVisualElement.Query<U>(name);
              if (element == null)
                  Log.LogError("UI Controller called for element '" + name + "' which does not exist in '"+uiDocument.visualTreeAsset.name+"' UXML.");

              return element;
            return null;
        }

        /// <summary>
        /// Register text field to always have an uppercase letter for string
        /// </summary>
        /// <param name="field">The text field</param>
        public void RegisterFirstLetterUpperCaseFormat(TextField field)
        {
            field.RegisterValueChangedCallback(TextChangedForceFormatUsername);
        }

        /// <summary>
        /// Register text field as a password (with support of placeholders)
        /// </summary>
        /// <param name="field">The text field</param>
        public void RegisterPlaceholderWithPasswordFormat(TextField field)
        {
            field.RegisterValueChangedCallback(TextChangedForceFormatPassword);
        }

        /// <summary>
        /// Register text field to use placeholders (Requires tooltip field to be set to the placeholder text)
        /// </summary>
        /// <param name="field">The text field</param>
        public void RegisterPlaceholderTextField(TextField field)
        {
            field.SetValueWithoutNotify(field.tooltip);
            field.userData = true;
            if (field.ClassListContains("placeholder") == false)
                field.AddToClassList("placeholder");
            field.RegisterCallback<FocusInEvent>(placeholderGainedFocusEvent);
            field.RegisterCallback<FocusOutEvent>(placeholderLostFocusEvent);
        }

        /// <summary>
        /// Returns the input value after placeholder processing
        /// </summary>
        /// <param name="field">The text field</param>
        /// <returns></returns>
        public string getPlaceholderTextFieldRealValue(TextField field)
        {
            if ((bool)field.userData)
                return "";
            else
                return field.value;
        }

        /// <summary>
        /// Called when the placeholder field gains focus
        /// </summary>
        /// <param name="focusEvent">The focus event</param>
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

        /// <summary>
        /// Called when the placeholder loses focus
        /// </summary>
        /// <param name="focusEvent">The focus event</param>
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

        /// <summary>
        /// Formats username text field
        /// </summary>
        /// <param name="changeEvent">The change event</param>
        private void TextChangedForceFormatUsername(ChangeEvent<string> changeEvent)
        {
            string textValue = changeEvent.newValue;
            ((TextField)changeEvent.target).SetValueWithoutNotify(StringFormatter.FirstCharToUpper(textValue));
        }

        /// <summary>
        /// Formats password text field with masking support
        /// </summary>
        /// <param name="changeEvent">The change event</param>
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