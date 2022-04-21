using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Boss {

public enum BodyOrientation {
    none,
    toShape,
    toPath
}

public enum BodyMovementType {
    none,
    steeringBehaviour,
    navMesh
}

///<summary>
/// Handles the boss position, using steering behaviour with the mountain coordinates
///</summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Boss))]
public class BossPositioner : MonoBehaviour
{
    [SerializeField]
    private SteeringBehaviour steeringBehaviour;

    [SerializeField]
    private AnimationCurve landingCurve;

    [SerializeField]
    private BossPathHandler pathHandeler;
    private MountainPath path;

    private Transform desiredTempPos;

    private Boss boss;

    [SerializeField]
    private Transform desiredPos;

    private NavMeshAgent navMeshAgent;

    private bool movementUpdateEnabled = true;
    public bool MovementEnabled {
        get { return movementUpdateEnabled;}
        set { 
            movementUpdateEnabled = value;
            navMeshAgent.enabled = movementType == BodyMovementType.navMesh && movementUpdateEnabled;
        }
    }

    BodyMovementType movementType = BodyMovementType.steeringBehaviour;
    public BodyMovementType BodyMovementType {
        get { return movementType;}
        set { 
            movementType = value; 
            navMeshAgent.enabled = movementType == BodyMovementType.navMesh && movementUpdateEnabled;
        }
    }


    private BodyOrientation bodyOrientation = BodyOrientation.toShape;
    public BodyOrientation BodyOrientation {
        get { return bodyOrientation;}
        set { bodyOrientation = value; }
    }

    private void Awake() {
        boss = GetComponent<Boss>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        BodyMovementType = movementType;

        desiredTempPos = Instantiate(new GameObject("desired boss position"), transform.position, Quaternion.identity).transform;
        
        steeringBehaviour.target = transform;
        desiredTempPos.SetParent(transform.parent);
        steeringBehaviour.desiredTarget = desiredTempPos;
        path.steps = 10;
    }

    private void Start() {
        SetDestinationPath(desiredPos);
    }


    ///<summary>
    /// Sets the destination of the positioner so that it can go from one place to another.
    ///</summary>
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3)) {
        desiredPos = _target;
        SetDestinationPath(desiredPos.position, _begin);
    }
    
    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3)) {
        path.begin = MountainCoordinate.FromPosition(pathHandeler, _begin == default(Vector3) ? transform.position : _begin);
        path.end = MountainCoordinate.FromPosition(pathHandeler, _end);
        path.generatePathPoints(pathHandeler);
        UpdateTempDestination();
    }

    private void UpdateTempDestination() {
        if (!movementUpdateEnabled) return;

        if (BodyMovementType == BodyMovementType.steeringBehaviour)
            desiredTempPos.position = path.GetClosestMountainCoord(transform.position, pathHandeler).ToVector(pathHandeler, 5f);
        else if (BodyMovementType == BodyMovementType.navMesh) {
            navMeshAgent.SetDestination(boss.Player.transform.position);
        }
    }

    public MountainCoordinate GetClosestMountainCoordOfBoss() {
        return path.GetClosestMountainCoord(transform.position, pathHandeler);
    }

    public bool isAtPosition(float _offset = .1f, float _velocityOffset = .1f) {
        if (Vector3.Distance(transform.position, desiredTempPos.position) > _offset) return false;
        if (steeringBehaviour.Velocity.magnitude > _velocityOffset) return false;
        return true;
    }

    private void Update() {
        if (movementUpdateEnabled) {
            if (BodyMovementType == BodyMovementType.steeringBehaviour) {
                UpdateTempDestination();
                steeringBehaviour.UpdatePosition();
                if (bodyOrientation != BodyOrientation.none) {
                    MountainCoordinate coord = path.GetClosestMountainCoord(transform.position, pathHandeler);
                    Vector3 normal = -coord.Normal(pathHandeler);
                    if (bodyOrientation == BodyOrientation.toShape)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(normal), Time.deltaTime);
                    else if (bodyOrientation == BodyOrientation.toPath) {
                        Vector3 pointDirection = path.GetPathDirection(coord, pathHandeler);
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(normal, pointDirection), Time.deltaTime);
                    }
                }
            } else if (BodyMovementType == BodyMovementType.navMesh) {
                UpdateTempDestination();
            } 
        }

    }

    private void OnDrawGizmos() {
        if (pathHandeler != null) path.DrawGizmo(pathHandeler);
        Gizmos.color = Color.green;
        if (desiredTempPos != null) Gizmos.DrawSphere(desiredTempPos.position, 3f);
    }

    public IEnumerator Landing( MountainCoordinate landingCoord, Action callback) {
        Vector3 landingPos = Vector3.zero;
        if (BodyMovementType == BodyMovementType.steeringBehaviour) {
            Vector3 normal = -landingCoord.Normal(pathHandeler);
            Vector3 pointDirection = path.GetPathDirection(landingCoord, pathHandeler);
            StartCoroutine(transform.AnimatingRotation(Quaternion.LookRotation(normal, pointDirection), AnimationCurve.EaseInOut(0,0,1,1), 2f));
            landingPos = landingCoord.ToVector(pathHandeler);
            yield return StartCoroutine(transform.AnimatingPos(landingPos, landingCurve, 2f));
        } else if (BodyMovementType == BodyMovementType.navMesh) {
            NavMeshHit myNavHit;
            if(NavMesh.SamplePosition(transform.position, out myNavHit, 100, -1 )){
                landingPos = myNavHit.position + (Vector3.up * navMeshAgent.height);
                yield return StartCoroutine(transform.AnimatingPos(landingPos, landingCurve, 2f));
            }
        }
        callback();
    }
}

}