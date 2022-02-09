using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static  IEnumerator AnimatingScale(this Transform transform, Vector3 endScale,  AnimationCurve curve, float duration = .5f) {
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
    public static  IEnumerator AnimatingRotation(this Transform transform, Quaternion endrotation,  AnimationCurve curve, float duration = .5f) {
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


}
