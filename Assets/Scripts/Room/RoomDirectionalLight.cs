using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDirectionalLight : MonoBehaviour
{

    private Quaternion startRotation;

    private float duration = 1f;
    public bool animating = true;
    private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    private void Awake() {
        startRotation = transform.rotation;
    }

    public void RotateToMatchRoon(Transform room) {
        StartCoroutine(AnimateRotation(room));
    }

    private IEnumerator AnimateRotation(Transform room) {
        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(
            room.transform.localRotation.eulerAngles.x + startRotation.eulerAngles.x, 
            room.transform.localRotation.eulerAngles.y + startRotation.eulerAngles.y, 
            room.transform.localRotation.eulerAngles.z + startRotation.eulerAngles.z
            );

        float index = 0;
        if (animating) {
            while (index < duration) {
                transform.localRotation = Quaternion.SlerpUnclamped(start, end, rotationCurve.Evaluate(index/ duration));
                yield return new WaitForEndOfFrame();
                index += Time.deltaTime;
            }
        }
        transform.localRotation = end;
    }
}
