///<summary>
/// A change that is set inside the room. Holds on what word it is about and what tv has caused the change.
///</summary>
[System.Serializable]
public class Change {
    ///<summary>
    /// The television this change is connected to.
    ///</summary>
    public RoomTelevision television;
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
