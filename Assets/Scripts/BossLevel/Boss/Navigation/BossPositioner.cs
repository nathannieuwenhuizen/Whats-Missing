using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Boss {

public enum BodyOrientation {
    none,
    toShape,
    toPath,
    toPlayer
}

public enum BodyMovementType {
    ///<summary>
    /// Uses air steering on the mountain surface.
    ///</summary>
    airSteeringAtMountain,
    ///<summary>
    /// Ground based navigation movement
    ///</summary>
    navMesh,
    ///<summary>
    /// Free floating in the air. Doesnt check collission.
    ///</summary>
    freeFloat
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
    private BossMountain bossMountain;
    public BossMountain BossMountain {
        get { return bossMountain;}
    }
    private MountainPath path;

    private Transform desiredTempPos;

    private Boss boss;

    [SerializeField]
    private Transform desiredPos;

    private NavMeshAgent navMeshAgent;


    private AirSteeringBehaviour airMovementBehaviour;
    private NavMeshBehaviour navMeshMovementBehaviour;
    private FreeFloatBehaviour freefloatMovementBehaviour;
    public IMovementBehavior CurrentMovementBehaviour {
        get {
            if (BodyMovementType == BodyMovementType.airSteeringAtMountain) return airMovementBehaviour;
            else if (BodyMovementType == BodyMovementType.freeFloat) return freefloatMovementBehaviour;
            return navMeshMovementBehaviour;
        }
    }

    public bool MovementEnabled {
        get { return CurrentMovementBehaviour.MovementEnabled;}
        set { 
            airMovementBehaviour.MovementEnabled = false;
            navMeshMovementBehaviour.MovementEnabled = false;
            freefloatMovementBehaviour.MovementEnabled = false;
            CurrentMovementBehaviour.MovementEnabled = value;
            navMeshAgent.enabled = movementType == BodyMovementType.navMesh && value;

        }
    }

    BodyMovementType movementType = BodyMovementType.airSteeringAtMountain;
    public BodyMovementType BodyMovementType {
        get { return movementType;}
        set { 
            movementType = value; 
            navMeshAgent.enabled = movementType == BodyMovementType.navMesh && MovementEnabled;
        }
    }

    private float speedScale = 1f;
    public float SpeedScale {
        get { return speedScale;}
        set { 
            speedScale = value;
            navMeshMovementBehaviour.SpeedScale = value;
            airMovementBehaviour.SpeedScale = value;
            freefloatMovementBehaviour.SpeedScale = value;
        }
    }

    private BodyOrientation bodyOrientation = BodyOrientation.none;
    public BodyOrientation BodyOrientation {
        get { return bodyOrientation;}
        set { 
            bodyOrientation = value;
        }
    }

    private void Awake() {
        boss = GetComponent<Boss>();
        desiredTempPos = new GameObject("desired temp position").transform;
        desiredTempPos.position = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();

        airMovementBehaviour = new AirSteeringBehaviour(transform, desiredTempPos, steeringBehaviour, bossMountain);
        navMeshMovementBehaviour = new NavMeshBehaviour(transform, desiredTempPos);
        freefloatMovementBehaviour = new FreeFloatBehaviour(transform, desiredTempPos, steeringBehaviour);

        BodyMovementType = movementType;        
    }

    private void Start() {
        SetDestinationPath(desiredPos);
    }


    ///<summary>
    /// Sets the destination of the positioner so that it can go from one place to another.
    ///</summary>
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3), bool _withPathOffset = true) {
        desiredPos = _target;
        CurrentMovementBehaviour.WithPathOffset = _withPathOffset;
        CurrentMovementBehaviour.SetDestinationPath(desiredPos, _begin);
    }
    
    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3), bool _withPathOffset = true) {
        CurrentMovementBehaviour.WithPathOffset = _withPathOffset;
        CurrentMovementBehaviour.SetDestinationPath(_end, _begin);
    }

    public MountainCoordinate GetClosestMountainCoordOfBoss() {
        return path.GetClosestMountainCoord(transform.position, bossMountain);
    }

    public bool AtPosition(float _offset = .1f) {
        return CurrentMovementBehaviour.ReachedDestination(_offset);
    }

    private void Update() {
        if (MovementEnabled) {
            CurrentMovementBehaviour.Update();
            UpdateRotation();
        }
    }

    public void UpdateRotation() {
        Quaternion desiredRotation = transform.rotation;
        switch (bodyOrientation) {
            case BodyOrientation.toShape:
            MountainCoordinate coord = MountainCoordinate.FromPosition(bossMountain, transform.position);
            Vector3 normal = -coord.Normal(bossMountain);
            desiredRotation = Quaternion.LookRotation(normal);
            break;
            case BodyOrientation.toPlayer:
            Vector3 delta = boss.Player.transform.position - transform.position;
            delta.y = 0;
            desiredRotation = Quaternion.LookRotation(delta, Vector3.up);
            break;
            case BodyOrientation.toPath:
            desiredRotation = CurrentMovementBehaviour.PathRotation();
            break;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime);
    }

    private void OnDrawGizmos() {
        if(CurrentMovementBehaviour != null) CurrentMovementBehaviour.DrawGizmo();
    }

    public IEnumerator Landing(Action callback, Transform _endPos = null) {
        OnBossLanding?.Invoke();
        BodyMovementType = BodyMovementType.navMesh;
        MovementEnabled = false;
        Vector3 _landingPosition = CurrentMovementBehaviour.GetClosestPointOnPath();
        if (_endPos != null) {
            _landingPosition = CurrentMovementBehaviour.GetClosestPointOnPath(_endPos.position);
        }
        yield return StartCoroutine(transform.AnimatingPos(_landingPosition, landingCurve, 2f));
        MovementEnabled = true;
        callback();
    }
    public IEnumerator TakeOff(Action callback) {
        inAir = true;
        OnBossTakeOff?.Invoke();
        BodyMovementType = BodyMovementType.airSteeringAtMountain;
        MovementEnabled = false;
        yield return StartCoroutine(transform.AnimatingPos(CurrentMovementBehaviour.GetClosestPointOnPath(), landingCurve, 2f));
        MovementEnabled = true;
        callback();
    }
}

}