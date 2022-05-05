using System.Collections;
using System.Collections.Generic;
using Boss;
using UnityEngine;

public class AirSteeringBehaviour : IMovementBehavior
{
    public Transform desiredPos { get; set; }
    public bool MovementEnabled { get; set; } = true;
    public BodyOrientation bodyOrientation {get; set;} = BodyOrientation.toShape;

    private float speedScale = 1f;
    public float SpeedScale { 
        get => SpeedScale;
        set {
            speedScale = value;
        } 
    }

    private BossPathHandler pathHandeler;
    private MountainPath path;
    private Transform transform;
    private Transform desiredTempPos;
    private SteeringBehaviour steeringBehaviour;


    public AirSteeringBehaviour(Transform _transform, Transform _desiredTempPos, SteeringBehaviour _steeringBehaviour, BossPathHandler _pathHandeler) {
        transform = _transform;
        desiredTempPos = _desiredTempPos;
        steeringBehaviour = _steeringBehaviour;
        pathHandeler = _pathHandeler;

        steeringBehaviour.target = transform;
        steeringBehaviour.desiredTarget = desiredTempPos;
        desiredTempPos.SetParent(transform.parent);

        path.steps = 10;
    }

    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default){
        path.begin = MountainCoordinate.FromPosition(pathHandeler, _begin == default(Vector3) ? transform.position : _begin);
        path.end = MountainCoordinate.FromPosition(pathHandeler, _end);
        path.generatePathPoints(pathHandeler);
        UpdateTempDestination();
    }

    public void SetDestinationPath(Transform _target, Vector3 _begin = default){
        desiredPos = _target;
        SetDestinationPath(desiredPos.position, _begin);
    }

    public void UpdateTempDestination(){
        desiredTempPos.position = path.GetClosestMountainCoord(transform.position, pathHandeler).ToVector(pathHandeler, 5f);
    }
    public void Update() {
        if (MovementEnabled) {
            UpdateTempDestination();
            steeringBehaviour.UpdatePosition();
            UpdateRotation();
        }
    }

    public Vector3 GetClosestPointOnPath(){
        // return MountainCoordinate.FromPosition(pathHandeler, transform.position).ToVector(pathHandeler);
        return path.GetClosestMountainCoord(transform.position, pathHandeler).ToVector(pathHandeler);    
    }

    public bool ReachedDestination(float _distanceThreshhold){
        if (Vector3.Distance(transform.position, desiredTempPos.position) > _distanceThreshhold) return false;
        if (steeringBehaviour.Velocity.magnitude > 1f) return false;
        return true;
    }

    public void UpdateRotation(){
        MountainCoordinate coord = path.GetClosestMountainCoord(transform.position, pathHandeler);
        Vector3 normal = -coord.Normal(pathHandeler);
        if (bodyOrientation == BodyOrientation.toShape)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(normal), Time.deltaTime);
        else if (bodyOrientation == BodyOrientation.toPath) {
            Vector3 pointDirection = path.GetPathDirection(coord, pathHandeler);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(normal, pointDirection), Time.deltaTime);
        }
    }

    public void DrawGizmo()
    {
        if (pathHandeler != null) path.DrawGizmo(pathHandeler);
    }
}
