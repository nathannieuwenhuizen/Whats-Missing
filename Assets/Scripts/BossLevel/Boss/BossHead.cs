using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHead: MonoBehaviour
{
    private Transform currentAim;
    private Transform desiredAim;
    [SerializeField]
    private BossAI bossAI;

    private float aimSpeed = 2f;
    [SerializeField]
    private bool steeringEnabled = false;

    private void Awake() {
        currentAim = Instantiate(new GameObject("current aim"), transform.position + transform.forward * bossAI.BossEye.viewRange, Quaternion.identity).transform;
        desiredAim = Instantiate(new GameObject("desired aim"), transform.position + transform.forward * bossAI.BossEye.viewRange, Quaternion.identity).transform;
        currentAim.SetParent(transform.parent);
        desiredAim.SetParent(bossAI.transform.parent);
    }
    public void Update()
    {
        if (steeringEnabled) {
            UpdateSteeringBehaviour();
        }
        else currentAim.position = Vector3.Lerp(currentAim.position, desiredAim.position, Time.deltaTime * aimSpeed);
       
        transform.LookAt(currentAim, transform.up);
    }

    Vector3 velocity = Vector3.zero;
    Vector3 desiredVelocity = Vector3.zero;
    private Vector3 steering;
    [Header("steering behavior")]
    [SerializeField]
    private float maxVelocity = .2f;
    [SerializeField]
    private float maxForce = 003f;
    [SerializeField]
    private float mass = 5f;
    [SerializeField]
    float slowingRadius = 5f;
    [SerializeField]
    float slowingAmplitude = .5f;

    private void UpdateSteeringBehaviour() {
        desiredVelocity = (desiredAim.position - currentAim.position);

        
        // Check the distance to detect whether the character
        float distance = desiredVelocity.magnitude;
        // is inside the slowing area
        if (distance < slowingRadius) {
            // Inside the slowing area
            desiredVelocity = desiredVelocity.normalized * maxVelocity * ((distance / slowingRadius) * slowingAmplitude);
        } else {
            // Outside the slowing area.
            desiredVelocity = desiredVelocity.normalized * maxVelocity;
        }

        steering = desiredVelocity - velocity;
        if (steering.magnitude > maxForce) steering = steering.normalized * maxForce;
        steering /= mass;

        velocity += steering;

        currentAim.position += truncate(velocity);
    }

    public void SetAim(Vector2 offset) {
        Transform t = transform.parent;
        Debug.Log("set aim" + offset.y);
        desiredAim.transform.position = transform.position + 
        t.forward * bossAI.BossEye.viewRange + 
        t.up * offset.y + 
        t.right * offset.x;
    }

    private Vector3 truncate(Vector3 vector) {
        float distance = (currentAim.position - desiredAim.position).magnitude;
        if (distance < vector.magnitude) {
            vector = vector.normalized * distance;
        }
        return vector;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (currentAim != null && desiredAim != null) {
            Gizmos.DrawSphere(currentAim.position, .5f);
            Gizmos.DrawSphere(desiredAim.position, .5f);
            float multiply = 10;
            Debug.DrawLine(currentAim.position, desiredAim.position, Gizmos.color);
            
            Debug.DrawLine(currentAim.position, currentAim.position + velocity * multiply , Color.red);
            Debug.DrawLine(currentAim.position + velocity * multiply, currentAim.position + desiredVelocity * multiply, Color.yellow);
            Debug.DrawLine(currentAim.position, currentAim.position + desiredVelocity * multiply, Color.green);
        }
    }
}
