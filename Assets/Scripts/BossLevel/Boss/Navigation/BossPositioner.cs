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
        set { 
            inAir = value; 
            if (boss.Body.BossAnimator != null) boss.Body.BossAnimator.SetBool(BossAnimatorParam.BOOL_INAIR, value);

        }
    }

    [SerializeField]
    private SteeringBehaviour steeringBehaviour;
    public SteeringBehaviour SteeringBehaviour {
        get { return steeringBehaviour;}
    }

    [SerializeField]
    private AnimationCurve landingCurve;

    [SerializeField]
    private BossMountain bossMountain;
    public BossMountain BossMountain {
        get { return bossMountain;}
    }
    private MountainPath path;

    private Transform desiredTempPos;
    private Transform desiredPos;

    private Boss boss;


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
            airMovementBehaviour.MovementEnabled = value;
            navMeshMovementBehaviour.MovementEnabled = value;
            freefloatMovementBehaviour.MovementEnabled = value;
            CurrentMovementBehaviour.MovementEnabled = value;
            navMeshAgent.enabled = movementType == BodyMovementType.navMesh && value;
        }
    }
    private bool rotationEnabled = true;
    public bool RotationEnabled {
        get { return rotationEnabled;}
        set { rotationEnabled = value; }
    }
    [SerializeField]
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
        desiredTempPos.SetParent(transform.parent);

        desiredPos = new GameObject("desired position").transform;
        desiredPos.position = transform.position;

        navMeshAgent = GetComponent<NavMeshAgent>();

        steeringBehaviour.target = transform;
        steeringBehaviour.desiredTarget = desiredTempPos;

        airMovementBehaviour = new AirSteeringBehaviour(transform, desiredTempPos, this, bossMountain);
        navMeshMovementBehaviour = new NavMeshBehaviour(transform, desiredTempPos);
        freefloatMovementBehaviour = new FreeFloatBehaviour(transform, desiredTempPos, this);
        airMovementBehaviour.desiredPos = navMeshMovementBehaviour.desiredPos = freefloatMovementBehaviour.desiredPos = desiredPos;

        BodyMovementType = movementType;        
    }

    private void Start() {
        SetDestinationPath(desiredPos);
    }


    ///<summary>
    /// Sets the destination of the positioner so that it can go from one place to another.
    ///</summary>
    public void SetDestinationPath(Transform _target, Vector3 _begin = default(Vector3), bool _withPathOffset = true, float _basePathOffset = 5f) {
        desiredPos = _target;
        CurrentMovementBehaviour.WithPathOffset = _withPathOffset;
        CurrentMovementBehaviour.BasePathOffset = _basePathOffset;
        steeringBehaviour.desiredTarget = desiredPos;
        CurrentMovementBehaviour.SetDestinationPath(desiredPos, _begin);
    }
    
    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default(Vector3), bool _withPathOffset = true, float _basePathOffset = 5f) {
        CurrentMovementBehaviour.WithPathOffset = _withPathOffset;
        CurrentMovementBehaviour.BasePathOffset = _basePathOffset;
        CurrentMovementBehaviour.SetDestinationPath(_end, _begin);
    }

    public MountainCoordinate GetClosestMountainCoordOfBoss() {
        return path.GetClosestMountainCoord(transform.position, bossMountain);
    }

    public bool AtPosition(float _offset = .1f) {
        return CurrentMovementBehaviour.ReachedDestination(_offset);
    }

    private void Update() {
        if (MovementEnabled) CurrentMovementBehaviour.Update();
        if (RotationEnabled) UpdateRotation();
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


    //lands the boss on a specific position.
    public IEnumerator Landing(Transform _endPos = null, Action callback = null) {
        Debug.Log("landing");

        //set the correct movement options.
        BodyOrientation = BodyOrientation.toPlayer;
        BodyMovementType = BodyMovementType.freeFloat;

        //sets the destination.
        MovementEnabled = true;
        SetDestinationPath(_endPos.position, transform.position);
        SpeedScale = 1f;

        //do landing animation.
        InAir = false;
        float timePassed = 0;

        while(!AtPosition(3f)) {
            timePassed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        //spawn debree at end position when the coordinate of the boss is within 3 units of its destination.
        SpawnDebree(_endPos);
        AudioHandler.Instance?.Play3DSound(SFXFiles.boss_landing, transform);

        //wait until the boss is within 1 unit and the animation has at least been 1 second to finish the animation
        while(!AtPosition(1f) && timePassed < 1.1f) {
            timePassed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        } 
        
        //callback will be called if it isnt null.
        if (callback != null) callback();
    }


    ///<summary>
    /// Spawns a debree prefab at the position
    ///</summary>
    private void SpawnDebree(Transform origin) {
        GameObject debree = Instantiate(Resources.Load<GameObject>("Roomprefabs/debrees"));
        debree.transform.position = origin.position;
        debree.transform.rotation = origin.rotation;
        debree.GetComponent<ParticleSystem>().Emit(10);
        debree.GetComponentInChildren<ParticleSystem>().Emit(3);
        Destroy(debree, 5f);
    }



    ///<summary>
    /// Makes the boss go into airing mode.
    ///</summary>
    public IEnumerator TakingOff(Action callback) {
        Debug.Log("taking off");
        //set the correct movement options.
        BodyOrientation = BodyOrientation.toPlayer;
        BodyMovementType = BodyMovementType.airSteeringAtMountain;

        //sets the destination.
        MovementEnabled = false;
        SetDestinationPath(transform.position + Vector3.up * 10f, transform.position + Vector3.up * 10f, false, 15f);
        SpeedScale = 1f;

        //do taking off animation.
        InAir = true;
        yield return new WaitForSeconds(.3f);

        MovementEnabled = true;
        //spawn debree at end position when the coordinate of the boss is within 3 units of its destination.
        // SpawnDebree(transform);
        // AudioHandler.Instance?.Play3DSound(SFXFiles.boss_landing, transform);

        //wait until the boss is within 1 unit.
        while(!AtPosition(1f)) yield return new WaitForFixedUpdate();
        
        //callback will be called if it isnt null.
        if (callback != null) callback();   
    }
}

}