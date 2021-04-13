using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMissable {
    void onMissing();
    void onAppearing();
}


public interface IChangable : IMissable
{
    string Word {get; set; }
    bool animated { get; set;}
    bool InSpace {get; }
    Transform Transform {get; }
    void SetChange(ChangeType changeType);
    void RemoveChange(ChangeType changeType);
}

public interface IPickable {
    Rigidbody RigidBody { get; }
    float Mass {get; set;}
    
}
public interface IInteractable {
    bool Focused {get; set;}
    void Interact();
}

public class Change {
    public RoomTelevision television;
    public string word;
}