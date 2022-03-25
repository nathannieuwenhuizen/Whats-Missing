using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The values of the boss looking state are defined here.
///</summary>
[System.Serializable]
public class BossEye {
    ///<summary>
    /// The range on which the boss can still view the player
    [Range(1, 200)]
    ///</summary
    public float viewRange = 10;
    ///<summary>
    /// The angle the boss can still see the player
    ///</summary>
    [Range(1, 70)]
    public float viewAngle = 25;
    ///<summary>
    /// The current value on how much the boss notices the palyer
    ///</summary>
    [HideInInspector]
    public float noticingValue = 0;
    ///<summary>
    /// The noticing threshhold, when the noticing value exceeds the threshhold the boss knows that the player is there.
    ///</summary>
    public float noticingThreshold = 2;
    ///<summary>
    /// The min speed the noticing values should go up when the player is in view  but is far away.
    ///</summary>
    public float noticingMinSpeed = 1;
    ///<summary>
    /// The min speed the noticing values should go up when the player is in view and is super close.
    ///</summary>
    public float noticingMaxSpeed = 2;

    public Transform eyeTransform;

    private RaycastHit hit;
    public bool PlayerIsInView(Player player) {
        if (eyeTransform!= null && player != null) {
            //if player is in the viewshape 
            float angle = Vector3.Angle(eyeTransform.forward, player.Camera.transform.position - eyeTransform.position);
            if (angle < viewAngle && distanceBetweenPlayer(player) < viewRange) {
                //then do two raycasts, on at the players feet and on at the main cam.
                if (!Physics.Raycast(eyeTransform.position, player.Camera.transform.position - eyeTransform.position, out hit, viewRange)) {
                    return true;
                }
                if (!Physics.Raycast(eyeTransform.position, (player.Camera.transform.position - player.transform.up * 2f) - eyeTransform.position, out hit, viewRange)) {
                    return true;
                }
                // return true;
            }
        }
        return false;
    }
    public float distanceBetweenPlayer(Player player) {
        return Vector3.Distance(eyeTransform.position, player.Camera.transform.position);
    }

    private Color debugColor;
    public void OnDrawGizmos(Boss boss) {
        if (eyeTransform != null) {
            bool inView = PlayerIsInView(boss.Player);
            Gizmos.color = debugColor = inView ? Color.yellow : Color.red;
            Debug.DrawLine(eyeTransform.position, eyeTransform.position + eyeTransform.forward * viewRange, debugColor );
            
            if (inView) Debug.DrawLine(eyeTransform.position, hit.point, debugColor );

            float opposideLength = Mathf.Tan(viewAngle * Mathf.Deg2Rad);

            Vector3 right = DebugDrawViewLine(eyeTransform.position, eyeTransform.position + (eyeTransform.forward + eyeTransform.right * opposideLength).normalized * viewRange );
            Vector3 left = DebugDrawViewLine(eyeTransform.position, eyeTransform.position + (eyeTransform.forward -eyeTransform.right * opposideLength).normalized * viewRange);
            Vector3 up = DebugDrawViewLine(eyeTransform.position, eyeTransform.position + (eyeTransform.forward + eyeTransform.up * opposideLength).normalized * viewRange);
            Vector3 down = DebugDrawViewLine(eyeTransform.position, eyeTransform.position + (eyeTransform.forward -eyeTransform.up * opposideLength).normalized * viewRange);
            float crossSteps = 4f;
            for (int j = 0; j <= crossSteps; j++) {
                opposideLength = Mathf.Tan(viewAngle * ((crossSteps - j)/ crossSteps) * Mathf.Deg2Rad);
                right = DebugDrawViewLine(right, eyeTransform.position + (eyeTransform.forward + eyeTransform.right * opposideLength).normalized * viewRange );
                left = DebugDrawViewLine(left, eyeTransform.position + (eyeTransform.forward -eyeTransform.right * opposideLength).normalized * viewRange);
                up = DebugDrawViewLine(up, eyeTransform.position + (eyeTransform.forward + eyeTransform.up * opposideLength).normalized * viewRange);
                down = DebugDrawViewLine(down, eyeTransform.position + (eyeTransform.forward -eyeTransform.up * opposideLength).normalized * viewRange);
            }


            float arcSteps = 5 * 4f;
            opposideLength = Mathf.Tan(viewAngle * Mathf.Deg2Rad);
            Quaternion oldRot = eyeTransform.rotation;
            up = DebugDrawViewLine(eyeTransform.position, eyeTransform.position + (eyeTransform.forward + eyeTransform.up * opposideLength).normalized * viewRange);

            for (int j = 0; j <= arcSteps; j++) {
                eyeTransform.Rotate(new Vector3(0,0, 360 / arcSteps), Space.Self);
                up = DebugDrawViewLine(up, eyeTransform.position + (eyeTransform.forward + eyeTransform.up * opposideLength).normalized * viewRange);
            }
            eyeTransform.rotation = oldRot;

        }
    }
    private Vector3 DebugDrawViewLine(Vector3 origin, Vector3 dest) {
        Debug.DrawLine(origin, dest, debugColor);
        return dest;
    }
}
