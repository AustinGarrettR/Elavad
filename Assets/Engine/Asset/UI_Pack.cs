using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Asset
{
    [CreateAssetMenu(fileName = "UI_Pack", menuName = "Data Packs/UI_Pack", order = 1)]
    public class UI_Pack : ScriptableObject
    { 
        public GameObject[] UI_Panels;
    }
}