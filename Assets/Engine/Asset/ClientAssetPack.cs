using UnityEngine;

namespace Engine.Asset
{
    /// <summary>
    /// An pack that contains references to assets
    /// </summary>
    [CreateAssetMenu(fileName = "ClientDataPack", menuName = "Data Packs/ClientDataPack", order = 1)]
    public class ClientAssetPack : ScriptableObject
    {
        /// <summary>
        /// A reference to a UI Pack
        /// </summary>
        public UI_Pack uiPack;
    }
}