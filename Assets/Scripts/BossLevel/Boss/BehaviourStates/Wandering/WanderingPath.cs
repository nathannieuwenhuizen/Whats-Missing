using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WanderPose {
    [SerializeField]
    public Transform position;
    [SerializeField]
    public Transform aimPosition;
}

public class WanderingPath : MonoBehaviour {
    public WanderPose[] poses;
    
    private void OnDrawGizmos() {
        if (poses.Length < 1) return;
        
        if (poses[0].position == null) return;
        Vector3 startpos = poses[0].position.position;
        for (int i = 0; i < poses.Length; i++)
        {
            if (poses[i].position != null) {
                
                Gizmos.DrawWireSphere(poses[i].position.position, .5f);

                Vector3 endPos = startpos;
                if (poses.Length - 2 < i ) endPos = poses[0].position.position;
                else if (poses[i + 1].position != null) endPos = poses[i + 1].position.position;
                Debug.DrawLine(startpos, endPos);
                startpos = endPos;
            }
        }
    }
}
