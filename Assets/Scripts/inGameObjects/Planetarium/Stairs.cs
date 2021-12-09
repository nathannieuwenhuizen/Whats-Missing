using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : RoomObject
{
    public static OnMissingEvent OnStairsMissing;


    public override void OnMissing()
    {
        base.OnMissing();
        OnStairsMissing?.Invoke();
    }
    
    private void Reset() {
        Word = "stairs";
        AlternativeWords = new string[] {"stair", "steps", "staircase"};
    }

}
