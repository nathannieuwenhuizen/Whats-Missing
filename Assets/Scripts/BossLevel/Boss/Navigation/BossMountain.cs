using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {

public class BossMountain : MonoBehaviour
{
    [SerializeField]
    private Color debugColor = new Color(1,1,1,.5f);
    public Color DebugColor {
        get { return debugColor;}
    }

    [Range(4, 100)]
    [SerializeField]
    private int debugLines = 10;
    [SerializeField]
    private bool enableDebug = true;
    [SerializeField]
    private bool enablePathDebug = true;

    public Transform test;

    [SerializeField]
    private float topRadius = 20f;
    public float TopRadius {
        get { return topRadius;}
    }
    [SerializeField]
    private float bottomRadius = 50f;
    public float BottomRadius {
        get { return bottomRadius;}
    }

    [SerializeField]
    private float height = 100f;
    public float Height {
        get { return height;}
    }

    // [Space]
    // [Header("path")]
    // [SerializeField]
    // private MountainPath path;
    // public MountainPath Path {
    //     get { return path;}
    // }

    [SerializeField]
    private bool onSurface;
    public bool OnSurface {
        get { return onSurface;}
        set { onSurface = value; }
    }

    public bool InsideShape(Vector3 _position){
        float precentage = (_position.y - transform.position.y) / height;
        float radius = Mathf.Lerp(bottomRadius, topRadius, precentage);
        Vector3 delta = _position - transform.position;
        delta.y = 0;
        return delta.magnitude < radius;
        // return false;
    }
    // public Vector3 GetAimPosition


    [SerializeField]
    private Boss boss;

    public Boss Boss {
        get { return boss;}
    }

    private void OnDrawGizmos() {
        if (!enableDebug) return;

        Gizmos.color = debugColor;
        DebugExtensions.DrawCircle(transform.position, bottomRadius, debugColor, 360, 20);
        DebugExtensions.DrawCircle(transform.position + new Vector3(0,height, 0), topRadius, debugColor, 360, 20);
        for (int j = 0; j < 360; j+= (360 / debugLines)) {
            MountainCoordinate coords = new MountainCoordinate() {Angle = j, yPos = transform.position.y};
            MountainCoordinate coordsEnd = new MountainCoordinate() {Angle = j, yPos = transform.position.y + height};
            Debug.DrawLine(coords.ToPrimitiveVector(this), coordsEnd.ToPrimitiveVector(this), debugColor);
        }
        // if (enablePathDebug){
        //     path.begin = MountainCoordinate.FromPosition(this, boss.transform.position);
        //     path.DrawGizmo(this);
        // }

        if (test != null) {
            Gizmos.color = InsideShape(test.position) ? Color.green : Color.red;
            Gizmos.DrawSphere(test.position, 2f);
        }
    }

}
}