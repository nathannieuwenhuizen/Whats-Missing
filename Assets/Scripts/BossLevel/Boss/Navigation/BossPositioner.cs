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
    airSteering,
    navMesh
}

///<summary>
/// Handles the boss position, using steering behaviour with the mountain coordinates
///</summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Boss))]
public class BossPositioner : MonoBehaviour
{

    public delegate void BossPositionEvent();
    public static BossPositionEvent OnBossLanding;
    public static BossPositionEvent OnBossTakeOff;

    private bool inAir = true;
    public bool InAir {
        get { return inAir;}
        set { inAir = value; }
    }

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


    private AirSteeringBehaviour airMovementBehaviour;
    private NavMeshBehaviour navMeshMovementBehaviour;
    public IMovementBehavior CurrentMovementBehaviour {
        get {
            if (BodyMovementType == BodyMovementType.airSteering) return airMovementBehaviour;
            return navMeshMovementBehaviour;
        }
    }

    public bool MovementEnabled {
        get { return CurrentMovementBehaviour.MovementEnabled;}
        set { 
            airMovementBehaviour.MovementEnabled = false;
            navMeshMovementBehaviour.MovementEnabled = false;
            CurrentMovementBehaviour.MovementEnabled = value;
            navMeshAgent.enabled = movementType == BodyMovementType.navMesh && value;

        }
    }

    BodyMovementType movementType = BodyMovementType.airSteering;
    public BodyMovementType BodyMovementType {
        get { return movementType;}
        set { 
            movementType = value; 
            
            navMeshAgent.enabled = movementType == BodyMovementType.navMesh && MovementEnabled;
        }
    }

    public float SpeedScale {
        get { return CurrentMovementBehaviour.SpeedScale;}
        set { CurrentMovementBehaviour.SpeedScale = value; }
    }


    public BodyOrientation BodyOrientation {
        get { return CurrentMovementBehaviour.bodyOrientation;}
        set { CurrentMovementBehaviour.bodyOrientation = value; }
    }

    private void Awake() {
        boss = GetComponent<Boss>();
        Debug.Log("awake");
        desiredTempPos = new GameObject("desired temp position").transform;
        desiredTempPos.position = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();

        airMovementBehaviour = new AirSteeringBehaviour(transform, desiredTempPos, steeringBehaviour, pathHandeler);
        navMeshMovementBehaviour = new NavMeshBehaviour(transform, desiredTempPos);

        BodyMovementType = movementType;        
    }

    private void Start() {
        SetDestinationPath(desiredPos);
    }


    ///<summary>
    /// Sets the destination of the positioner so that it can go from one place to another.
    ///</summary>
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3)) {
        desiredPos = _target;
        CurrentMovementBehaviour.SetDestinationPath(desiredPos, _begin);
    }
    
    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3)) {
        CurrentMovementBehaviour.SetDestinationPath(_end, _begin);
    }

    public MountainCoordinate GetClosestMountainCoordOfBoss() {
        return path.GetClosestMountainCoord(transform.position, pathHandeler);
    }

    public bool AtPosition(float _offset = .1f) {
        return CurrentMovementBehaviour.ReachedDestination(_offset);
    }

    private void Update() {
        if (MovementEnabled) {
            CurrentMovementBehaviour.Update();
        }
    }

    private void OnDrawGizmos() {
        if(CurrentMovementBehaviour != null) CurrentMovementBehaviour.DrawGizmo();
    }

    public IEnumerator Landing(Action callback) {
        OnBossLanding?.Invoke();
        BodyMovementType = BodyMovementType.navMesh;
        MovementEnabled = false;
        yield return StartCoroutine(transform.AnimatingPos(CurrentMovementBehaviour.GetClosestPointOnPath(), landingCurve, 2f));
        MovementEnabled = true;
        callback();
    }
    public IEnumerator TakeOff(Action callback) {
        inAir = true;
        OnBossTakeOff?.Invoke();
        BodyMovementType = BodyMovementType.airSteering;
        MovementEnabled = false;
        yield return StartCoroutine(transform.AnimatingPos(CurrentMovementBehaviour.GetClosestPointOnPath(), landingCurve, 2f));
        MovementEnabled = true;
        callback();
    }
}

}