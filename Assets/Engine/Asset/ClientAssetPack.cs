using UnityEngine;

namespace Engine.Asset
{
    /// <summary>
    /// An pack that contains references to assets
    /// </summary>
    [CreateAssetMenu(fileName = "ClientAssetPack", menuName = "Data Packs/ClientAssetPack", order = 1)]
    public class ClientAssetPack : ScriptableObject
    {
        /// <summary>
        /// A reference to a UI Pack
        /// </summary>
        public UIPack uiPack;

        /// <summary>
        /// A reference to a player pack
        /// </summary>
        public ClientPlayerPack clientPlayerPack;
    }
}