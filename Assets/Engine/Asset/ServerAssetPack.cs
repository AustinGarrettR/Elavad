using UnityEngine;

namespace Engine.Asset
{
    /// <summary>
    /// An pack that contains references to assets
    /// </summary>
    [CreateAssetMenu(fileName = "ServerAssetPack", menuName = "Data Packs/ServerAssetPack", order = 1)]
    public class ServerAssetPack : ScriptableObject
    {
        /// <summary>
        /// A reference to a player pack
        /// </summary>
        public ServerPlayerPack serverPlayerPack;
    }
}