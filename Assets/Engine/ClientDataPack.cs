using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClientDataPack", menuName = "Data Packs/ClientDataPack", order = 1)]
public class ClientDataPack : ScriptableObject
{
    public GameObject[] UI_Panels;
}
