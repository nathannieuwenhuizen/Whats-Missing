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


    protected MirrorChange currentChange;

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

    public override void AddChange(MirrorChange change) {
        currentChange = change;
        Debug.Log("adding change" + change);
        base.AddChange(change);        
    }
    public override void AddChange(IChange change) {
        currentChange = (MirrorChange)change;
        Debug.Log("adding mirror change" + change);
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
        base.OnShrinkRevert();
        if (Animated) onShockwave?.Invoke(currentChange.mirror.transform);
    }

    public override void OnEnlarge()
    {
        base.OnEnlarge();
        if (Animated) onShockwave?.Invoke(currentChange.mirror.transform);
    }

    public override void OnEnlargeRevert()
    {
        base.OnEnlargeRevert();
        if (Animated) onShockwave?.Invoke(currentChange.mirror.transform);
    }

    #endregion
}
