using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;

///<summary>
/// The mvoement behaviour where the boss can us to navigate the terrain
///</summary>
public interface IMovementBehavior
{
    public Transform desiredPos {get; set; }
    public bool MovementEnabled {get; set; }

    public float SpeedScale{get; set; }
    public bool WithPathOffset { get; set; }
    public float BasePathOffset { get; set; }

    public void UpdateTempDestination();
    public void Update();

    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3));
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3));

    public Vector3 GetClosestPointOnPath();
    public Vector3 GetClosestPointOnPath(Vector3 _position);

    public Quaternion PathRotation();

    public float GetPathLength();

    public bool ReachedDestination(float _distanceThreshhold);

    public void DrawGizmo();

}
