using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The type of the missing/unmissing animation
///</summary>
public enum MissingChangeEffect {
    none,
    scale,
    dissolve,
    animation
}

///<summary>
/// An entity that can be missing.
///</summary>
public interface IMissable {

    MissingChangeEffect MissingChangeEffect {get; }
    ///<summary>
    /// Fires when the object starts missing. Here it calls the animation if the object is set to animate.
    ///</summary>
    void onMissing();
    ///<summary>
    /// The animation of the object when it goes missing.
    ///</summary>
    IEnumerator AnimateMissing();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually set.
    ///</summary>
    void onMissingFinish();
    ///<summary>
    /// Fires when the object starts un-missing/appearing. Here it calls the animation if the object is set to animate.
    ///</summary>
    void onAppearing();
    ///<summary>
    /// The animation of the object when it goes un-missing/appearing.
    ///</summary>
    IEnumerator AnimateAppearing();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually rmeoved.
    ///</summary>
    void onAppearingFinish();

}

///<summary>
/// The entity can actually change.
///</summary>
public interface IChangable : IMissable
{
    ///<summary>
    /// The word the tv searches for when the player checks the answer or sets a change.
    ///</summary>
    string Word {get; set; }
    ///<summary>
    /// Array of alternate words. Example (color, colour)
    ///</summary>
    string[] AlternativeWords {get; set; }
    ///<summary>
    /// If the entity sets it changes to be animated.
    ///</summary>
    bool Animated { get; set;}
    ///<summary>
    /// I don't know what this is...sorry
    ///</summary>
    bool InSpace {get; }
    ///<summary>
    /// The transform of the entity.
    ///</summary>
    Transform Transform {get; }
    ///<summary>
    /// Sets the change of the entity.
    ///</summary>
    void SetChange(Change changeType);
    ///<summary>
    /// Removes the change of the entity.
    ///</summary>
    void RemoveChange(Change changeType);
}

public interface IPickable {
    void Grab(Rigidbody connectedRB);
    void Release();
    GameObject gameObject {get; }
    Rigidbody RigidBody { get; set; }
    RigidBodyInfo RigidBodyInfo { get;set;}    
}
public interface IInteractable {
    bool Focused {get; set;}
    GameObject Gameobject { get; }
    void Interact();
}

