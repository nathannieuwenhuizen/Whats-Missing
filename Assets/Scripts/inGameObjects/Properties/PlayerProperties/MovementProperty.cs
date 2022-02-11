using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementProperty : Property
{


    [SerializeField]
    private Room room;

    public override float CurrentScale { 
        get => room.Player.Movement.WalkMultiplier;
        set => room.Player.Movement.WalkMultiplier = value;
    }

    public override void OnEnlarge()
    {
        base.OnEnlarge();
        CurrentScale = largeScale;
    }

    public override void OnEnlargeRevert()
    {
        base.OnEnlargeRevert();
        CurrentScale = 1f;
    }
    public override void OnShrinking()
    {
        base.OnShrinking();
        CurrentScale = shrinkScale;
    }

    public override void OnShrinkRevert()
    {
        base.OnShrinkRevert();
        CurrentScale = 1f;
    }
    public override void OnFlipped()
    {
        base.OnFlipped();
        CurrentScale *= -1f;
    }

    public override void OnFlippingRevert()
    {
        base.OnFlippingRevert();
        CurrentScale *= -1f;
    }

    public override void OnMissing()
    {
        base.OnMissing();
        CurrentScale = 0;
    }

    public override void OnAppearing()
    {
        base.OnAppearing();
        CurrentScale = 1f;
    }

    private void Reset() {
        Word = "movement";
        AlternativeWords = new string[]{ "movements", "feet", "foot"};
    }

}
