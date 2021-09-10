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
    string[] AlternativeWords {get; set; }
    bool Animated { get; set;}
    bool InSpace {get; }
    Transform Transform {get; }
    void SetChange(Change changeType);
    void RemoveChange(Change changeType);
}

public interface IPickable {
    Rigidbody RigidBody { get; }
    float Mass {get; set;}
    
}
public interface IInteractable {
    bool Focused {get; set;}
    GameObject Gameobject { get; }
    void Interact();
}

