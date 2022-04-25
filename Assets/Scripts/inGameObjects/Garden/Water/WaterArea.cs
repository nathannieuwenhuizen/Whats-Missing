using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterArea : MonoBehaviour, ITriggerArea
{
    public bool InsideArea { get; set;} = false;
    public static bool IN_WATER = false;
    public static Transform WATER_TRANSFORM;
    private void OnDisable() {
        IN_WATER = false;
    }
    public void OnAreaEnter(Player player)
    {
        IN_WATER = true;
        WATER_TRANSFORM = transform;
    }

    public void OnAreaExit(Player player)
    {
        IN_WATER = false;
        WATER_TRANSFORM = null;
    }

}
