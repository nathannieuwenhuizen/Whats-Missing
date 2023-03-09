using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BossLines
{
    public static Line Laugh = new Line() {text = "Hahaha!", lineEffect = LineEffect.shake, duration = 2f};
    public static Line BeforeGrowth = new Line() {text = "ENOUGH!!!", lineEffect = LineEffect.shake, duration = 2f};
    public static Line AfterGrowth = new Line() {text = "Now I am strong enough to get you inside that stupid shield", 
    lineEffect = LineEffect.none, duration = 4f};
    public static Line GetShardKickLine() {
        List<Line> Lines = new List<Line>() {
            new Line() {text = "Looking for this?", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "Try to get this one!", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "Look what I have found!", lineEffect = LineEffect.shake, duration = 2f}
        };
        return Lines[Mathf.FloorToInt(Random.Range(0, Lines.Count))];
    }
}