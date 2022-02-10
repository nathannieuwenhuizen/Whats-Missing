///<summary>
/// A change that is set inside the room. Holds on what word it is about and what mirror has caused the change.
///</summary>
[System.Serializable]
public class Change {
    ///<summary>
    /// The mirror this change is connected to.
    ///</summary>
    public Mirror mirror;
    ///<summary>
    /// The word that this change represents
    ///</summary>
    public string word;
    ///<summary>
    /// Alternative words that can be also used as the same word. Otherwise synonyms.
    ///</summary>
    public string[] alternativeWords;
    ///<summary>
    /// Whether the change is active or not inside the room.
    ///</summary>
    public bool active = false;

    ///<summary>
    /// Whether the change is connected to this room or to the room next/previous of the current room.
    ///</summary>
    public int roomIndexOffset = 0;
}

[System.Serializable]
public class RoomObjectChange {
    public RoomObject roomObject;
    public ChangeType changeType;
}
