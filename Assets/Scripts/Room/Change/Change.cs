public interface IChange {
    public string Word {get; set; }
    public ChangeType ChangeType {get; set; }
    public bool Active {get; set; }
    public string[] AltarnativeWords {get; set; }
}

///<summary>
/// A change that is set inside the room. Holds on what word it is about and what mirror has caused the change.
///</summary>
[System.Serializable]
public class Change : IChange {
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

    public ChangeType changeType;

    public bool Active { get => active; set => active = value; }
    public string[] AltarnativeWords { get => alternativeWords; set => alternativeWords = value; }
    string IChange.Word { get => word; set => word = value; }
    ChangeType IChange.ChangeType { get => changeType; set => changeType = value; }

    public static string GetChangeTypeText(ChangeType changeType) {
        string result = "";
        switch (changeType) {
            case ChangeType.missing:
                result = "missing";
                break;
            case ChangeType.flipped:
                result = "flipped";
                break;
            case ChangeType.tooBig:
                result = "altered";
                break;
            case ChangeType.tooSmall:
                result = "altered";
                break;
        }
        return result;
    }
}

[System.Serializable]
public class MirrorChange : Change {
    ///<summary>
    /// The mirror this change is connected to.
    ///</summary>
    public Mirror mirror;
}

public enum ChangeCausation {
    environment,
    potion,
    boss
}

[System.Serializable]
public class RoomChange : Change {
    public IChangable roomObject;
    public ChangeCausation changeCausation = ChangeCausation.environment;
}
