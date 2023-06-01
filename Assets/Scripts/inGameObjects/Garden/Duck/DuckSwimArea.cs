using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DuckSwimArea : MonoBehaviour, IRoomObject
{
    [SerializeField]
    private float radius = 10;

    public float Radius {
        get { return radius;}
    }
    [SerializeField]
    private Vector3 target;
    public Vector3 Target {
        get { return target;}
    }

    public bool InSpace { get; set; } = false;

    private void Awake() {
        SetNewTarget();
    }
    public void SetNewTarget()
    {
        float randomAngle = Random.Range(0, Mathf.PI * 2f);
        target = new Vector3(

            transform.position.x + Mathf.Cos(randomAngle) * Random.Range(0, radius * .9f),
            transform.position.y,
            transform.position.z + Mathf.Sin(randomAngle) * Random.Range(0, radius * .9f)
        );
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (int i = 0 ; i < 36; i++) {
            float precentage = (Mathf.PI * 2f);
            Vector3 temp = transform.position +  new Vector3(
                Mathf.Cos(((float)i / 36f) * precentage), 0, Mathf.Sin(((float)i / 36f) * precentage
            )) * radius;
            Vector3 temp2 = transform.position +  new Vector3(
                Mathf.Cos(((float)(i + 1) / 36f) * precentage), 0, Mathf.Sin(((float)(i + 1) / 36f) * precentage
            )) * radius;

            Debug.DrawLine(temp, temp2, Color.blue);
        }
        Gizmos.DrawSphere(target, .3f);
    }

    public void OnRoomEnter()
    {
        SetNewTarget();
    }

    public void OnRoomLeave()
    {

    }
}
