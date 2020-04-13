using UnityEngine;

namespace Engine.Asset
{
    [CreateAssetMenu(fileName = "ClientDataPack", menuName = "Data Packs/ClientDataPack", order = 1)]
    public class ClientDataPack : ScriptableObject
    {
        public UIPack uiPack;
    }
}