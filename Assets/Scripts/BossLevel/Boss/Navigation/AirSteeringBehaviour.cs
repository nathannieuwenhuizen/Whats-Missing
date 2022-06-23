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

    public bool WithPathOffset { get; set; } = true;
    public float BasePathOffset { get; set; } = 5f;
    public Vector3 Velocity { 
        get => bossPositioner.SteeringBehaviour.Velocity;
        set => bossPositioner.SteeringBehaviour.Velocity = value;
    }


    private BossMountain bossMountain;
    private MountainPath path;
    private Transform transform;
    private Transform desiredTempPos;
    private BossPositioner bossPositioner;


    public AirSteeringBehaviour(Transform _transform, Transform _desiredTempPos, BossPositioner _bossPositioner, BossMountain _pathHandeler) {
        transform = _transform;
        desiredTempPos = _desiredTempPos;
        bossPositioner = _bossPositioner;
        bossMountain = _pathHandeler;

        bossPositioner.SteeringBehaviour.target = transform;
        bossPositioner.SteeringBehaviour.desiredTarget = desiredTempPos;
        desiredTempPos.SetParent(transform.parent);

        path.steps = 10;
    }

    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default){
        path.begin = MountainCoordinate.FromPosition(bossMountain, _begin == default(Vector3) ? transform.position : _begin);
        path.end = MountainCoordinate.FromPosition(bossMountain, _end);
        path.generatePathPoints(bossMountain, WithPathOffset, BasePathOffset);
        UpdateTempDestination();
    }

    public void SetDestinationPath(Transform _target, Vector3 _begin = default){
        desiredPos = _target;
        SetDestinationPath(desiredPos.position, _begin);
    }

    public void UpdateTempDestination(){
        desiredTempPos.position = path.GetClosestMountainCoord(transform.position, bossMountain).ToPathVector(bossMountain, path, BasePathOffset);
        bossPositioner.SteeringBehaviour.desiredTarget = desiredTempPos;
    
    }
    public void Update() {
        if (MovementEnabled) {
            UpdateTempDestination();
            bossPositioner.SteeringBehaviour.UpdatePosition(speedScale);
            // UpdateRotation();
        }
    }

    public Vector3 GetClosestPointOnPath(){
        // return MountainCoordinate.FromPosition(pathHandeler, transform.position).ToVector(pathHandeler);
        return path.GetClosestMountainCoord(transform.position, bossMountain).ToPathVector(bossMountain, path, BasePathOffset);    
    }

    public bool ReachedDestination(float _distanceThreshhold){
        Vector3 pos = path.end.ToPathVector(bossMountain, path, BasePathOffset);
        // Debug.Log("distance = " + Vector3.Distance(transform.position, pos));
        if (Vector3.Distance(transform.position, pos) > _distanceThreshhold) return false;
        if (bossPositioner.SteeringBehaviour.Velocity.magnitude > 1f) return false;
        return true;
    }

    public void DrawGizmo()
    {
        if (bossMountain != null) path.DrawGizmo(bossMountain);
        Gizmos.DrawSphere(desiredTempPos.position, 1f);
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
            result += Vector3.Distance( path.Coords[i-1].ToVector(bossMountain), path.Coords[i].ToPathVector(bossMountain, path, 0));
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
