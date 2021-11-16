using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The type of the missing/unmissing animation
///</summary>
public enum MissingChangeEffect {
    none,
    scale,
    dissolve
}

///<summary>
/// An entity that can be missing.
///</summary>
public interface IMissable {

    MissingChangeEffect MissingChangeEffect {get; }
    ///<summary>
    /// Fires when the object starts missing. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnMissing();
    ///<summary>
    /// The animation of the object when it goes missing.
    ///</summary>
    IEnumerator AnimateMissing();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually set.
    ///</summary>
    void OnMissingFinish();
    ///<summary>
    /// Fires when the object starts un-missing/appearing. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnAppearing();
    ///<summary>
    /// The animation of the object when it goes un-missing/appearing.
    ///</summary>
    IEnumerator AnimateAppearing();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually rmeoved.
    ///</summary>
    void OnAppearingFinish();
}

///<summary>
/// An entity that can be shrunk.
///</summary>
public interface IShrinkable {
    ///<summary>
    /// Fires when the object starts shrinking. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnShrinking();
    ///<summary>
    /// The animation of the object when it shrinks.
    ///</summary>
    IEnumerator AnimateShrinking();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually set.
    ///</summary>
    void OnShrinkingFinish();
    ///<summary>
    /// Fires when the object starts unshrinking. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnShrinkRevert();
    ///<summary>
    /// The animation of the object when it goes unshrinking.
    ///</summary>
    IEnumerator AnimateShrinkRevert();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually rmeoved.
    ///</summary>
    void OnShrinkingRevertFinish();
}

///<summary>
/// An entity that can be enlarged.
///</summary>
public interface IEnlargable {
    ///<summary>
    /// Fires when the object starts enlarging. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnEnlarge();
    ///<summary>
    /// The animation of the object when it enlarges.
    ///</summary>
    IEnumerator AnimateEnlarging();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually set.
    ///</summary>
    void OnEnlargingFinish();
    ///<summary>
    /// Fires when the object starts becomming normal from being too big. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnEnlargeRevert();
    ///<summary>
    /// The animation of the object when it goes back to normal from being too big.
    ///</summary>
    IEnumerator AnimateEnlargeRevert();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually rmeoved.
    ///</summary>
    void OnEnlargeRevertFinish();
}

///<summary>
/// An entity that can be flipped.
///</summary>
public interface IFlippable {
    ///<summary>
    /// Fires when the object starts flipping. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnFlipped();
    ///<summary>
    /// The animation of the object when it flips.
    ///</summary>
    IEnumerator AnimateFlipping();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually set.
    ///</summary>
    void OnFlippingFinish();
    ///<summary>
    /// Fires when the object starts becomming normal from being flipped. Here it calls the animation if the object is set to animate.
    ///</summary>
    void OnFlippingRevert();
    ///<summary>
    /// The animation of the object when it goes back to normal from being flipped.
    ///</summary>
    IEnumerator AnimateFlippingRevert();
    ///<summary>
    /// Fired when the animation has finished. Or immediately when the animation is set to false. Here the changes are actually rmeoved.
    ///</summary>
    void OnFlippingRevertFinish();
}


///<summary>
/// Interface for an object inside a room
///</summary>
public interface IRoomObject {
    ///<summary>
    /// Is true when the player is inside the same room as the object
    ///</summary>
    bool InSpace {get; }
    void OnRoomEnter();
    void OnRoomLeave();
}

///<summary>
/// The entity can actually change.
///</summary>
public interface IChangable : IMissable, IShrinkable, IEnlargable, IFlippable
{
    ///<summary>
    /// The word the mirror searches for when the player checks the answer or sets a change.
    ///</summary>
    string Word {get; set; }
    ///<summary>
    /// Array of alternate words. Example (color, colour)
    ///</summary>
    string[] AlternativeWords {get; set; }

    int id {get;set;}
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
    /// Adds the change of the entity.
    ///</summary>
    void AddChange(Change changeType);
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

