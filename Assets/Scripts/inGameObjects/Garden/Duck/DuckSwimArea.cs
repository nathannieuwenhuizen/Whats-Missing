using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckSwimArea : MonoBehaviour
{
    [SerializeField]
    private Vector2 size = new Vector2(5, 5);
    [SerializeField]
    private Vector3 target;
    public Vector3 Target {
        get { return target;}
    }

    private void Awake() {
        SetNewTarget();
    }
    public void SetNewTarget()
    {
        target = new Vector3(
            transform.position.x + Random.Range(-size.x / 2f, size.x / 2f),
            transform.position.y,
            transform.position.z + Random.Range(-size.y / 2f, size.y / 2f)
        );
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, .1f, size.y));
        Gizmos.DrawSphere(target, .3f);
    }

}
