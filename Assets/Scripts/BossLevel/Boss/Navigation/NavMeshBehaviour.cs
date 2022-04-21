using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Boss;

public class NavMeshBehaviour : IMovementBehavior
{
    public Transform desiredPos { get; set; }
    public BodyOrientation bodyOrientation {get; set;} = BodyOrientation.toShape;

    private Transform transform;
    private Transform desiredTempPos;
    private NavMeshAgent navMeshAgent;
    private Boss.Boss boss;

    private bool movementUpdateEnabled = true;
    public bool MovementEnabled {
        get { return movementUpdateEnabled;}
        set { 
            movementUpdateEnabled = value;
            navMeshAgent.enabled = movementUpdateEnabled;
        }
    }


    public NavMeshBehaviour(Transform _transform, Transform _desiredTempPos) {
        transform = _transform;
        desiredTempPos = _desiredTempPos;
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
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
        navMeshAgent.SetDestination(desiredPos.position);
    }
    public void Update() {
        if (MovementEnabled) {
                UpdateTempDestination();
        }
    }
}
