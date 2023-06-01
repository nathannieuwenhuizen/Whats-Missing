using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;

///<summary>
/// The movement behaviour where the boss can us to navigate the terrain
///</summary>
public interface IMovementBehavior
{
    ///<summary>
    /// The end destination position the movement behaviour wants to go
    ///</summary>
    public Transform desiredPos {get; set; }
    ///<summary>
    /// If hte movement is enabled or not
    ///</summary>
    public bool MovementEnabled {get; set; }

    ///<summary>
    /// The scale of the speed the movement seed is set up. It doesn't make hte pudate go faster
    ///</summary>
    public float SpeedScale{get; set; }
    ///<summary>
    /// If hte path has an offset that deviates
    /// Only used to avoid obstacles in the airsteering behaviour
    ///</summary>
    public bool WithPathOffset { get; set; }
    ///<summary>
    /// Normal base offset of the movement behaviour. Also only used only for the airsteeringbehaviour
    ///</summary>
    public float BasePathOffset { get; set; }

    ///<summary>
    /// The current velocity and idrection of the movement
    ///</summary>
    public Vector3 Velocity {get; set;}

    ///<summary>
    /// Updates the temporary destination needed to get ot hte ned destination
    ///</summary>
    public void UpdateTempDestination();
    ///<summary>
    /// Updates the movvment
    ///</summary>
    public void Update();

    ///<summary>
    /// Calculates the path that is needed to go to the end
    ///</summary>
    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3));
    ///<summary>
    /// Calculates the path that is needed to go to the end
    ///</summary>
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3));
    ///<summary>
    /// Returns the lcosest point on the path from the positioner view
    ///</summary>
    public Vector3 GetClosestPointOnPath();
    ///<summary>
    /// Returns the lcosest point on the path from the _position parameter view
    ///</summary>
    public Vector3 GetClosestPointOnPath(Vector3 _position);

    ///<summary>
    /// Normal rotation of hte path.
    ///</summary>
    public Quaternion PathRotation();

    ///<summary>
    /// The length of the path.
    ///</summary>
    public float GetPathLength();

    ///<summary>
    /// Returns true if the positoner reached its target in a certain distances threshhold.
    ///</summary>
    public bool ReachedDestination(float _distanceThreshhold);

    ///<summary>
    /// Draws the gizmo needed for debugging.
    ///</summary>
    public void DrawGizmo();

}
