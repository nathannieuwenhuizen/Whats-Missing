using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable/RoomLevel", order = 2)]
public class RoomLevel : ScriptableObject
{
    [Header("Prefab of the room")]
    public Room prefab;
    [Header("Room info")]
    public RoomData roomInfo;
    [Header("Potion info")]
    public RoomPotionData potionInfo;

}

[System.Serializable]
public class RoomData {
    public bool revealChangesAfterFinish = true;
    public bool doCutsceneOnRevealingChanges = true;
    public bool alwaysAnimate = false;
    public MirrorData[] questionMirror;
    public Change[] loadedChanges;
    [Header("Hints")]
    public float durationBeforeHint = 60f;
    public string hintText;
    public bool highLightAnswerLetter = true;
    public float durationBeforeHighlighting = 120f;
    public bool EventSenderActive = false;
}


[System.Serializable]
public class RoomPotionData {
    public bool missingPotionAvailable = false;
    public bool tooBigPotionAvailable = false;
    public bool tooSmallPotionAvailable = false;
}

[System.Serializable]
public class MirrorData {
    public string letters;
    public ChangeType changeType = ChangeType.missing;
    public bool isOn = false;
    public bool huzzleWords = true;
    public bool isQuestion = true;
    public bool isInteractable = true;

    //was nessecary because the scrptable objects kept changing in playmode
    public MirrorData Clone {
        get => new MirrorData() {letters = letters, changeType = changeType, isOn = isOn, huzzleWords = huzzleWords, isQuestion = isQuestion , isInteractable = isInteractable };
    }
}
