using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static  IEnumerator AnimatingLocalScale(this Transform transform, Vector3 endScale,  AnimationCurve curve, float duration = .5f) {
        float timePassed = 0f;
        Vector3 beginScale = transform.localScale;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            transform.localScale = Vector3.LerpUnclamped(beginScale, endScale , curve.Evaluate(timePassed / duration));
        }
        transform.localScale = endScale;
    }
    public static IEnumerator AnimatingRoomObjectScale(this RoomObject roomObject, float endScale,  AnimationCurve curve, float duration = .5f) {
        float timePassed = 0f;
        float beginScale = roomObject.CurrentScale;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            roomObject.CurrentScale = Mathf.LerpUnclamped(beginScale, endScale , curve.Evaluate(timePassed / duration));
        }
        roomObject.CurrentScale = endScale;
    }

    public static  IEnumerator AnimatingLocalPos(this Transform transform, Vector3 endLocalPos,  AnimationCurve curve, float duration = .5f) {
        float timePassed = 0f;
        Vector3 beginPos = transform.localPosition;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            transform.localPosition = Vector3.LerpUnclamped(beginPos, endLocalPos , curve.Evaluate(timePassed / duration));
        }
        transform.localPosition = endLocalPos;
    }
    public static  IEnumerator AnimatingPos(this Transform transform, Vector3 endPos,  AnimationCurve curve, float duration = .5f) {
        float timePassed = 0f;
        Vector3 beginPos = transform.position;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            transform.position = Vector3.LerpUnclamped(beginPos, endPos , curve.Evaluate(timePassed / duration));
        }
        transform.position = endPos;
    }
    public static IEnumerator AnimatingFlip(this Transform transform, Renderer renderer, float duration = 5f, FlippingAxis flippingAxis = FlippingAxis.up) {

        float timePassed = 0f;
        float oldTimepaseed = 0;
        AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            timePassed = Mathf.Min(duration, timePassed);
            float angle = curve.Evaluate(timePassed / duration) - curve.Evaluate(oldTimepaseed / duration);
            Vector3 axis = transform.up;
            if (flippingAxis == FlippingAxis.forward) axis = transform.forward;
            else if (flippingAxis == FlippingAxis.right) axis = transform.right;
            transform.RotateAround(renderer.bounds.center, axis, 180 * (angle));
            oldTimepaseed = timePassed;
        }
    }

    public static  IEnumerator AnimatingPosBounce(this Transform transform, float amplitude,  AnimationCurve curve, float duration = 5f) {
        float timePassed = 0f;
        Vector3 beginPos = transform.position;
        float currentHeight = 0;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            currentHeight = Mathf.Sin(Mathf.PI * (curve.Evaluate(timePassed / duration))) * amplitude;
            transform.position = beginPos + new Vector3(0,currentHeight, 0);
            // transform.position = Vector3.LerpUnclamped(beginPos, endPos , curve.Evaluate(timePassed / duration));
        }
        transform.position = beginPos;
    }

    public static  IEnumerator AnimatingPosBezierCurve(this Transform transform, Vector3 end, Vector3 mid, AnimationCurve curve, float duration = 5f) {
        float index = 0f;
        Vector3 begin = transform.position;
        while (index < duration) {
            yield return new WaitForEndOfFrame();
            index += Time.deltaTime;
            transform.position = Extensions.CalculateQuadraticBezierPoint(curve.Evaluate(index / duration),  begin, mid, end);
        }
        transform.position = end;
    }


    public static  IEnumerator AnimatingLocalRotation(this Transform transform, Quaternion endrotation,  AnimationCurve curve, float duration = .5f) {
        float timePassed = 0f;
        Quaternion beginrotation = transform.rotation;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            transform.rotation = Quaternion.SlerpUnclamped(beginrotation, endrotation , curve.Evaluate(timePassed / duration));
        }
        transform.rotation = endrotation;
    }

    public static  IEnumerator Shake(this Transform transform, float magintude, float frequence, float duration = .5f) {
        float timePassed = 0f;
        while (timePassed < duration) {
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
            float currentMagnitude = Mathf.Sin(Mathf.PI * (timePassed / duration)) * magintude;
            transform.localRotation = Quaternion.Euler(
                transform.localRotation.x,
                transform.localRotation.y,
                Mathf.Sin((timePassed * frequence) * (Mathf.PI * 2)) * currentMagnitude
            );
        }
        transform.localRotation = Quaternion.Euler(
                transform.localRotation.x,
                transform.localRotation.y,
                0
            );
    }
    public static Quaternion RandomRotation (float amplitude = 1f) {
        return Quaternion.Euler(Random.Range(0.0f, 360.0f * amplitude), Random.Range(0.0f, 360.0f * amplitude), Random.Range(0.0f, 360.0f * amplitude));
    }
    public static Vector3 RandomVector (float amplitude = 1f) {
        return new Vector3(Random.Range(0.0f, amplitude), Random.Range(0.0f, amplitude), Random.Range(0.0f, amplitude));
    }


}
