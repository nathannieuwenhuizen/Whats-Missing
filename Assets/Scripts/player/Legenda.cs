using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legenda : MonoBehaviour
{

    public delegate void FocusedAction(string changable);
    public static event FocusedAction OnFocus; 
    private Ray ray;
    private RaycastHit hit;
    


    private float maxDistance = 100f;
    private string word = "";

    public string Word {
        get {
            return word;
        }
        set {
            if (word != value) {
                word = value;
                OnFocus?.Invoke(word.ToLower());
            }
        }
    }

    void Update()
    {
        ray = new Ray(transform.position, transform.forward * maxDistance);
        IChangable focussedObject = FocussedObject<IChangable>();
        if (focussedObject != default(IChangable)) {
            Word = focussedObject.Word;
        } else {
            Word = "";
        }

    }
    //raycast froward from the camera to any object that has the component T with it.
    private T FocussedObject<T>() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance)) {
            if (hit.collider.gameObject.GetComponent<T>() != null) {
                return hit.collider.gameObject.GetComponent<T>();
            } else if (hit.collider.transform.parent != null) {
                if (hit.collider.transform.parent.GetComponent<T>() != null) 
                    return hit.collider.transform.parent.GetComponent<T>();

                if (hit.collider.transform.parent.parent != null) {
                    if (hit.collider.transform.parent.parent.GetComponent<T>() != null) 
                        return hit.collider.transform.parent.parent.GetComponent<T>();
            }
            } 
        }
        return default(T);
    }
}
