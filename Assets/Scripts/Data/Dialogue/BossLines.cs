using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BossLines
{
    public static Line Laugh = new Line() {text = "Hahaha!", lineEffect = LineEffect.shake, duration = 2f};


    //intro
    public static Line Intro_1 = new Line() {
        text = "I'll make sure myself that you're done for", 
        lineEffect = LineEffect.none, duration = 2f};

    public static Line Intro_2 = new Line() {
        text = "Did you really thought you could prevent all of this? Starting over? ", 
        lineEffect = LineEffect.none, duration = 3f};
    public static Line Intro_3 = new Line() {
        text = "Now it is time for you to die. Time to join her too, Gregory", 
        lineEffect = LineEffect.none, duration = 3f};
    
    public static Line Intro_shield = new Line() {
        text = "It seems like you're protected", 
        lineEffect = LineEffect.none, duration = 3f};
    
    public static Line Intro_shield_2 = new Line() {
        text = "No worries, that shield won't hold for long!", 
        lineEffect = LineEffect.none, duration = 4f};
    
    //die
    public static Line Boss_Die = new Line() {
        text = "AAAAAAAAAAAAAAAAAAAAAAAAAH! THIS CAN'T... THIS CAN'T BE HAPPINING! YOU'RE SUPPOSED TO STAY STUCK WITH ME FOREVER!", 
        lineEffect = LineEffect.shake, duration = 6f};


    #region  transform lines
    public static Line Transform = new Line() {
        text = "Playtime is over", 
        lineEffect = LineEffect.none, duration = 3.5f,
        delay = 2f};

    public static Line NowBurn = new Line() {
        text = "Now burn!", 
        lineEffect = LineEffect.shake, duration = 1.6f,
        delay = 1f};

    public static Line Hahaha = new Line() {
        text = "Hahahah!!", 
        lineEffect = LineEffect.shake, duration = 2f,
        delay = 0.5f};

    #endregion

    //search
    public static Line Search() {
        List<Line> Lines = new List<Line>() {
            new Line() {text = "Where are you?", lineEffect = LineEffect.none, duration = 2f},
            new Line() {text = "Little little gregory, where are you", lineEffect = LineEffect.none, duration = 3f},
            new Line() {text = "You can't hide forever...", lineEffect = LineEffect.none, duration = 2f}
        };
        return Lines[Mathf.FloorToInt(Random.Range(0, Lines.Count))];
    }

    public static Line Noticing() {
        List<Line> Lines = new List<Line>() {
            new Line() {text = "Huh?", lineEffect = LineEffect.none, duration = 1f},
            new Line() {text = "I though I saw something...", lineEffect = LineEffect.none, duration = 2f}
        };
        return Lines[Mathf.FloorToInt(Random.Range(0, Lines.Count))];
    }

    public static Line Chase() {
        List<Line> Lines = new List<Line>() {
            new Line() {text = "There you are!", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "Found you!", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "BUUUUAAAAARGH!", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "Don't try to run away!", lineEffect = LineEffect.shake, duration = 2f}
        };
        return Lines[Mathf.FloorToInt(Random.Range(0, Lines.Count))];
    }

    public static Line Transforming = new Line() {text = "I'll squash you like a bug!", lineEffect = LineEffect.shake, duration = 2f};
    
    public static Line BeforeGrowth = new Line() {text = "ENOUGH!", lineEffect = LineEffect.shake, duration = 2f, delay = 1f};
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

    public static Line OnKillingPlayer() {
        List<Line> Lines = new List<Line>() {
            new Line() {text = "Time to join your daughter WUAHAHAHAHA!", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "Game over!", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "You'll never get her back.", lineEffect = LineEffect.shake, duration = 2f}
        };
        return Lines[Mathf.FloorToInt(Random.Range(0, Lines.Count))];
    }

    public static Line AskingForHintReaction() {
        List<Line> Lines = new List<Line>() {
            new Line() {text = "Do you really think you'll get a hint for defeating me? I was the one holding your hand. This was all my plan for you to release me!", lineEffect = LineEffect.none, duration = 5f},
            new Line() {text = "Just stop gregory, this time there is no hint to help you go through this.", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "Take the hint already, give up.", lineEffect = LineEffect.shake, duration = 2f},
            new Line() {text = "Haha, you really thought I was going to help you? Phathetic.", lineEffect = LineEffect.shake, duration = 3f}
        };
        return Lines[Mathf.FloorToInt(Random.Range(0, Lines.Count))];
    }

    public static List<Line> TimeHintAfterDying  = new List<Line>() {
        new Line() {text = "You’re running out of time!", lineEffect = LineEffect.none, duration = 3f, delay = 4f},
        new Line() {text = "Poor gregory),If only you had more time.", lineEffect = LineEffect.none, duration = 4f, delay = 4f},
    };
}
