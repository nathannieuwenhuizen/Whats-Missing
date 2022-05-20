using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;

public class AirSteeringBehaviour : IMovementBehavior
{
    public Transform desiredPos { get; set; }
    public bool MovementEnabled { get; set; } = true;

    private float speedScale = 1f;
    public float SpeedScale { 
        get => SpeedScale;
        set {
            speedScale = value;
        } 
    }

    private BossMountain bossMountain;
    private MountainPath path;
    private Transform transform;
    private Transform desiredTempPos;
    private SteeringBehaviour steeringBehaviour;


    public AirSteeringBehaviour(Transform _transform, Transform _desiredTempPos, SteeringBehaviour _steeringBehaviour, BossMountain _pathHandeler) {
        transform = _transform;
        desiredTempPos = _desiredTempPos;
        steeringBehaviour = _steeringBehaviour;
        bossMountain = _pathHandeler;

        steeringBehaviour.target = transform;
        steeringBehaviour.desiredTarget = desiredTempPos;
        desiredTempPos.SetParent(transform.parent);

        path.steps = 10;
    }

    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default){
        path.begin = MountainCoordinate.FromPosition(bossMountain, _begin == default(Vector3) ? transform.position : _begin);
        path.end = MountainCoordinate.FromPosition(bossMountain, _end);
        path.generatePathPoints(bossMountain);
        UpdateTempDestination();
    }

    public void SetDestinationPath(Transform _target, Vector3 _begin = default){
        desiredPos = _target;
        SetDestinationPath(desiredPos.position, _begin);
    }

    public void UpdateTempDestination(){
        desiredTempPos.position = path.GetClosestMountainCoord(transform.position, bossMountain).ToVector(bossMountain, 5f);
    }
    public void Update() {
        if (MovementEnabled) {
            UpdateTempDestination();
            steeringBehaviour.UpdatePosition(speedScale);
            // UpdateRotation();
        }
    }

    public Vector3 GetClosestPointOnPath(){
        // return MountainCoordinate.FromPosition(pathHandeler, transform.position).ToVector(pathHandeler);
        return path.GetClosestMountainCoord(transform.position, bossMountain).ToVector(bossMountain);    
    }

    public bool ReachedDestination(float _distanceThreshhold){
        if (Vector3.Distance(transform.position, desiredTempPos.position) > _distanceThreshhold) return false;
        if (steeringBehaviour.Velocity.magnitude > 1f) return false;
        return true;
    }

    public void DrawGizmo()
    {
        if (bossMountain != null) path.DrawGizmo(bossMountain);
    }

    public Vector3 GetClosestPointOnPath(Vector3 _position)
    {
        return path.GetClosestMountainCoord(_position, bossMountain).ToVector(bossMountain);    
    }

    ///<summary>
    /// Returns the distance of the whole path.
    ///</summary>
    public float GetPathLength()
    {
        float result = 0;
        for ( int i = 1; i < path.Coords.Length; ++i )
            result += Vector3.Distance( path.Coords[i-1].ToVector(bossMountain), path.Coords[i].ToVector(bossMountain));
        return result;
    }

    public Quaternion PathRotation()
    {
        MountainCoordinate coord = path.GetClosestMountainCoord(transform.position, bossMountain);
        Vector3 normal = -coord.Normal(bossMountain);
        Vector3 pointDirection = path.GetPathDirection(coord, bossMountain);
        return Quaternion.LookRotation(normal, pointDirection);
    }
}