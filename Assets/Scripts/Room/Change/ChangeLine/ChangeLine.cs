using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// A particle effect that goes towardsa destination to create a cool effect.
///</summary>
[RequireComponent(typeof(ParticleSystem))]
public class ChangeLine : MonoBehaviour
{
    [SerializeField]
    private Vector3 point0, point1, point2 = new Vector3();
    private float duration = 2f;

    private ParticleSystem ps;

    private AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);


    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }
    public void SetDestination( Vector3 begin, Vector3 end) {
        point0 = begin;
        point2 = end;
        point1 = begin + ((end - begin) / 2f) + new Vector3(0,5,0);
    }

    ///<summary>
    /// Moves the position of the changeline towards the destination.
    ///</summary>
    public IEnumerator MoveTowardsDestination() {
        float index = 0;
        while (index < duration) {
            index += Time.deltaTime;
            transform.position = Extensions.CalculateQuadraticBezierPoint(curve.Evaluate(index / duration), point0, point1, point2);
            yield return new WaitForEndOfFrame();
        }
        ps.startSpeed = ps.startSpeed * 2;
        ps.Emit(100);
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmos() {
        int numberOfPoints = 50;
        Gizmos.DrawWireSphere(point0, .5f);
        Gizmos.DrawWireSphere(point1, .5f);
        Gizmos.DrawWireSphere(point2, .5f);
        Vector3 beginPos = point0;
        if (point0 != null && point1 != null && point2 != null) {

            for (int i = 1; i < numberOfPoints + 1; i++)
            {
                float t = i / (float)numberOfPoints;
                Vector3 newPos =  Extensions.CalculateQuadraticBezierPoint(t, point0, point1, point2);
                Debug.DrawLine(beginPos, newPos);
                beginPos = newPos;
            }
        }
    }

    ///<summary>
    /// Returns a Vector3 point based on the three points of the bezier curve.
    ///</summary>
    private Vector3 CalculateSpiralBezierCurve(float t, Vector3 point0, Vector3 point1, Vector3 point2) {
        Vector3 pointa = Extensions.CalculateQuadraticBezierPoint(t, point0, point1, point2);
        Vector3 pointb = Extensions.CalculateQuadraticBezierPoint(t + 0.01f, point0, point1, point2);
        Vector3 dir = pointb - pointa;
        Vector3 right = (Quaternion.AngleAxis(-180, dir) * dir);

        return pointa + right * 30f;
        // return Extensions.CalculateQuadraticBezierPoint(t, point0, point1, point2);
    }



}
