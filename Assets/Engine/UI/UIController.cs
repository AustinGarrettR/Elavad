using Engine.Logging;
using System;
using System.Collections.Generic;
using Unity.UIElements.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

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

    }
}