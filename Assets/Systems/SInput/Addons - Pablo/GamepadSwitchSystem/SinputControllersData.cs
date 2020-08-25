using UnityEngine;
using SinputSystems;
using System;

[CreateAssetMenu(menuName = "SinputControllersData")]
public class SinputControllersData : ScriptableObject
{
    public Controller[] controllers = null;

    [Serializable]
    public struct Controller
    {
        public CommonMapping[] mappings;
    }
}
