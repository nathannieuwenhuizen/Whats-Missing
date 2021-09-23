using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum MissingChangeEffect {
    none,
    scale,
    dissolve,
    animation
}
public interface IMissable {
    MissingChangeEffect MissingChangeEffect {get; }
    void onMissing();
    IEnumerator AnimateMissing();
    void onMissingFinish();
    void onAppearing();
    IEnumerator AnimateAppearing();
    void onAppearingFinish();

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
    void Grab();
    void Release();
    GameObject gameObject {get; }
    Rigidbody RigidBody { get; set; }
    float Mass {get; set;} 
    Vector3 HoldVelocity {get; set;}
    
}
public interface IInteractable {
    bool Focused {get; set;}
    GameObject Gameobject { get; }
    void Interact();
}

