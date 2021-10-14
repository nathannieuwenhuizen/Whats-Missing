///<summary>
/// A change that is set inside the room. Holds on what word it is about and what tv has caused the change.
///</summary>
[System.Serializable]
public class Change {
    public RoomTelevision television;
    public string word;
    public string[] alternativeWords;
    public bool active = false;
}
