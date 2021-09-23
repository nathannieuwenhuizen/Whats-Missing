using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ChangeLine : MonoBehaviour
{
    [SerializeField]
    private Vector3 point0, point1, point2 = new Vector3();
    private float duration = 2f;

    private ParticleSystem particleSystem;

    private AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);


    private void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
    }
    public void SetDestination( Vector3 begin, Vector3 end) {
        point0 = begin;
        point2 = end;
        point1 = begin + ((end - begin) / 2f) + new Vector3(0,5,0);
    }

    public IEnumerator MoveTowardsDestination() {
        float index = 0;
        while (index < duration) {
            index += Time.unscaledDeltaTime;
            transform.position = Extensions.CalculateQuadraticBezierPoint(curve.Evaluate(index / duration), point0, point1, point2);
            yield return new WaitForEndOfFrame();
        }
        particleSystem.startSpeed = particleSystem.startSpeed * 2;
        particleSystem.Emit(100);
        Destroy(gameObject, 2f);
    }

    private int numberOfPoints = 50;
    private void OnDrawGizmos() {
        Vector3 beginPos = point0;
        Gizmos.DrawWireSphere(point0, .5f);
        Gizmos.DrawWireSphere(point1, .5f);
        Gizmos.DrawWireSphere(point2, .5f);
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

    private Vector3 CalculateSpiralBezierCurve(float t, Vector3 point0, Vector3 point1, Vector3 point2) {
        Vector3 pointa = Extensions.CalculateQuadraticBezierPoint(t, point0, point1, point2);
        Vector3 pointb = Extensions.CalculateQuadraticBezierPoint(t + 0.01f, point0, point1, point2);
        Vector3 dir = pointb - pointa;
        
        return Extensions.CalculateQuadraticBezierPoint(t, point0, point1, point2);
    }


}
