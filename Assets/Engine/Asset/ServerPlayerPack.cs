using UnityEngine;

namespace Engine.Asset
{
    /// <summary>
    /// An object that stores references to player prefabs
    /// </summary>
    [CreateAssetMenu(fileName = "Server_Player_Pack", menuName = "Data Packs/Server_Player_Pack", order = 1)]
    public class ServerPlayerPack : ScriptableObject
    { 
        /// <summary>
        /// Reference of the server player prefab
        /// </summary>
        public GameObject playerPrefab;
    }
}