using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable/RoomLevel", order = 2)]
public class RoomLevel : ScriptableObject
{
    [Header("Prefab of the room")]
    public Room prefab;
    [Header("Room info (can be null)")]
    public RoomData roomInfo;
}

[System.Serializable]
public class RoomData {
    public bool revealChangesAfterFinish = true;
    public MirrorData questionMirror;
    public MirrorData[] changeMirror;
    public float durationBeforeHint = 60f;
    public string hintText;
}

[System.Serializable]
public class MirrorData {
    public string letters;
    [Range(-1,1)]
    public int roomIndexoffset = 0;
    public ChangeType changeType = ChangeType.missing;
}
