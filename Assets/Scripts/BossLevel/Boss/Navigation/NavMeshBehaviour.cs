using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Boss;

public struct NavMeshValues {
    public float speed;
    public float acceleration;
    public float angularSpeed;

    public static NavMeshValues Save(NavMeshAgent _agent) {
        return new NavMeshValues() {
            speed = _agent.speed,
            acceleration = _agent.acceleration,
            angularSpeed = _agent.angularSpeed
        };
    }
    public void SetValues(NavMeshAgent _agent, float _scale = 1f) {
        _agent.speed = speed * _scale;
        _agent.acceleration = acceleration * _scale;
        _agent.angularSpeed = angularSpeed * _scale;
    }
}

public class NavMeshBehaviour : IMovementBehavior
{
    public Transform desiredPos { get; set; }
    public float BasePathOffset { get; set; } = 5f;

    private Transform transform;
    private Transform desiredTempPos;
    private NavMeshAgent navMeshAgent;

    private bool movementUpdateEnabled = true;
    public bool MovementEnabled {
        get { return movementUpdateEnabled;}
        set { 
            movementUpdateEnabled = value;
        }
    }
    public Vector3 Velocity { 
        get => navMeshAgent.velocity;
        set => navMeshAgent.velocity = value;
    }

    public Boss.Boss boss;

    public bool WithPathOffset { get; set; } = true;

    private NavMeshValues startNavMeshValues;

    private float speedScale = 1f;
    public float SpeedScale { 
        get => SpeedScale;
        set {
            speedScale = value;
            startNavMeshValues.SetValues(navMeshAgent, value);
        } 
    }

    public NavMeshBehaviour(Transform _transform, Transform _desiredTempPos, Boss.Boss _boss) {
        transform = _transform;
        desiredTempPos = _desiredTempPos;
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        startNavMeshValues = NavMeshValues.Save(navMeshAgent);
        boss = _boss;
    }

    public void SetDestinationPath(Vector3 _end, Vector3 _begin = default)
    {
        UpdateTempDestination();
    }

    public void SetDestinationPath(Transform _target, Vector3 _begin = default)
    {
        desiredPos = _target;
        SetDestinationPath(desiredPos.position, _begin);
    }

    public void UpdateTempDestination()
    {
        if (navMeshAgent.enabled) navMeshAgent.SetDestination(desiredPos.position);
    }
    private float cVelocity = 0;
    public void Update() {
        if (MovementEnabled) {
            UpdateTempDestination();

            //update the animator crawlspeed
            Debug.Log("velocity: " + Velocity.magnitude);
            if (cVelocity > Velocity.magnitude) {
                cVelocity = Mathf.Lerp(cVelocity, Velocity.magnitude, Time.deltaTime * 5f);
            } else {
                cVelocity = Mathf.Lerp(cVelocity, Velocity.magnitude, Time.deltaTime);
            }
            boss.Body.BossAnimator.SetFloat(BossAnimatorParam.FLOAT_CRAWLSPEED, cVelocity / 10f);
        }
    }

    public Vector3 GetClosestPointOnPath()
    {
        NavMeshHit myNavHit;
        if(NavMesh.SamplePosition(transform.position, out myNavHit, 100, -1 ))
            return  myNavHit.position + (Vector3.up * navMeshAgent.height);
        return transform.position;   
    }

    public bool ReachedDestination(float _distanceThreshhold)
    {
        if (navMeshAgent.enabled == false) return false;
        if (navMeshAgent.remainingDistance - (navMeshAgent.radius * 1f) > _distanceThreshhold || navMeshAgent.remainingDistance <= 0) return false;
        return true;
    }

    public void DrawGizmo()
    {
        if (navMeshAgent != null) {
            if (navMeshAgent.destination.x < 10000f) { //otherwise bug for the mathf infinity :/
                Debug.DrawLine(transform.position, navMeshAgent.destination, Color.green);
                Gizmos.DrawWireSphere(navMeshAgent.destination, .5f); 
            }
        }
    }

    public Vector3 GetClosestPointOnPath(Vector3 _position)
    {
        NavMeshHit myNavHit;
        if(NavMesh.SamplePosition(_position, out myNavHit, 100, -1 )){
            return  myNavHit.position;
        }
        return _position;       
    }
       
    public float GetPathLength()
    {
        NavMeshPath path = navMeshAgent.path;
        float result = 0.0f;
        if (( path.status != NavMeshPathStatus.PathInvalid ) && ( path.corners.Length > 1 ))
            for ( int i = 1; i < path.corners.Length; ++i )
                result += Vector3.Distance( path.corners[i-1], path.corners[i] );
       
        return result;
    }

    public Quaternion PathRotation()
    {
        Vector3 turnTowardNavSteeringTarget = navMeshAgent.velocity;
        Vector3 direction = navMeshAgent.velocity; // (turnTowardNavSteeringTarget - transform.position).normalized;
        // if(NavMesh.SamplePosition(navMeshAgent.nextPosition, out myNavHit, .1f, -1 ))
        //     direction = myNavHit.normal;
        if (direction.magnitude != 0) {
            return Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        }
        return transform.rotation;
    }
}
