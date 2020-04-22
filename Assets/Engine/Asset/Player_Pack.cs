using UnityEngine;

namespace Engine.Asset
{
    /// <summary>
    /// An object that stores references to player prefabs
    /// </summary>
    [CreateAssetMenu(fileName = "Player_Pack", menuName = "Data Packs/Player_Pack", order = 1)]
    public class Player_Pack : ScriptableObject
    { 
        /// <summary>
        /// Reference of the main player prefab
        /// </summary>
        public GameObject playerPrefab;
    }
}