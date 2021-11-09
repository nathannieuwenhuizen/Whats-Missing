using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A non-physical object inside the room.
///</summary>
public abstract class Property : RoomEntity
{

    public delegate void OnPropertyToggle();

    public delegate void OnShockwave(Transform origin);
    public static event OnShockwave onShockwave;


    protected Change currentChange;

    public override void OnAppearing()
    {
        base.OnAppearing();
        if (Animated) onShockwave?.Invoke(currentChange.mirror.transform);
    }

    public override void OnMissing()
    {
        base.OnMissing();
        if (Animated) onShockwave?.Invoke(currentChange.mirror.transform);
    }

    public override void AddChange(Change change) {
        currentChange = change;
        base.AddChange(change);        
    }

    
    #region  shrinking/enlarging
    public override void OnShrinking()
    {
        base.OnShrinking();
        if (Animated) onShockwave?.Invoke(currentChange.mirror.transform);
    }

    public override void OnShrinkRevert()
    {
        if (Animated) onShockwave?.Invoke(currentChange.mirror.transform);
    }

    #endregion
}
