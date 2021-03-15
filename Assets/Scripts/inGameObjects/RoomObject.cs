using System.Collections;
using UnityEngine;

public class RoomObject : MonoBehaviour, IChangable
{
    [SerializeField]
    private string word;


    private Vector3 currentScale;
    private Quaternion currentRotation;
    private Quaternion missingRotation;


    public string Word {
        get { return word;}
        set {word = value;}
    }

    public bool animated { 
        get; set; 
    }

    public void SetChange(ChangeType changeType) {
        switch (changeType) {
            case ChangeType.missing:
                onMissing();
                break;
        }
    }
    public void RemoveChange(ChangeType changeType) {
        switch (changeType) {
            case ChangeType.missing:
                onAppearing();
                break;
        }
    }
    public void onAppearing()
    {
        gameObject.SetActive(true);
        StartCoroutine(Appearing());
    }

    public void onMissing()
    {
        currentScale = transform.localScale;
        currentRotation = transform.rotation;
        missingRotation = Quaternion.Euler(currentRotation.eulerAngles.x + 45, currentRotation.eulerAngles.y + 90, currentRotation.eulerAngles.z);
        StartCoroutine(Dissappearing());
    }
    public virtual IEnumerator Dissappearing() {

        if (animated) {
            AnimationCurve curve = AnimationCurve.EaseInOut(0,1,3,0);

            float timePassed = 0f;
            while (transform.localScale.x > 0) {
                yield return new WaitForFixedUpdate();
                timePassed += Time.deltaTime;
                transform.localScale = currentScale * curve.Evaluate(timePassed);
                //transform.localRotation = Quaternion.Lerp(currentRotation, missingRotation, (1- curve.Evaluate(timePassed)));
            }
        }
        transform.localScale = new Vector3(0,0,0);
        gameObject.SetActive(false);
    }
    public virtual IEnumerator Appearing() {

        if (animated) {
            AnimationCurve curve = AnimationCurve.EaseInOut(0,0,3,1);

            float timePassed = 0f;
            while (transform.localScale.x < currentScale.x) {
                yield return new WaitForFixedUpdate();
                timePassed += Time.deltaTime;
                transform.localScale = currentScale * curve.Evaluate(timePassed);
                //transform.localRotation = Quaternion.Lerp(currentRotation, missingRotation, (1- curve.Evaluate(timePassed)));
            }
        }
        transform.localScale = currentScale;
    }
}