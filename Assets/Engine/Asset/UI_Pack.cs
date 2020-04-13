using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Asset
{
    [CreateAssetMenu(fileName = "UIPack", menuName = "Data Packs/UIPack", order = 1)]
    public class UIPack : ScriptableObject
    {
        public GameObject[] UI_Panels;
    }
}