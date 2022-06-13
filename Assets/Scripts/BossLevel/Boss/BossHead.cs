using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

///<summary>
/// The boss head that rotates arround trying to see the player.
/// A steering behavior handles the movement
///</summary>
public class BossHead: MonoBehaviour
{

    public delegate void HeadAimUpdateEvent(Vector3 _position);
    public static HeadAimUpdateEvent OnHeadAimUpdate;

    private Transform currentAim;
    private Transform desiredAim;
    
    [SerializeField]
    private BossAI bossAI;

    private float aimSpeed = 2f;
    [SerializeField]
    private bool steeringEnabled = true;

    public bool SteeringEnabled {
        get { return steeringEnabled;}
        set { 
            steeringEnabled = value; 
        }
    }
    private SFXInstance voiceAudio;

    private bool lookAtPlayer = false;
    ///<summary>
    /// If the boss head looks at the player or not.
    ///</summary>
    public bool LookAtPlayer {
        get { return lookAtPlayer;}
        set { lookAtPlayer = value; }
    }

    ///<summary>
    /// Returns the desired position, can be the player or the current target depending on the values of the bosshead script
    ///</summary>
    private Vector3 getDesriedPosition() {
        return lookAtPlayer ? bossAI.Boss.Player.Camera.transform.position :desiredAim.position;
    }

    private void Awake() {
        currentAim = new GameObject("head current aim").transform;
        currentAim.position = transform.position + transform.forward * 10f;
        currentAim.SetParent(bossAI.transform.parent);

        desiredAim = new GameObject("head desired aim").transform;
        desiredAim.position = transform.position + transform.forward * 10f;
        desiredAim.SetParent(bossAI.transform.parent);
        
        steeringBehaviour.target = currentAim;
        steeringBehaviour.desiredTarget = desiredAim;
    }

    public void PlayBossVoice() {
        if (voiceAudio == null) voiceAudio = AudioHandler.Instance.Play3DSound(SFXFiles.boss_talking, transform);
        voiceAudio.Play();
    }
    
    public void StopBossVoice() {
        if (voiceAudio != null) voiceAudio.Stop();
    }
    public void Update()
    {

        if (steeringEnabled) {
            steeringBehaviour.UpdatePosition(aimSpeed, getDesriedPosition());
        }
        else currentAim.position = Vector3.Lerp(currentAim.position, getDesriedPosition(), Time.deltaTime * aimSpeed);

        Quaternion oldRot = transform.localRotation;
        transform.LookAt(currentAim, transform.up);
        OnHeadAimUpdate?.Invoke(currentAim.position);

        if (Vector3.Angle(transform.forward, transform.parent.forward) > 90f) {
            transform.localRotation = oldRot;
        }
    }

    [SerializeField]
    private SteeringBehaviour steeringBehaviour;
    public SteeringBehaviour SteeringBehaviour {
        get { return steeringBehaviour;}
    }


    ///<summary>
    /// Sets the aim of the desired aim. 
    ///</summary>
    public void SetAim(Vector3 pos, Vector2 relativeOffset, bool localPos = false) {
        desiredAim.transform.SetParent(localPos ? transform.parent : bossAI.transform.parent);
        
        desiredAim.transform.position = pos + 
        transform.parent.up * relativeOffset.y + 
        transform.parent.right * relativeOffset.x;
}

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (currentAim != null && desiredAim != null) {
            Gizmos.DrawSphere(currentAim.position, .5f);
            Gizmos.DrawSphere(desiredAim.position, .5f);
            float multiply = (currentAim.position - getDesriedPosition()).magnitude / steeringBehaviour.DesiredVelocity.magnitude;
            Debug.DrawLine(currentAim.position, getDesriedPosition(), Gizmos.color);
            
            Debug.DrawLine(currentAim.position, currentAim.position + steeringBehaviour.Velocity * multiply , Color.red);
            Debug.DrawLine(currentAim.position + steeringBehaviour.Velocity * multiply, currentAim.position + steeringBehaviour.DesiredVelocity * multiply, Color.yellow);
            Debug.DrawLine(currentAim.position, currentAim.position + steeringBehaviour.DesiredVelocity * multiply, Color.green);
        }
    }

    public bool IsAtPosition(float offset = .1f, float velocityOffset = .1f) {
        if (Vector3.Distance(currentAim.position, desiredAim.position) > offset) return false;
        if (steeringEnabled) {
            // if (SteeringBehaviour.Velocity.magnitude > velocityOffset) return false;
        }
        return true;
    }
}

}