using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

///<summary>
/// Handles the boss position, using steering behaviour with the mountain coordinates
///</summary>
public class BossPositioner : MonoBehaviour
{
    [SerializeField]
    private SteeringBehaviour steeringBehaviour;

    [SerializeField]
    private BossPathHandler pathHandeler;
    private MountainPath path;
    private MountainCoordinate[] coords;

    private Transform desiredTempPos;

    [SerializeField]
    private Transform desiredPos;

    [SerializeField]
    private bool useSteering = false;
    public bool UseSteering {
        get { return useSteering;}
        set { useSteering = value; }
    }

    private void Awake() {
        steeringBehaviour.target = transform;
        desiredTempPos = Instantiate(new GameObject("desired boss position"), transform.position, Quaternion.identity).transform;
        desiredTempPos.SetParent(transform.parent);
        steeringBehaviour.desiredTarget = desiredTempPos;
        path.steps = 10;
    }

    private void Start() {
        SetDestinationPath(desiredPos);
    }


    ///<summary>
    /// Sets the destination of the positioner so that it can go from one palce to another.
    ///</summary>
    public void SetDestinationPath(Transform _tr) {
        desiredPos = _tr;
        path.begin = MountainCoordinate.FromPosition(pathHandeler, transform.position);
        path.end = MountainCoordinate.FromPosition(pathHandeler, desiredPos.position);
        coords = path.generatePathPoints(pathHandeler);
        UpdateTempDestination();
    }

    private void UpdateTempDestination() {
        desiredTempPos.position = path.getClosestMountainCoord(coords, transform.position, pathHandeler).ToVector(pathHandeler, 5f);
    }

    public Vector3 GetDesiredPosition(float _offset) {
        return path.end.ToVector(pathHandeler, _offset);
    }

    public bool isAtPosition(float offset = .1f, float velocityOffset = .1f) {
        if (Vector3.Distance(transform.position, desiredTempPos.position) > offset) return false;
        if (steeringBehaviour.Velocity.magnitude > velocityOffset) return false;
        return true;
    }

    private void Update() {
        if (useSteering) {
            UpdateTempDestination();
            steeringBehaviour.UpdatePosition();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-path.getClosestMountainCoord(coords, transform.position, pathHandeler).Normal(pathHandeler)), Time.deltaTime);
        }
    }
    private void OnDrawGizmos() {
        if (pathHandeler != null) path.DrawGizmo(pathHandeler);
        Gizmos.color = Color.green;
        if (desiredTempPos != null) Gizmos.DrawSphere(desiredTempPos.position, 3f);
    }
}

}