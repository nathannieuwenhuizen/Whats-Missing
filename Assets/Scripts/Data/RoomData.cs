using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoomLevel
{
    [Header("Prefab of the room")]
    public Room prefab;
    [Header("Room info (can be null)")]
    public RoomData roomInfo;
}


[CreateAssetMenu(menuName = "Scriptable/RoomLevel", order = 2)]
public class RoomData : ScriptableObject {
    public bool revealChangesAfterFinish = true;
    public MirrorData questionMirror;
    public MirrorData[] changeMirror;
}

[System.Serializable]
public class MirrorData {
    public string letters;
    [Range(-1,1)]
    public int roomIndexoffset = 0;
}
