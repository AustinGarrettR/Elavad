using UnityEngine;

namespace Engine.Asset
{
    /// <summary>
    /// An object that stores references to UI panels
    /// </summary>
    [CreateAssetMenu(fileName = "UI_Pack", menuName = "Data Packs/UI_Pack", order = 1)]
    public class UIPack : ScriptableObject
    { 
        /// <summary>
        /// Reference of UI panel prefabs
        /// </summary>
        public GameObject[] UI_Panels;
    }
}