using UnityEngine;

namespace Engine.Asset
{
    /// <summary>
    /// An object that stores references to player prefabs
    /// </summary>
    [CreateAssetMenu(fileName = "Client_Player_Pack", menuName = "Data Packs/Client_Player_Pack", order = 1)]
    public class ClientPlayerPack : ScriptableObject
    { 
        /// <summary>
        /// Reference of the main player prefab
        /// </summary>
        public GameObject playerPrefab;
    }
}