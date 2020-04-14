using UnityEngine;

namespace Engine.Asset
{
    [CreateAssetMenu(fileName = "ClientDataPack", menuName = "Data Packs/ClientDataPack", order = 1)]
    public class ClientAssetPack : ScriptableObject
    {
        public UI_Pack uiPack;
    }
}