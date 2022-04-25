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

    public void UpdateTempDestination();
    public void Update();
    public void UpdateRotation();

    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3));
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3));

    public BodyOrientation bodyOrientation {get; set;}

    public Vector3 GetClosestPointOnPath();

    public bool ReachedDestination(float _distanceThreshhold);

}
