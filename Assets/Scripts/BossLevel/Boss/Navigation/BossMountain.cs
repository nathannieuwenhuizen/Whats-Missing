using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boss {
    [System.Serializable]
    public struct MountainShapeData {
        public float BottomRadius;
        public float TopRadius;
        public float Height;
    }

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

    [SerializeField]
    private MountainShapeData shapeData;

    private MountainShapeData testShape;
    public MountainShapeData ShapeData {
        get { return shapeData;}
    }

    private MountainShapeData oldShapeData;


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
        float precentage = (_position.y - transform.position.y) / shapeData.Height;
        float radius = Mathf.Lerp(shapeData.BottomRadius, shapeData.TopRadius, precentage);
        Vector3 delta = _position - transform.position;
        delta.y = 0;
        return delta.magnitude < radius * 1.3f;
        // return false;
    }
    // public Vector3 GetAimPosition


    [SerializeField]
    private Boss boss;

    public Boss Boss {
        get { return boss;}
    }

    ///<summary>
    /// Returns a new shapedata that fits with the points on the mountain
    ///</summary>
    private MountainShapeData GenerateCustomShapeData(Vector3 a, Vector3 b){
        //check which one is the highest point vertically
        Vector3 low = a.y < b.y ? a : b;
        Vector3 high = a.y > b.y ? a : b;

        //calculate the radius of hte low point
        Vector3 lowDelta = low - transform.position;
        lowDelta.y = 0;
        float radiusLow = lowDelta.magnitude;

        //calculate the radius of hte highpoint
        Vector3 highDelta = high - transform.position;
        highDelta.y = 0;
        float radiusHigh = highDelta.magnitude;

        //calculate the delta
        float radiusDelta = (radiusHigh - radiusLow) / (high.y - low.y);

        //calculate the result
        float newBottomRadius = radiusLow - (low.y - transform.position.y) * radiusDelta;
        float newTopRadius = newBottomRadius + (shapeData.Height * radiusDelta);

        //return result
        return new MountainShapeData() {Height = shapeData.Height, BottomRadius = newBottomRadius, TopRadius = newTopRadius};
    }

        
    public void MakeMountainFit(Vector3 a, Vector3 b) {
        if (!oldShapeData.Equals(default(MountainShapeData))) return;
        SaveShape();
        shapeData = GenerateCustomShapeData(a, b);
    }

    ///<summary>
    /// Saves the shape data of the mountain
    ///</summary>
    private void SaveShape() {
        oldShapeData = shapeData;
    }

    ///<summary>
    /// Restores the shapedata to that of hte old shape data
    ///</summary>
    public void RestoreShape() {
        shapeData = oldShapeData;
        oldShapeData = default(MountainShapeData);
    }

    private void OnDrawGizmosSelected() {
        if (!enableDebug) return;

        Gizmos.color = debugColor;
        DrawMountainShape(ShapeData);

        // if (enablePathDebug){
        //     path.begin = MountainCoordinate.FromPosition(this, boss.transform.position);
        //     path.DrawGizmo(this);
        // }
    }

    private void DrawMountainShape(MountainShapeData _shapeData) {
        DebugExtensions.DrawCircle(transform.position, _shapeData.BottomRadius, debugColor, 360, 20);
        DebugExtensions.DrawCircle(transform.position + new Vector3(0,_shapeData.Height, 0), _shapeData.TopRadius, debugColor, 360, 20);
        for (int j = 0; j < 360; j+= (360 / debugLines)) {
            MountainCoordinate coords = new MountainCoordinate() {Angle = j, yPos = transform.position.y};
            MountainCoordinate coordsEnd = new MountainCoordinate() {Angle = j, yPos = transform.position.y + _shapeData.Height};
            Debug.DrawLine(coords.ToPrimitiveVector(this), coordsEnd.ToPrimitiveVector(this), debugColor);
        }

    }

}
}