using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBody : MonoBehaviour
{
    [SerializeField]
    private SteeringBehaviour steeringBehaviour;

    [SerializeField]
    private BossPathHandler pathHandeler;
    private MountainPath path;
    private MountainCoordinate[] coords;

    private Transform desiredPos;

    [SerializeField]
    private Transform testPos;

    private void Awake() {
        steeringBehaviour.target = transform;
        desiredPos = Instantiate(new GameObject("desired position"), transform.position, Quaternion.identity).transform;
        desiredPos.SetParent(transform.parent);
        steeringBehaviour.desiredTarget = desiredPos;
        path.steps = 10;
    }

    private void Start() {
        SetAirDestinationPath(testPos.position);
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
        SetAirDestinationPath(testPos.position);

        UpdateDestination();
        steeringBehaviour.UpdatePosition();

    }
    private void OnDrawGizmos() {
        if (pathHandeler != null) path.DrawGizmo(pathHandeler);
        Gizmos.color = Color.green;
        if (desiredPos != null) Gizmos.DrawSphere(desiredPos.position, 3f);
    }
}
