using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpread : MonoBehaviour, ITriggerArea
{

    public delegate void EnterEvent();
    public static EnterEvent OnFireSpreadEnter;
    public static EnterEvent OnFireSpreadExit;

    public bool InsideArea { get; set; }

    public void OnAreaEnter(Player player)
    {
        OnFireSpreadEnter?.Invoke();
    }

    public void OnAreaExit(Player player)
    {
        OnFireSpreadExit?.Invoke();

    }
}
