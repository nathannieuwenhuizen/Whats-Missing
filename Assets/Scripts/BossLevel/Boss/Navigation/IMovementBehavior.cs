using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;

public interface IMovementBehavior
{
    public Transform desiredPos {get; set; }
    public bool MovementEnabled {get; set; }

    public void UpdateTempDestination();

    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3));
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3));

    public BodyOrientation bodyOrientation {get; set;}

}
