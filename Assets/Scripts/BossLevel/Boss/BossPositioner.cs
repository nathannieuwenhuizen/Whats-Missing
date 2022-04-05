using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Transform desiredPos;

    [SerializeField]
    private Transform test;

    [SerializeField]
    private bool useSteering = false;
    public bool UseSteering {
        get { return useSteering;}
        set { useSteering = value; }
    }

    private void Awake() {
        steeringBehaviour.target = transform;
        desiredPos = Instantiate(new GameObject("desired boss position"), transform.position, Quaternion.identity).transform;
        desiredPos.SetParent(transform.parent);
        steeringBehaviour.desiredTarget = desiredPos;
        path.steps = 10;
    }

    private void Start() {
        SetAirDestinationPath(test.position);
    }

    public void SetAirDestinationPath(Vector3 pos) {
        path.begin = MountainCoordinate.FromPosition(pathHandeler, transform.position);
        path.end = MountainCoordinate.FromPosition(pathHandeler, pos);
        coords = path.generatePathPoints(pathHandeler);
        UpdateDestination();
    }

    public void UpdateDestination() {
        desiredPos.position = path.getClosestMountainCoord(coords, transform.position, pathHandeler).ToVector(pathHandeler, 5f);
    }

    private void Update() {
        if (useSteering) {
            SetAirDestinationPath(test.position);
            UpdateDestination();
            steeringBehaviour.UpdatePosition();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-path.getClosestMountainCoord(coords, transform.position, pathHandeler).Normal(pathHandeler)), Time.deltaTime);
        }
    }
    private void OnDrawGizmos() {
        if (pathHandeler != null) path.DrawGizmo(pathHandeler);
        Gizmos.color = Color.green;
        if (desiredPos != null) Gizmos.DrawSphere(desiredPos.position, 3f);
    }
}
