///<summary>
/// Interface for a roomobject that sends its own changes to the mirror
///</summary>

public interface IMissableSender {
    Room room {get; set;}
    public delegate void OnMissingEvent();
}