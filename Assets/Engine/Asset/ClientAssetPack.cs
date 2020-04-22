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
        public UI_Pack uiPack;

        /// <summary>
        /// A reference to a player pack
        /// </summary>
        public Player_Pack playerPack;
    }
}