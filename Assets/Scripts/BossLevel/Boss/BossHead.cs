using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The boss head that rotates arround trying to see the player.
/// A steering behavior handles the movement
///</summary>
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
        currentAim = Instantiate(new GameObject("current aim"), transform.position + transform.forward * bossAI.BossEye.ViewRange, Quaternion.identity).transform;
        desiredAim = Instantiate(new GameObject("desired aim"), transform.position + transform.forward * bossAI.BossEye.ViewRange, Quaternion.identity).transform;
        currentAim.SetParent(transform.parent);
        desiredAim.SetParent(bossAI.transform.parent);
        steeringBehaviour.target = currentAim;
        steeringBehaviour.desiredTarget = desiredAim;
    }
    public void Update()
    {
        if (steeringEnabled) {
            steeringBehaviour.UpdatePosition();
        }
        else currentAim.position = Vector3.Lerp(currentAim.position, desiredAim.position, Time.deltaTime * aimSpeed);
       
        transform.LookAt(currentAim, transform.up);
    }

    [SerializeField]
    private SteeringBehaviour steeringBehaviour;
    public SteeringBehaviour SteeringBehaviour {
        get { return steeringBehaviour;}
    }

    public void SetAim(Vector3 pos, Vector2 relativeOffset) {
        Transform t = transform.parent;
        // Debug.Log("set aim" + relativeOffset.y);
        desiredAim.transform.position = pos + 
        t.up * relativeOffset.y + 
        t.right * relativeOffset.x;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (currentAim != null && desiredAim != null) {
            Gizmos.DrawSphere(currentAim.position, .5f);
            Gizmos.DrawSphere(desiredAim.position, .5f);
            float multiply = (currentAim.position - desiredAim.position).magnitude / steeringBehaviour.DesiredVelocity.magnitude;
            Debug.DrawLine(currentAim.position, desiredAim.position, Gizmos.color);
            
            Debug.DrawLine(currentAim.position, currentAim.position + steeringBehaviour.Velocity * multiply , Color.red);
            Debug.DrawLine(currentAim.position + steeringBehaviour.Velocity * multiply, currentAim.position + steeringBehaviour.DesiredVelocity * multiply, Color.yellow);
            Debug.DrawLine(currentAim.position, currentAim.position + steeringBehaviour.DesiredVelocity * multiply, Color.green);
        }
    }
}
