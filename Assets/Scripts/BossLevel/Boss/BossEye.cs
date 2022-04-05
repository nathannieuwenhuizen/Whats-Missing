using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The values of the boss looking state are defined here.
///</summary>
public class BossEye: MonoBehaviour {
    //used to determine the fake light aspects
    private readonly float sizeAspect = 5f;
    private readonly float angleAspect = 27f;

    [SerializeField]
    private Transform fakeLight;
    [SerializeField]
    private MeshRenderer fakeLightRenderer;

    [SerializeField]
    private bool lightIsOn = false;
    public bool LightIsOn {
        get { return lightIsOn;}
        set { 
            lightIsOn = value; 
            UpdateFakeLight();
        }
    }


    [SerializeField]
    private bool showGizmo = true;
    
    [Range(1, 200)]
    [SerializeField]
    private float viewRange = 10;
    public float ViewRange {
        get { return viewRange;}
        set {
            viewRange = value;
            UpdateFakeLight();
        }
    }
    ///<summary>
    /// The angle the boss can still see the player
    ///</summary>
    [Range(1, 70)]
    [SerializeField]
    private float viewAngle = 25;
    public float ViewAngle {
        get { return viewAngle;}
        set { viewAngle = value; 
            UpdateFakeLight();
        }
    }
    [ColorUsage(true, true)]
    [SerializeField]
    private Color viewColor;
    public Color ViewColor {
        get { return viewColor;}
        set { viewColor = value; 
            UpdateFakeLight();
        }
    }

    ///<summary>
    /// The current value on how much the boss notices the palyer
    ///</summary>
    [HideInInspector]
    public float noticingValue = 0;
    public float NoticingValue {
        get { return noticingValue;}
        set { noticingValue = Mathf.Clamp(value, 0, noticingThreshold); }
    }
    ///<summary>
    /// The noticing threshhold, when the noticing value exceeds the threshhold the boss knows that the player is there.
    ///</summary>
    [SerializeField]
    private float noticingThreshold = 2;
    public float NoticingThreshold {
        get { return noticingThreshold;}
    }
    ///<summary>
    /// The min speed the noticing values should go up when the player is in view  but is far away.
    ///</summary>
    [SerializeField]
    private float noticingMinSpeed = 1;
    ///<summary>
    /// The min speed the noticing values should go up when the player is in view and is super close.
    ///</summary>
    [SerializeField]
    private float noticingMaxSpeed = 2;

    [SerializeField]
    private Boss boss;

    private RaycastHit hit;
    public bool PlayerIsInView(Player player) {
        if (transform!= null && player != null) {
            float angle = Vector3.Angle(transform.forward, player.Camera.transform.position - transform.position);
            float dist = distanceBetweenPlayer(player);
            //if player is in the viewshape 
            if (angle < viewAngle && dist < viewRange) {
                //then do two raycasts, on at the players feet and on at the main cam.
                if (!Physics.Raycast(transform.position, player.Camera.transform.position - transform.position, out hit, dist)) {
                    return true;
                }
                if (!Physics.Raycast(transform.position, (player.Camera.transform.position - player.transform.up * 2f) - transform.position, out hit, dist)) {
                    return true;
                }
            }
        } 
        return false;
    }
    public float distanceBetweenPlayer(Player player) {
        return Vector3.Distance(transform.position, player.Camera.transform.position);
    }

    public void UpdateNoticing(Player player) {
        if (PlayerIsInView(player)) {
            float distance = (transform.position - player.Camera.transform.position).magnitude;
            float viewPrecentage = distance / ViewRange;
            //player is real close!
            if (viewPrecentage < 0.5f) NoticingValue += noticingMaxSpeed * Time.deltaTime;
            //player far away but still in view
            else NoticingValue += noticingMinSpeed * Time.deltaTime;

        } else {
             NoticingValue -= noticingMinSpeed * Time.deltaTime;
        }
    }


    public bool NoticesPlayer { get => noticingValue >= noticingThreshold; }
    public bool DoesntNoticesPlayer { get => noticingValue <= 0; }

    private Color debugColor;
    public void OnDrawGizmos() {
        if (!showGizmo) return;
        if (boss != null) {
            bool inView = PlayerIsInView(boss.Player);
            Gizmos.color = debugColor = inView ? Color.yellow : Color.red;
            Debug.DrawLine(transform.position, transform.position + transform.forward * viewRange, debugColor );
            
            if (inView) Debug.DrawLine(transform.position, hit.point, debugColor );

            float opposideLength = Mathf.Tan(viewAngle * Mathf.Deg2Rad);

            Vector3 right = DebugDrawViewLine(transform.position, transform.position + (transform.forward + transform.right * opposideLength).normalized * viewRange );
            Vector3 left = DebugDrawViewLine(transform.position, transform.position + (transform.forward -transform.right * opposideLength).normalized * viewRange);
            Vector3 up = DebugDrawViewLine(transform.position, transform.position + (transform.forward + transform.up * opposideLength).normalized * viewRange);
            Vector3 down = DebugDrawViewLine(transform.position, transform.position + (transform.forward -transform.up * opposideLength).normalized * viewRange);
            float crossSteps = 4f;
            for (int j = 0; j <= crossSteps; j++) {
                opposideLength = Mathf.Tan(viewAngle * ((crossSteps - j)/ crossSteps) * Mathf.Deg2Rad);
                right = DebugDrawViewLine(right, transform.position + (transform.forward + transform.right * opposideLength).normalized * viewRange );
                left = DebugDrawViewLine(left, transform.position + (transform.forward -transform.right * opposideLength).normalized * viewRange);
                up = DebugDrawViewLine(up, transform.position + (transform.forward + transform.up * opposideLength).normalized * viewRange);
                down = DebugDrawViewLine(down, transform.position + (transform.forward -transform.up * opposideLength).normalized * viewRange);
            }


            float arcSteps = 5 * 4f;
            opposideLength = Mathf.Tan(viewAngle * Mathf.Deg2Rad);
            Quaternion oldRot = transform.rotation;
            up = DebugDrawViewLine(transform.position, transform.position + (transform.forward + transform.up * opposideLength).normalized * viewRange);

            for (int j = 0; j <= arcSteps; j++) {
                transform.Rotate(new Vector3(0,0, 360 / arcSteps), Space.Self);
                up = DebugDrawViewLine(up, transform.position + (transform.forward + transform.up * opposideLength).normalized * viewRange);
            }
            transform.rotation = oldRot;
            UpdateFakeLight();
        }
    }
    private void UpdateFakeLight() {
        if (fakeLight == null) return;

        fakeLight.gameObject.SetActive(lightIsOn);
        Vector3 scale = Vector3.one;
        scale.y = ViewRange / sizeAspect;
        float angleResult = Mathf.Tan(viewAngle * Mathf.Deg2Rad);
        scale.x = ViewRange / sizeAspect * angleResult;
        scale.z = ViewRange / sizeAspect * angleResult;
        fakeLight.localScale = scale;

        if (fakeLightRenderer != null) {
            fakeLightRenderer.sharedMaterial.SetColor("_color", viewColor);
        } 
    }
    private Vector3 DebugDrawViewLine(Vector3 origin, Vector3 dest) {
        Debug.DrawLine(origin, dest, debugColor);
        return dest;
    }
}
